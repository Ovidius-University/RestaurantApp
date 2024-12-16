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

public class FoodProviderController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly UserManager<AppUser> _userManager;

    public FoodProviderController(ApplicationDbContext context, IMapper mapper, UserManager<AppUser> userManager)
    {
        _context = context;
        _mapper = mapper;
        _userManager = userManager;
    }

    [Route("Admin/FoodProviders")]
    public async Task<IActionResult> Index()
    {
        if (_context.Providers == null)
            return Problem("Entity set 'ApplicationDbContext.Providers' is null.");

        var ingredients = await _context.Ingredients.ToListAsync();

        var foods = await _context.Foods.Where(c => c.IsHomeMade == false).ToListAsync();

        var emptyprovs = await _context.Providers.ToListAsync();

        var query1 = ingredients.GroupBy(name => new { name.ProviderId })
            .Select(pro => new
            {
                ProviderId = pro.Key.ProviderId,
                Quantity = pro.Count()
            });

        var query2 = foods.GroupBy(name => new { name.ProviderId })
            .Select(pro => new
            {
                ProviderId = pro.Key.ProviderId,
                Quantity = pro.Count()
            });

        var query = query1.Join(query2, query1 => query1.ProviderId, query2 => query2.ProviderId,
        (query1, query2) => new { ProviderId = query1.ProviderId, Ingredients = query1.Quantity, Foods = query2.Quantity });

        var providers = new List<FoodProviderVm> ();

        foreach (var item in query)
        {
            var pro = await _context.Providers
                        .Where(i => i.Id == item.ProviderId)
                        .FirstOrDefaultAsync();

            var name = pro!.Name;

            providers.Add(new FoodProviderVm()
            {
                ProviderId = (int)item.ProviderId!,
                Name = name!,
                Ingredients = item.Ingredients,
                Foods = item.Foods,
            });
        }

        foreach (var item in query1)
        {
            var pro = await _context.Providers
                        .Where(i => i.Id == item.ProviderId)
                        .FirstOrDefaultAsync();

            var name = pro!.Name;
            var sch = providers.FirstOrDefault(i => i.ProviderId == item.ProviderId);

            if (sch == null)
            {
                providers.Add(new FoodProviderVm()
                {
                    ProviderId = (int)item.ProviderId!,
                    Name = name!,
                    Ingredients = item.Quantity,
                    Foods = 0,
                });
            }
        }

        foreach (var item in query2)
        {
            var pro = await _context.Providers
                        .Where(i => i.Id == item.ProviderId)
                        .FirstOrDefaultAsync();

            var name = pro!.Name;
            var sch = providers.FirstOrDefault(i => i.ProviderId == item.ProviderId);

            if (sch == null)
            {
                providers.Add(new FoodProviderVm()
                {
                    ProviderId = (int)item.ProviderId!,
                    Name = name!,
                    Ingredients = 0,
                    Foods = item.Quantity,
                });
            }
        }

        foreach (var item in emptyprovs)
        {
            var name = item!.Name;
            var sch = providers.FirstOrDefault(i => i.ProviderId == item.Id);

            if (sch == null)
            {
                providers.Add(new FoodProviderVm()
                {
                    ProviderId = (int)item.Id!,
                    Name = name!,
                    Ingredients = 0,
                    Foods = 0,
                });
            }
        }

        var output = new IndexFoodVm()
        {
            ListFoodProviders = providers.OrderBy(c => c.ProviderId).ToList()
        };
        return View(output);
    }

    // GET: FoodProvider/Details/5
    [Route("Admin/FoodProvider/Details")]
    public async Task<IActionResult> Details(int? id)
    {
        var ings = _mapper.Map<List<IngredientVm>>(
            await _context.Ingredients
            .Where(c => c.ProviderId == id)
            .ToListAsync());

        var foods = _mapper.Map<List<FoodVm>>(
            await _context.Foods
            .Include(c => c.Category)
            .Include(c => c.Offer)
            .Where(c => c.IsHomeMade == false && c.ProviderId == id)
            .ToListAsync());

        var pro = await _context.Providers
                        .Where(i => i.Id == id)
                        .FirstOrDefaultAsync();

        var name = pro!.Name;

        var output = new IndexFoodProviderVm()
        {
            Name = name,
            ListIngredients = ings,
            ListFoods = foods
        };
        return View(output);
    }
}
