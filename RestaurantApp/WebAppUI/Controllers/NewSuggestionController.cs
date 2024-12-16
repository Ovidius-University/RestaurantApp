using WebAppUI.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppUI.Models.ViewModels;
namespace WebAppUI.Controllers;
public class NewSuggestionController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    public NewSuggestionController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [Route("NewSuggestions")]
    public async Task<IActionResult> Index()
    {
        var CurrentCustomer = await _context.Users.FirstOrDefaultAsync(q => q.UserName == User.Identity!.Name);
        if (CurrentCustomer == null || CurrentCustomer.UserName == null)
            return Redirect("Identity/Account/Login");

        var OwnedOrders = await _context.Orders
            .Include(c => c.Customer)
            .Include(c => c.PayingMethod)
            .Include(c => c.OrderContents)
            .Where(q => q.CustomerId == CurrentCustomer.Id && q.IsFinal)
            .ToListAsync();
        
        var OwnedOrderContents = await _context.OrderContents
            .Include(c => c.Food)
            .ToListAsync();

        OwnedOrderContents = OwnedOrderContents
            .Where(q => OwnedOrders.Any(j => j.Id == q.OrderId))
            .ToList();

        var OwnedFoods = await _context.Foods
            .Include(t => t.Ingredients!)
            .ThenInclude(t => t.Ingredient)
            .Include(t => t.Category)
            .Include(t => t.Offer)
            .OrderBy(i => i.Title)
            .ToListAsync();

        OwnedFoods = OwnedFoods
            .Where(i => OwnedOrderContents.Any(j => j.FoodId == i.Id))
            .OrderBy(i => i.Title)
            .ToList();

        var Suggestions = await _context.Foods
            .Include(t => t.Ingredients!)
            .ThenInclude(t => t.Ingredient)
            .Include(t => t.Category)
            .Include(t => t.Offer)
            .OrderBy(i => i.Title)
            .ToListAsync();

        Suggestions = Suggestions
            .Where(i => !OwnedFoods.Any(j => j.Id == i.Id) && i.IsFinal)
            .OrderBy(i => i.Title)
            .ToList();
        return View(_mapper.Map<List<CardFoodVm>>(Suggestions));
    }
    /*
    [Route("Food/Details/{id}")]
    public async Task<IActionResult> Details(int id)
    {
        var Food = _mapper.Map<FoodDetailsVm>(await _context.Foods
            .Include(t => t.Offer)
            .Include(t => t.Category)
            .Where(i => i.IsFinal)
            .FirstOrDefaultAsync(i => i.Id == id));
        if (Food == null)
            return NotFound();
        var Ingredients = _mapper.Map<List<CardIngredientVm>>(await _context.Ingredients
            .Where(i => i.Foods!.Any(i => i.FoodId == id))
            .ToListAsync());
        Food.ListIngredients = Ingredients;
        return View(Food);
    }

    [Route("Offers")]
    public async Task<IActionResult> Offers()
    {
        var Foods = await _context.Foods
            .Include(t => t.Ingredients!)
            .ThenInclude(t => t.Ingredient)
            .Include(t => t.Offer)
            .Where(i => i.IsFinal && i.Offer != null)
            .OrderBy(i => i.Title)
            .ToListAsync();
        return View(_mapper.Map<List<CardFoodVm>>(Foods));
    }
    */
}