using WebAppUI.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppUI.Models.ViewModels;
using WebAppUI.Areas.Admin.Models.ViewModels;
namespace WebAppUI.Controllers;
public class ReviewController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    public ReviewController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [Route("Reviews")]
    public async Task<IActionResult> Items()
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

        var Reviews = await _context.Reviews
            .Include(t => t.Reviewer!)
            .Include(t => t.Food!)
            .Where(i => i.IsFinal && i.Food!.IsFinal)
            .OrderBy(i => i.FoodId)
            .ToListAsync();

        var foodItems = new List<Food>(Foods);

        foreach (var item in Foods)
        {
            var r = Reviews.Where(c => c.FoodId == item.Id).FirstOrDefault();
            if (r == null)
            {
                foodItems.Remove(item);
            }
        }
        return View(_mapper.Map<List<CardFoodVm>>(foodItems));
    }

    [Route("Review/Index/{id}")]
    public async Task<IActionResult> Index(int? id)
    {
        var Reviews = await _context.Reviews
            .Include(t => t.Reviewer!)
            .Include(t => t.Food!)
            .Where(i => i.IsFinal && i.Food!.IsFinal && i.FoodId == id)
            .OrderBy(i => i.FoodId)
            .ToListAsync();

        var food = await _context.Foods
            .Where(i => i.Id == id)
            .FirstOrDefaultAsync();

        var output = new IndexFoodReviewsVm()
        {
            Food = food!.Title,
            ListReviews = _mapper.Map<List<CardReviewVm>>(Reviews)
        };

        return View(output);
    }

    [ActionName("Details")]
    public async Task<IActionResult> Details(int? fid, int? rid)
    {
        var Review = _mapper.Map<ReviewDetailsVm>(await _context.Reviews
            .Include(i => i.Food)
            .Include(i => i.Reviewer)
            .Where(i => i.IsFinal)
            .FirstOrDefaultAsync(i => i.FoodId == fid && i.ReviewerId == rid));
        if (Review == null)
            return NotFound();
        return View(Review);
    }
}