using WebAppUI.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppUI.Models.ViewModels;
namespace WebAppUI.Controllers;
public class CategoryController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    public CategoryController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [Route("Categories")]
    public async Task<IActionResult> Index()
    {
        var categories = await _context.Categories
            .OrderBy(i => i.Name)
            .ToListAsync();
        return View(_mapper.Map<List<CardCategoryVm>>(categories));
    }

    [Route("Category/Details/{id}")]
    public async Task<IActionResult> Details(int id)
    {
        var Category = await _context.Categories
            .FirstOrDefaultAsync(i => i.Id == id);
        if (Category == null)
            return NotFound();
        var Foods = await _context.Foods
            .Include(t => t.Offer)
            .Include(t => t.Category)
            .Where(i => i.CategoryId == id && i.IsFinal)
            .OrderBy(i => i.Title)
            .ToListAsync();
        var output = new CategoryFoodsVm()
        {
            CategoryDetails = _mapper.Map<CardCategoryVm>(Category),
            Foods = _mapper.Map<List<CardFoodVm>>(Foods)
        };
        return View(output);
    }
}