using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppUI.Models.ViewModels;
namespace WebAppUI.Controllers;
public class IngredientController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    public IngredientController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [Route("Ingredients")]
    public async Task<IActionResult> Index()
    {
        var Ingredients = await _context.Ingredients
            .OrderBy(i => i.Name)
            .ToListAsync();
        return View(_mapper.Map<List<CardIngredientVm>>(Ingredients));
    }

    [Route("Ingredient/Details/{id}")]
    public async Task<IActionResult> Details(int id)
    {
        var Ingredient = await _context.Ingredients
            .Include(t => t.Provider)
            .FirstOrDefaultAsync(i => i.Id == id);
        if (Ingredient == null)
            return NotFound();
        var Foods = await _context.Foods
            .Include(t => t.Offer)
            .Include(t => t.Category)
            .Where(i => i.Ingredients!.Any(i => i.IngredientId == id) && i.IsFinal)
            .OrderBy(i => i.Title)
            .ToListAsync();
        var output = new IngredientFoodsVm()
        {
            IngredientDetails = _mapper.Map<CardIngredientVm>(Ingredient),
            Foods = _mapper.Map<List<CardFoodVm>>(Foods)
        };
        return View(output);
    }
}