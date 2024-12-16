using WebAppUI.Models.CustomIdentity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using WebAppUI.Areas.Manager.Models.DTOs;
using WebAppUI.Areas.Manager.Models.ViewModels;
namespace WebAppUI.Areas.Manager.Controllers;

[Area("Manager")]
[Authorize(Roles = "Manager")]
public class CategoryController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly UserManager<AppUser> _userManager;

    public CategoryController(ApplicationDbContext context, IMapper mapper, UserManager<AppUser> userManager)
    {
        _context = context;
        _mapper = mapper;
        _userManager = userManager;
    }

    // GET: Manager/Category
    [Route("Manager/Categories")]
    public async Task<IActionResult> Index()
    {
        if (_context.Categories == null)
            return Problem("Entity set 'ApplicationDbContext.Categories' is null.");
        return View(_mapper.Map<List<ExistentCategoryDto>>(await _context.Categories.ToListAsync()));
    }

    // GET: Manager/Category/Details/5
    [Route("Manager/Category/Details/{id}")]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || _context.Categories == null)
        {
            return NotFound();
        }

        var ExistentCategory = _mapper.Map<ExistentCategoryDto>(await _context.Categories.FirstOrDefaultAsync(m => m.Id == id));
        if (ExistentCategory == null)
        {
            return NotFound();
        }

        return View(ExistentCategory);
    }

    // GET: Manager/Category/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Manager/Category/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([FromForm] NewCategoryDto newCategory)
    {
        if (ModelState.IsValid)
        {
            var history = await _context.Categories
                .Where(i => i.Name == newCategory.Name)
                .FirstOrDefaultAsync();
            if (history != null)
            {
                TempData["MessageCategory"] = $"The category <strong>{newCategory.Name}</strong> is already added!";
                return RedirectToAction(nameof(Index));
            }
            var category = _mapper.Map<Category>(newCategory);
            _context.Add(category);
            await _context.SaveChangesAsync();
            TempData["MessageCategory"] = $"We added category <strong>{newCategory.Name}</strong>!";
            return RedirectToAction(nameof(Index));
        }
        return View(newCategory);
    }

    // GET: Manager/Category/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || _context.Categories == null)
        {
            return NotFound();
        }

        var ExistentCategory = _mapper.Map<ExistentCategoryDto>(await _context.Categories.FindAsync(id));
        if (ExistentCategory == null)
        {
            return NotFound();
        }
        return View(ExistentCategory);
    }

    // POST: Manager/Category/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [FromForm] ExistentCategoryDto modifiedCategory)
    {
        if (id != modifiedCategory.Id)
        {
            return NotFound();
        }
        var ExistentCategory = await _context.Categories.FindAsync(id);
        if (ExistentCategory == null)
        {
            return NotFound();
        }
        if (ExistentCategory.Id != modifiedCategory.Id)
        {
            return BadRequest();
        }

        var history = await _context.Categories
                .Where(i => i.Name == modifiedCategory.Name && i.Id!=id)
                .FirstOrDefaultAsync();
        if (history != null)
        {
            TempData["MessageCategory"] = $"The category <strong>{modifiedCategory.Name}</strong> already exists!";
            return RedirectToAction(nameof(Index));
        }
        if (ModelState.IsValid)
        {
            try
            {
                TempData["MessageCategory"] = $"We modified category <strong>{ExistentCategory.Name}</strong> in <strong>{modifiedCategory.Name}</strong>!";
                ExistentCategory.Name = modifiedCategory.Name;
                _context.Update(ExistentCategory);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(ExistentCategory.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(modifiedCategory);
    }

    // GET: Manager/Category/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null || _context.Categories == null)
        {
            return NotFound();
        }

        var ExistentCategory = _mapper.Map<ExistentCategoryDto>(await _context.Categories.FirstOrDefaultAsync(m => m.Id == id));
        if (ExistentCategory == null)
        {
            return NotFound();
        }

        var FoodCategory = await _context.Categories
            .Include(t => t.Foods)
            .FirstOrDefaultAsync(i => i.Id == id);
        if (FoodCategory == null)
        {
            return NotFound();
        }

        if (FoodCategory.Foods!.Count > 0)
        {
            TempData["MessageCategory"] = $"Category <strong>{ExistentCategory.Name}</strong> can not be deleted as it has associated food items, {FoodCategory.Foods!.Count} to be more exact!";
            return RedirectToAction(nameof(Index));
        }   

        return View(ExistentCategory);
    }

    // POST: Manager/Category/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (_context.Categories == null)
        {
            return Problem("Entity set 'ApplicationDbContext.Categories' is null.");
        }
        var ExistentCategory = await _context.Categories
            .Include(t => t.Foods)
            .FirstOrDefaultAsync(i => i.Id == id);
        if (ExistentCategory == null)
        {
            return NotFound();
        }
        else
        {
            if (ExistentCategory.Foods!.Count == 0)
            {
                _context.Categories.Remove(ExistentCategory);
                TempData["MessageCategory"] = $"Category <strong>{ExistentCategory.Name}</strong> was successfully deleted!";
            }
            else
            {
                TempData["MessageCategory"] = $"Category <strong>{ExistentCategory.Name}</strong> can not be deleted as it has associated food items!";
            }
        }
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool CategoryExists(int id)
    {
        return (_context.Categories?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}