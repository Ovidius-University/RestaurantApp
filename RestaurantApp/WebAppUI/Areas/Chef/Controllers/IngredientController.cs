using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebAppUI.Areas.Chef.Models.DTOs;
using WebAppUI.Areas.Chef.Models.ViewModels;
using WebAppUI.Models.Entities;
namespace WebAppUI.Areas.Chef.Controllers;

[Area("Chef")]
[Authorize(Roles = "Chef")]
public class IngredientController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public IngredientController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    // GET: Chef/Ingredient
    [Route("Chef/Ingredients")]
    public async Task<IActionResult> Index()
    {
        if (_context.Ingredients == null)
            return Problem("Entity set 'ApplicationDbContext.Ingredients' is null.");
        var Ingredients = await _context.Ingredients.ToListAsync();
        //List<ShortIngredientVm> lista = new();
        //foreach(var ingredient in Ingredients)
        //{
        //    lista.Add(new ShortIngredientVm(ingredient.Id, ingredient.Name));
        //}
        //return View(lista);
        return  View(_mapper.Map<List<ShortIngredientVm>>(Ingredients));
    }
    /*
    // GET: Chef/Ingredient/Details/5
    [Route("Chef/Ingredient/Details/{id}")]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || _context.Ingredients == null)
        {
            return NotFound();
        }

        var ingredient = await _context.Ingredients
            .FirstOrDefaultAsync(m => m.Id == id);
        if (ingredient == null)
        {
            return NotFound();
        }

        return View(_mapper.Map<IngredientDetailsVm>(ingredient));
    }
    // GET: Chef/Ingredient/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Chef/Ingredient/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([FromForm]NewIngredientDto NewIngredient)
    {
        if (ModelState.IsValid)
        {
            //Ingredient ingredient = new()
            //{
            //    Name = NewIngredient.Name,
            //};
            //_context.Add(ingredient);
            var Ingredient = _mapper.Map<Ingredient>(NewIngredient);
            _context.Add(Ingredient);
            await _context.SaveChangesAsync();
            TempData["MessageIngredient"] = $"The ingredient <strong>{NewIngredient.Name}</strong> was successfully added!";
            return RedirectToAction(nameof(Index));
        }
        return View(NewIngredient);
    }

    // GET: Chef/Ingredient/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        if (_context.Ingredients == null)
        {
            return NotFound();
        }

        var existentIngredient = _mapper.Map<ExistentIngredientDto>(await _context.Ingredients.FindAsync(id));
        if (existentIngredient == null)
        {
            return NotFound();
        }
        return View(existentIngredient);
    }

    // POST: Chef/Ingredient/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [FromForm]ExistentIngredientDto existentIngredient)
    {
        var ingredient = await _context.Ingredients.FindAsync(id);
        if (id != existentIngredient.IngredientId || id!=ingredient!.Id || ingredient is null)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                TempData["MessageIngredient"] = $"The ingredient <strong>{ingredient.Name}</strong> was modified to <strong>{existentIngredient.Name}</strong> successfully!";
                existentIngredient.ToEntity(ref ingredient);
                _context.Update(ingredient);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IngredientExists(existentIngredient.IngredientId))
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
        return View(existentIngredient);
    }

    // GET: Chef/Ingredient/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null || _context.Ingredients == null)
        {
            return NotFound();
        }

        var ingredient = await _context.Ingredients
            .FirstOrDefaultAsync(m => m.Id == id);
        if (ingredient == null)
        {
            return NotFound();
        }

        var IngredientInFoods = await _context.IngredientsFood.Where(q => q.IngredientId == id).CountAsync();

        if (IngredientInFoods > 0)
        {
            TempData["MessageIngredient"] = $"<strong>{ingredient.Name}</strong> cannot be deleted as they are a ingredient for food items of the restaurant, to be more exact: {IngredientInFoods}!";
            return RedirectToAction(nameof(Index));
        }
        
        return View(_mapper.Map<ShortIngredientVm>(ingredient));
    }

    // POST: Chef/Ingredient/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (_context.Ingredients == null)
        {
            return Problem("Entity set 'ApplicationDbContext.Ingredients' is null.");
        }
        var existentIngredient = await _context.Ingredients.FindAsync(id);
        if (existentIngredient is null)
            return NotFound();
        var IngredientInFoods = await _context.IngredientsFood.Where(q=>q.IngredientId == id).CountAsync();
        if (IngredientInFoods==0)
        {
            _context.Ingredients.Remove(existentIngredient);
            TempData["MessageIngredient"] = $"The ingredient <strong>{existentIngredient.Name}</strong> was deleted successfully!";
            await _context.SaveChangesAsync();
        }
        else
        {
            TempData["MessageIngredient"] = $"<strong>{existentIngredient.Name}</strong> was not deleted as they are a ingredient for food items of the restaurant, to be more exact: {IngredientInFoods}!";
        }
        return RedirectToAction(nameof(Index));
    }
    */
    // GET: Chef/Ingredient/AddFood/5
    public async Task<IActionResult> AddFood(int id)
    {
        if (_context.Ingredients == null)
        {
            return NotFound();
        }

        var existentIngredient = _mapper.Map<IngredientAddFoodDto>(await _context.Ingredients.FindAsync(id));
        if (existentIngredient == null)
        {
            return NotFound();
        }
        var empSched = await _context.EmployeeSchedules
                        .Include(i => i.Employee)
                        .Include(i => i.Day)
                        .Where(i => i.Employee!.UserName == User.Identity!.Name)
                        .FirstOrDefaultAsync();
        if (empSched == null)
            return Problem("You haven't been assigned a work schedule yet!");

        var foods = _mapper.Map<List<FoodDto>>(
                    await _context.Foods
                    .Include(t => t.Ingredients)
                    .Where(q => q.ChefId == empSched.EmployeeId && !q.IsFinal && q.IsHomeMade)
                    .ToListAsync());
        ViewBag.AvailableFoods = new SelectList(foods, "FoodId", "Title");
        return View(existentIngredient);
    }


    // POST: Chef/Ingredient/AddFood/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddFood(int id, [FromForm] IngredientAddFoodDto IngredientAddFood)
    {
        var existentIngredient = await _context.Ingredients.FindAsync(id);
        var ExistentFood = await _context.Foods.FindAsync(IngredientAddFood.FoodId);
        if(existentIngredient is null || ExistentFood is null)
            return NotFound();
        if (id != IngredientAddFood.IngredientId || id != existentIngredient!.Id)
        {
            return BadRequest();
        }

        if (ModelState.IsValid)
        {
            try
            {
                var existentIngredientFood = await _context.IngredientsFood.FirstOrDefaultAsync(q => q.IngredientId == IngredientAddFood.IngredientId && q.FoodId == IngredientAddFood.FoodId);
                if(existentIngredientFood is null)
                {
                    IngredientFood IngredientFood = new()
                    {
                        IngredientId = IngredientAddFood.IngredientId,
                        FoodId = IngredientAddFood.FoodId
                    };
                    await _context.IngredientsFood.AddAsync(IngredientFood);
                    await _context.SaveChangesAsync();
                    TempData["MessageIngredient"] = $"The ingredient <strong>{existentIngredient.Name}</strong> was successfully associated with food item <strong>{ExistentFood.Title}</strong>!";
                }
                else
                {
                    TempData["MessageIngredient"] = $"The ingredient <strong>{existentIngredient.Name}</strong> was already associated with food item <strong>{ExistentFood.Title}</strong>!";
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IngredientFoodExists(IngredientAddFood.IngredientId, IngredientAddFood.FoodId))
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
        return View(IngredientAddFood);
    }

    // GET: Chef/Ingredient/RemoveFood/5
    public async Task<IActionResult> RemoveFood(int id)
    {
        if (_context.Ingredients == null)
        {
            return NotFound();
        }

        var existentIngredient = _mapper.Map<IngredientAddFoodDto>(await _context.Ingredients.FindAsync(id));
        if (existentIngredient == null)
        {
            return NotFound();
        }
        var empSched = await _context.EmployeeSchedules
                        .Include(i => i.Employee)
                        .Include(i => i.Day)
                        .Where(i => i.Employee!.UserName == User.Identity!.Name)
                        .FirstOrDefaultAsync();
        if (empSched == null)
            return Problem("You haven't been assigned a work schedule yet!");

        var foods = _mapper.Map<List<FoodDto>>(
                    await _context.Foods
                    .Include(t => t.Ingredients)
                    .Where(q => q.ChefId == empSched.EmployeeId && !q.IsFinal && q.IsHomeMade).ToListAsync());
        ViewBag.AcquiredFoods = new SelectList(foods, "FoodId", "Title");
        return View(existentIngredient);
    }


    // POST: Chef/Ingredient/RemoveFood/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveFood(int id, [FromForm] IngredientAddFoodDto IngredientRemoveFood)
    {
        var existentIngredient = await _context.Ingredients.FindAsync(id);
        var ExistentFood = await _context.Foods.FindAsync(IngredientRemoveFood.FoodId);
        if (existentIngredient is null || ExistentFood is null)
            return NotFound();
        if (id != IngredientRemoveFood.IngredientId || id != existentIngredient!.Id)
        {
            return BadRequest();
        }

        if (ModelState.IsValid)
        {
            try
            {
                var existentIngredientFood = await _context.IngredientsFood.FirstOrDefaultAsync(q => q.IngredientId == IngredientRemoveFood.IngredientId && q.FoodId == IngredientRemoveFood.FoodId);
                if (existentIngredientFood is not null)
                {
                    _context.IngredientsFood.Remove(existentIngredientFood);
                    await _context.SaveChangesAsync();
                    TempData["MessageIngredient"] = $"The ingredient <strong>{existentIngredient.Name}</strong> was successfully disassociated with food item <strong>{ExistentFood.Title}</strong>!";
                }
                else
                {
                    TempData["MessageIngredient"] = $"The ingredient <strong>{existentIngredient.Name}</strong> was already not associated with food item <strong>{ExistentFood.Title}</strong>!";
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IngredientFoodExists(IngredientRemoveFood.IngredientId, IngredientRemoveFood.FoodId))
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
        return View(IngredientRemoveFood);
    }

    private bool IngredientExists(int IngredientId)
    {
        return (_context.Ingredients?.Any(e => e.Id == IngredientId)).GetValueOrDefault();
    }

    private bool IngredientFoodExists(int IngredientId, int FoodId)
    {
      return (_context.IngredientsFood?.Any(e => e.IngredientId == IngredientId && e.FoodId == FoodId)).GetValueOrDefault();
    }
}
