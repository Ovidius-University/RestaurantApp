using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebAppUI.Data;
using WebAppUI.Models.Entities;
using WebAppUI.Models.CustomIdentity;
using Microsoft.AspNetCore.Identity;
using WebAppUI.Areas.Admin.Models.ViewModels;
using WebAppUI.Areas.Admin.Models.DTOs;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using System.Drawing;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace WebAppUI.Areas.Admin.Controllers;
[Area("Admin")]
[Authorize(Roles = "Administrator")]

public class ChefController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly UserManager<AppUser> _userManager;

    public ChefController(ApplicationDbContext context, IMapper mapper, UserManager<AppUser> userManager)
    {
        _context = context;
        _mapper = mapper;
        _userManager = userManager;
    }

    [Route("Admin/Chefs")]
    public async Task<IActionResult> Index()
    {
        var foods = await _context.Foods.Where(c => c.IsHomeMade == true).ToListAsync();

        var emptychefs = _mapper.Map<List<ExistentUserDto>>(await _userManager.GetUsersInRoleAsync("Chef"));

        var query = foods.GroupBy(name => new { name.ChefId })
            .Select(pro => new
            {
                ChefId = pro.Key.ChefId,
                Quantity = pro.Count()
            });

        var chefs = new List<ChefVm>();

        foreach (var item in query)
        {
            var pro = emptychefs
                      .Where(i => i.Id == item.ChefId)
                      .FirstOrDefault();

            var name = pro!.Email;

            chefs.Add(new ChefVm()
            {
                ChefId = (int)item.ChefId!,
                Name = name!,
                Foods = item.Quantity
            });
        }

        foreach (var item in emptychefs)
        {
            var name = item!.Email;
            var sch = chefs.FirstOrDefault(i => i.ChefId == item.Id);

            if (sch == null)
            {
                chefs.Add(new ChefVm()
                {
                    ChefId = (int)item.Id!,
                    Name = name!,
                    Foods = 0
                });
            }
        }

        var output = new IndexChefVm()
        {
            ListChefs = chefs.OrderBy(c => c.ChefId).ToList()
        };
        return View(output);
    }

    // GET: Chef/Details/5
    [Route("Admin/Chef/Details")]
    public async Task<IActionResult> Details(int? id)
    {
        var foods = _mapper.Map<List<FoodVm>>(
            await _context.Foods
            .Include(c => c.Category)
            .Include(c => c.Offer)
            .Where(c => c.IsHomeMade == true && c.ChefId == id)
            .ToListAsync());

        var emptychefs = _mapper.Map<List<ExistentUserDto>>(await _userManager.GetUsersInRoleAsync("Chef"));
        var pro = emptychefs
                  .Where(i => i.Id == id)
                  .FirstOrDefault();

        var name = pro!.Email;

        var output = new IndexChefFoodVm()
        {
            Name = name,
            ListFoods = foods
        };
        return View(output);
    }
}
