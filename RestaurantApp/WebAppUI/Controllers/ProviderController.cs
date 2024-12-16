using WebAppUI.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppUI.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using WebAppUI.Areas.Admin.Models.DTOs;
using WebAppUI.Models.CustomIdentity;
namespace WebAppUI.Controllers;
public class ProviderController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    public readonly SignInManager<AppUser> _signInManager;
    public readonly UserManager<AppUser> _userManager;
    public ProviderController(ApplicationDbContext context, IMapper mapper, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
    {
        _context = context;
        _mapper = mapper;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [Route("Providers")]
    public async Task<IActionResult> Index()
    {
        var providers = await _context.Providers.ToListAsync();
        return View(_mapper.Map<List<CardProviderVm>>(providers));
    }

    [Route("Provider/Details/{id}")]
    public async Task<IActionResult> Details(int id)
    {
        var Provider = await _context.Providers.FirstOrDefaultAsync(i => i.Id == id);
        if (Provider == null)
            return NotFound();
        var Foods = await _context.Foods
            .Include(t => t.Provider)
            .Include(t => t.Offer)
            .Where(i => i.ProviderId! == id && i.IsFinal && i.IsHomeMade==false)
            .OrderBy(i => i.Title)
            .ToListAsync();
        var Ingredients = await _context.Ingredients
            .Include(t => t.Provider)
            .Where(i => i.ProviderId! == id)
            .OrderBy(i => i.Name)
            .ToListAsync();
        var output = new ProviderFoodsVm()
        {
            ProviderDetails = _mapper.Map<CardProviderVm>(Provider),
            Foods = _mapper.Map<List<CardFoodVm>>(Foods),
            Ingredients = _mapper.Map<List<CardIngredientVm>>(Ingredients)
        };
        return View(output);
    }
}