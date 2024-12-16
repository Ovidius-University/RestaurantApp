using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebAppUI.Areas.Chef.Models.DTOs;
using WebAppUI.Areas.Chef.Models.ViewModels;
using WebAppUI.Models.CustomIdentity;
using WebAppUI.Models.Entities;
namespace WebAppUI.Areas.Chef.Controllers;

[Area("Chef")]
[Authorize(Roles = "Chef")]
public class AllergenController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly UserManager<AppUser> _userManager;

    public AllergenController(ApplicationDbContext context, IMapper mapper, UserManager<AppUser> userManager)
    {
        _context = context;
        _mapper = mapper;
        _userManager = userManager;
    }

    // GET: Chef/Allergen
    [Route("Chef/Allergens")]
    public async Task<IActionResult> Index()
    {
        if (_context.Allergens == null)
            return Problem("Entity set 'ApplicationDbContext.Allergens' is null.");
        var Allergens = await _context.Allergens.ToListAsync();
        //List<ShortAllergenVm> lista = new();
        //foreach(var allergen in Allergens)
        //{
        //    lista.Add(new ShortAllergenVm(allergen.Id, allergen.Name));
        //}
        //return View(lista);
        return  View(_mapper.Map<List<ShortAllergenVm>>(Allergens));
    }

    // GET: Chef/Allergen/Details/5
    [Route("Chef/Allergen/Details/{id}")]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || _context.Allergens == null)
        {
            return NotFound();
        }

        var allergen = await _context.Allergens
                       .FirstOrDefaultAsync(m => m.Id == id);
        if (allergen == null)
        {
            return NotFound();
        }

        return View(_mapper.Map<AllergenDetailsVm>(allergen));
    }

    // GET: Chef/Allergen/Create
    public async Task<IActionResult> Create()
    {
        var CurrentUser = await _userManager.FindByEmailAsync(User.Identity!.Name!);
        if (CurrentUser == null)
            return Problem("You are not logged in!");

        return View();
    }

    // POST: Chef/Allergen/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([FromForm]NewAllergenDto NewAllergen)
    {
        var CurrentUser = await _userManager.FindByEmailAsync(User.Identity!.Name!);
        
        if (ModelState.IsValid)
        {
            var history = await _context.Allergens
            .Where(i => i.Name == NewAllergen.Name)
            .FirstOrDefaultAsync();
            if (history != null)
            {
                TempData["MessageAllergen"] = $"The allergen <strong>{NewAllergen.Name}</strong> is already added!";
                return RedirectToAction(nameof(Index));
            }

            var Allergen = _mapper.Map<Allergen>(NewAllergen);
            _context.Add(Allergen);
            await _context.SaveChangesAsync();
            TempData["MessageAllergen"] = $"The allergen <strong>{NewAllergen.Name}</strong> was successfully added!";
            return RedirectToAction(nameof(Index));
        }
        return View(NewAllergen);
    }

    // GET: Chef/Allergen/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        if (_context.Allergens == null)
        {
            return NotFound();
        }

        var existentAllergen = _mapper.Map<ExistentAllergenDto>(await _context.Allergens.FindAsync(id));
        if (existentAllergen == null)
        {
            return NotFound();
        }
        return View(existentAllergen);
    }

    // POST: Chef/Allergen/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [FromForm]ExistentAllergenDto existentAllergen)
    {
        var allergen = await _context.Allergens.FindAsync(id);
        if (id != existentAllergen.AllergenId || id!=allergen!.Id || allergen is null)
        {
            return NotFound();
        }

        var history = await _context.Allergens
            .Where(i => i.Name == existentAllergen.Name && i.Id != id)
            .FirstOrDefaultAsync();
        if (history != null)
        {
            TempData["MessageAllergen"] = $"The allergen <strong>{existentAllergen.Name}</strong> already exists!";
            return RedirectToAction(nameof(Index));
        }

        if (ModelState.IsValid)
        {
            try
            {
                TempData["MessageAllergen"] = $"The allergen <strong>{allergen.Name}</strong> was modified to <strong>{existentAllergen.Name}</strong> successfully!";
                existentAllergen.ToEntity(ref allergen);
                _context.Update(allergen);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AllergenExists(existentAllergen.AllergenId))
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
        return View(existentAllergen);
    }

    // GET: Chef/Allergen/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null || _context.Allergens == null)
        {
            return NotFound();
        }

        var allergen = await _context.Allergens
            .FirstOrDefaultAsync(m => m.Id == id);
        if (allergen == null)
        {
            return NotFound();
        }

        var AllergenInFoods = await _context.AllergensFood.Where(q => q.AllergenId == id).CountAsync();

        if (AllergenInFoods > 0)
        {
            TempData["MessageAllergen"] = $"<strong>{allergen.Name}</strong> cannot be deleted as they are an allergen for food items of the restaurant, to be more exact: {AllergenInFoods}!";
            return RedirectToAction(nameof(Index));
        }
        
        return View(_mapper.Map<ShortAllergenVm>(allergen));
    }

    // POST: Chef/Allergen/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (_context.Allergens == null)
        {
            return Problem("Entity set 'ApplicationDbContext.Allergens' is null.");
        }
        var existentAllergen = await _context.Allergens.FindAsync(id);
        if (existentAllergen is null)
            return NotFound();
        var AllergenInFoods = await _context.AllergensFood.Where(q=>q.AllergenId == id).CountAsync();
        if (AllergenInFoods<=0)
        {
            _context.Allergens.Remove(existentAllergen);
            TempData["MessageAllergen"] = $"The allergen <strong>{existentAllergen.Name}</strong> was deleted successfully!";
            await _context.SaveChangesAsync();
        }
        else
        {
            TempData["MessageAllergen"] = $"<strong>{existentAllergen.Name}</strong> cannot be deleted as they are an allergen for food items of the restaurant, to be more exact: {AllergenInFoods}!";
        }
        return RedirectToAction(nameof(Index));
    }

    // GET: Chef/Allergen/AddFood/5
    public async Task<IActionResult> AddFood(int id)
    {
        if (_context.Allergens == null)
        {
            return NotFound();
        }
        var CurrentUser = await _userManager.FindByEmailAsync(User.Identity!.Name!);
        if (CurrentUser == null)
            return Problem("You are not logged in!");

        var empSched = await _context.EmployeeSchedules
                        .Include(i => i.Employee)
                        .Include(i => i.Day)
                        .Where(i => i.Employee!.UserName == User.Identity!.Name)
                        .FirstOrDefaultAsync();
        if (empSched == null)
            return Problem("You haven't been assigned a work schedule yet!");

        var existentAllergen = _mapper.Map<AllergenAddFoodDto>(await _context.Allergens.FindAsync(id));
        if (existentAllergen == null)
        {
            return NotFound();
        }
        var foods = _mapper.Map<List<FoodDto>>(
                    await _context.Foods
                    .Where(q => q.ChefId == empSched.EmployeeId && !q.IsFinal && q.IsHomeMade)
                    .ToListAsync());
        ViewBag.AvailableFoods = new SelectList(foods, "FoodId", "Title");
        return View(existentAllergen);
    }


    // POST: Chef/Allergen/AddFood/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddFood(int id, [FromForm] AllergenAddFoodDto AllergenAddFood)
    {
        var existentAllergen = await _context.Allergens.FindAsync(id);
        var ExistentFood = await _context.Foods.FindAsync(AllergenAddFood.FoodId);
        if (existentAllergen is null || ExistentFood is null)
            return NotFound();
        if (id != AllergenAddFood.AllergenId || id != existentAllergen!.Id)
        {
            return BadRequest();
        }

        var CurrentUser = await _userManager.FindByEmailAsync(User.Identity!.Name!);
        if (ModelState.IsValid)
        {
            try
            {
                var existentAllergenFood = await _context.AllergensFood.FirstOrDefaultAsync(q => q.AllergenId == AllergenAddFood.AllergenId && q.FoodId == AllergenAddFood.FoodId);
                if (existentAllergenFood is null)
                {
                    AllergenFood AllergenFood = new()
                    {
                        AllergenId = AllergenAddFood.AllergenId,
                        FoodId = AllergenAddFood.FoodId
                    };
                    await _context.AllergensFood.AddAsync(AllergenFood);
                    await _context.SaveChangesAsync();
                    TempData["MessageAllergen"] = $"The allergen <strong>{existentAllergen.Name}</strong> was successfully associated with food item <strong>{ExistentFood.Title}</strong>!";
                }
                else
                {
                    TempData["MessageAllergen"] = $"The allergen <strong>{existentAllergen.Name}</strong> was already associated with food item <strong>{ExistentFood.Title}</strong>!";
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AllergenFoodExists(AllergenAddFood.AllergenId, AllergenAddFood.FoodId))
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
        return View(AllergenAddFood);
    }

    // GET: Chef/Allergen/RemoveFood/5
    public async Task<IActionResult> RemoveFood(int id)
    {
        if (_context.Allergens == null)
        {
            return NotFound();
        }
        var CurrentUser = await _userManager.FindByEmailAsync(User.Identity!.Name!);
        if (CurrentUser == null)
            return Problem("You are not logged in!");

        var empSched = await _context.EmployeeSchedules
                        .Include(i => i.Employee)
                        .Include(i => i.Day)
                        .Where(i => i.Employee!.UserName == User.Identity!.Name)
                        .FirstOrDefaultAsync();
        if (empSched == null)
            return Problem("You haven't been assigned a work schedule yet!");

        var existentAllergen = _mapper.Map<AllergenAddFoodDto>(await _context.Allergens.FindAsync(id));
        if (existentAllergen == null)
        {
            return NotFound();
        }
        var foods = _mapper.Map<List<FoodDto>>(
                    await _context.Foods
                    .Where(q => q.ChefId == empSched.EmployeeId && !q.IsFinal && q.IsHomeMade).ToListAsync());
        ViewBag.AcquiredFoods = new SelectList(foods, "FoodId", "Title");
        return View(existentAllergen);
    }

    // POST: Chef/Allergen/RemoveFood/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveFood(int id, [FromForm] AllergenAddFoodDto AllergenRemoveFood)
    {
        var existentAllergen = await _context.Allergens.FindAsync(id);
        var ExistentFood = await _context.Foods.FindAsync(AllergenRemoveFood.FoodId);
        if (existentAllergen is null || ExistentFood is null)
            return NotFound();
        if (id != AllergenRemoveFood.AllergenId || id != existentAllergen!.Id)
        {
            return BadRequest();
        }

        if (ModelState.IsValid)
        {
            try
            {
                var existentAllergenFood = await _context.AllergensFood.FirstOrDefaultAsync(q => q.AllergenId == AllergenRemoveFood.AllergenId && q.FoodId == AllergenRemoveFood.FoodId);
                if (existentAllergenFood is not null)
                {
                    _context.AllergensFood.Remove(existentAllergenFood);
                    await _context.SaveChangesAsync();
                    TempData["MessageAllergen"] = $"The allergen <strong>{existentAllergen.Name}</strong> was successfully disassociated with food item <strong>{ExistentFood.Title}</strong>!";
                }
                else
                {
                    TempData["MessageAllergen"] = $"The allergen <strong>{existentAllergen.Name}</strong> was already not associated with food item <strong>{ExistentFood.Title}</strong>!";
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AllergenFoodExists(AllergenRemoveFood.AllergenId, AllergenRemoveFood.FoodId))
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
        return View(AllergenRemoveFood);
    }

    private bool AllergenExists(int AllergenId)
    {
        return (_context.Allergens?.Any(e => e.Id == AllergenId)).GetValueOrDefault();
    }

    private bool AllergenFoodExists(int AllergenId, int FoodId)
    {
      return (_context.AllergensFood?.Any(e => e.AllergenId == AllergenId && e.FoodId == FoodId)).GetValueOrDefault();
    }
}
