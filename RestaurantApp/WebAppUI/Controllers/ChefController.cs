using WebAppUI.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppUI.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using WebAppUI.Areas.Admin.Models.DTOs;
using WebAppUI.Models.CustomIdentity;
namespace WebAppUI.Controllers;
public class ChefController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    public readonly SignInManager<AppUser> _signInManager;
    public readonly UserManager<AppUser> _userManager;
    public ChefController(ApplicationDbContext context, IMapper mapper, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
    {
        _context = context;
        _mapper = mapper;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [Route("Chefs")]
    public async Task<IActionResult> Index()
    {
        var chefs = _mapper.Map<List<ExistentUserDto>>(
            await _userManager.GetUsersInRoleAsync("Chef"));
        return View(_mapper.Map<List<CardChefVm>>(chefs));
    }

    [Route("Chef/Details/{id}")]
    public async Task<IActionResult> Details(int id)
    {
        var Chef = _mapper.Map<List<ExistentUserDto>>(
            await _userManager.GetUsersInRoleAsync("Chef")).FirstOrDefault(i => i.Id == id);
        if (Chef == null)
            return NotFound();
        var Foods = await _context.Foods
            .Include(t => t.Chef)
            .Include(t => t.Offer)
            .Where(i => i.ChefId! == id && i.IsFinal && i.IsHomeMade == true)
            .OrderBy(i => i.Title)
            .ToListAsync();
        var output = new ChefFoodsVm()
        {
            ChefDetails = _mapper.Map<CardChefVm>(Chef),
            Foods = _mapper.Map<List<CardFoodVm>>(Foods)
        };
        return View(output);
    }
}