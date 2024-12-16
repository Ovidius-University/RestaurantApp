using WebAppUI.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppUI.Models.ViewModels;
namespace WebAppUI.Controllers;
public class FoodController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    public FoodController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [Route("Foods")]
    public async Task<IActionResult> Index()
    {
        var Foods = await _context.Foods
            .Include(t => t.Ingredients!)
            .ThenInclude(t => t.Ingredient)
            .Include(t => t.Allergens!)
            .ThenInclude(t => t.Allergen)
            .Include(t => t.Category)
            .Include(t => t.Offer)
            .Where(i => i.IsFinal)
            .OrderBy(i => i.Title)
            .ToListAsync();
        return View(_mapper.Map<List<CardFoodVm>>(Foods));
    }

    [Route("Food/Details/{id}")]
    public async Task<IActionResult> Details(int id)
    {
        var Food = _mapper.Map<FoodDetailsVm>(await _context.Foods
                   .Where(i => i.IsFinal)
                   .FirstOrDefaultAsync(i => i.Id == id));
        if (Food == null)
            return NotFound();

        if (Food.HomeMade.Equals("Yes"))
            Food = _mapper.Map<FoodDetailsVm>(await _context.Foods
                   .Include(t => t.Offer)
                   .Include(t => t.Category)
                   .Include(t => t.Chef)
                   .Where(i => i.IsFinal)
                   .FirstOrDefaultAsync(i => i.Id == id));
        else Food = _mapper.Map<FoodDetailsVm>(await _context.Foods
                   .Include(t => t.Offer)
                   .Include(t => t.Category)
                   .Include(t => t.Provider)
                   .Where(i => i.IsFinal)
                   .FirstOrDefaultAsync(i => i.Id == id));
        if (Food == null)
            return NotFound();

        var Ingredients = _mapper.Map<List<CardIngredientVm>>(await _context.Ingredients
                          .Include(t => t.Provider)
                          .Where(i => i.Foods!.Any(i => i.FoodId == id))
                          .ToListAsync());
        Food.ListIngredients = Ingredients;

        var Allergens = _mapper.Map<List<CardAllergenVm>>(await _context.Allergens
                        .Where(i => i.Foods!.Any(i => i.FoodId == id))
                        .ToListAsync());
        Food.ListAllergens = Allergens;
        return View(Food);
    }

    [Route("Offers")]
    public async Task<IActionResult> Offers()
    {
        var Foods = await _context.Foods
            .Include(t => t.Ingredients!)
            .ThenInclude(t => t.Ingredient)
            .Include(t => t.Allergens!)
            .ThenInclude(t => t.Allergen)
            .Include(t => t.Offer)
            .Include(t => t.Category)
            .Where(i => i.IsFinal && i.Offer != null)
            .OrderBy(i => i.Title)
            .ToListAsync();
        return View(_mapper.Map<List<CardFoodVm>>(Foods));
    }
}