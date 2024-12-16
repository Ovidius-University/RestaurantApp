using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebAppUI.Areas.Manager.Models.DTOs;
using WebAppUI.Areas.Manager.Models.ViewModels;
using WebAppUI.Models.CustomIdentity;
using WebAppUI.Models.Entities;
namespace WebAppUI.Areas.Manager.Controllers;

[Area("Manager")]
[Authorize(Roles = "Manager")]
public class IngredientController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly UserManager<AppUser> _userManager;

    public IngredientController(ApplicationDbContext context, IMapper mapper, UserManager<AppUser> userManager)
    {
        _context = context;
        _mapper = mapper;
        _userManager = userManager;
    }

    // GET: Manager/Ingredient
    [Route("Manager/Ingredients")]
    public async Task<IActionResult> Index()
    {
        if (_context.Ingredients == null)
            return Problem("Entity set 'ApplicationDbContext.Ingredients' is null.");
        var CurrentProvider = await _context.ProviderManagers
                .Include(t => t.Provider)
                .Include(t => t.Manager)
                .FirstOrDefaultAsync(q => q.Manager!.UserName == User.Identity!.Name);
        if (CurrentProvider == null)
            return Problem("You haven't been assigned to a provider yet!");
        var Ingredients = await _context.Ingredients
                            .Where(i => i.ProviderId == CurrentProvider.ProviderId)
                            .ToListAsync();
        //List<ShortIngredientVm> lista = new();
        //foreach(var ingredient in Ingredients)
        //{
        //    lista.Add(new ShortIngredientVm(ingredient.Id, ingredient.Name));
        //}
        //return View(lista);
        return  View(_mapper.Map<List<ShortIngredientVm>>(Ingredients));
    }

    // GET: Manager/Ingredient/Details/5
    [Route("Manager/Ingredient/Details/{id}")]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || _context.Ingredients == null)
        {
            return NotFound();
        }

        var ingredient = await _context.Ingredients
            .Include(m => m.Provider)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (ingredient == null)
        {
            return NotFound();
        }

        return View(_mapper.Map<IngredientDetailsVm>(ingredient));
    }

    // GET: Manager/Ingredient/Create
    public async Task<IActionResult> Create()
    {
        var CurrentManager = await _context.ProviderManagers
            .Include(t => t.Provider)
            .Include(t => t.Manager)
            .FirstOrDefaultAsync(q => q.Manager!.UserName == User.Identity!.Name);

        if (CurrentManager == null)
            return NotFound();//redirection

        return View();
    }

    // POST: Manager/Ingredient/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([FromForm]NewIngredientDto NewIngredient)
    {
        var CurrentUser = await _userManager.FindByEmailAsync(User.Identity!.Name!);
        var CurrentProvider = await _context.ProviderManagers.Where(q => q.ManagerId == CurrentUser!.Id).FirstOrDefaultAsync();
        if (CurrentProvider == null)
            return BadRequest();

        if (ModelState.IsValid)
        {
            var history = await _context.Ingredients
            .Where(i => i.Name == NewIngredient.Name && i.ProviderId == CurrentProvider!.ProviderId)
            .FirstOrDefaultAsync();
            if (history != null)
            {
                TempData["MessageIngredient"] = $"The ingredient <strong>{NewIngredient.Name}</strong> is already added!";
                return RedirectToAction(nameof(Index));
            }

            var Ingredient = _mapper.Map<Ingredient>(NewIngredient);
            Ingredient.ProviderId = CurrentProvider!.ProviderId;
            _context.Add(Ingredient);
            await _context.SaveChangesAsync();
            TempData["MessageIngredient"] = $"The ingredient <strong>{NewIngredient.Name}</strong> was successfully added!";
            return RedirectToAction(nameof(Index));
        }
        return View(NewIngredient);
    }

    // GET: Manager/Ingredient/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        if (_context.Ingredients == null)
        {
            return NotFound();
        }

        var CurrentProvider = await _context.ProviderManagers
            .Include(t => t.Manager)
            .Where(q => q.Manager!.UserName == User!.Identity!.Name).FirstOrDefaultAsync();
        if (CurrentProvider is null)
            return BadRequest();

        var existentIngredient = _mapper.Map<ExistentIngredientDto>(await _context.Ingredients.FindAsync(id));
        if (existentIngredient == null)
        {
            return NotFound();
        }
        return View(existentIngredient);
    }

    // POST: Manager/Ingredient/Edit/5
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

        var CurrentProvider = await _context.ProviderManagers
            .Include(t => t.Manager)
            .Where(q => q.Manager!.UserName == User!.Identity!.Name).FirstOrDefaultAsync();
        if (CurrentProvider is null)
            return BadRequest();

        var history = await _context.Ingredients
            .Where(i => i.Name == existentIngredient.Name && i.Id != id && i.ProviderId == CurrentProvider!.ProviderId)
            .FirstOrDefaultAsync();
        if (history != null)
        {
            TempData["MessageIngredient"] = $"The ingredient <strong>{existentIngredient.Name}</strong> already exists!";
            return RedirectToAction(nameof(Index));
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

    // GET: Manager/Ingredient/Delete/5
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
            TempData["MessageIngredient"] = $"<strong>{ingredient.Name}</strong> cannot be deleted as they are an ingredient for food items of the restaurant, to be more exact: {IngredientInFoods}!";
            return RedirectToAction(nameof(Index));
        }
        
        return View(_mapper.Map<ShortIngredientVm>(ingredient));
    }

    // POST: Manager/Ingredient/Delete/5
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
        if (IngredientInFoods<=0)
        {
            _context.Ingredients.Remove(existentIngredient);
            TempData["MessageIngredient"] = $"The ingredient <strong>{existentIngredient.Name}</strong> was deleted successfully!";
            await _context.SaveChangesAsync();
        }
        else
        {
            TempData["MessageIngredient"] = $"<strong>{existentIngredient.Name}</strong> cannot be deleted as they are an ingredient for food items of the restaurant, to be more exact: {IngredientInFoods}!";
        }
        return RedirectToAction(nameof(Index));
    }

    // GET: Chef/Ingredient/AddFood/5
    public async Task<IActionResult> AddFood(int id)
    {
        if (_context.Ingredients == null)
        {
            return NotFound();
        }
        var CurrentUser = await _userManager.FindByEmailAsync(User.Identity!.Name!);
        var CurrentProvider = await _context.ProviderManagers.Where(q => q.ManagerId == CurrentUser!.Id).FirstOrDefaultAsync();
        if (CurrentProvider == null)
            return BadRequest();

        var existentIngredient = _mapper.Map<IngredientAddFoodDto>(await _context.Ingredients.FindAsync(id));
        if (existentIngredient == null)
        {
            return NotFound();
        }
        var foods = _mapper.Map<List<FoodDto>>(
                    await _context.Foods
                    .Where(q => q.ProviderId == CurrentProvider.ProviderId && !q.IsFinal && !q.IsHomeMade)
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
        if (existentIngredient is null || ExistentFood is null)
            return NotFound();
        if (id != IngredientAddFood.IngredientId || id != existentIngredient!.Id)
        {
            return BadRequest();
        }

        var CurrentUser = await _userManager.FindByEmailAsync(User.Identity!.Name!);
        var CurrentProvider = await _context.ProviderManagers.Where(q => q.ManagerId == CurrentUser!.Id).FirstOrDefaultAsync();
        if (CurrentProvider == null)
            return BadRequest();

        if (ModelState.IsValid)
        {
            try
            {
                var existentIngredientFood = await _context.IngredientsFood.FirstOrDefaultAsync(q => q.IngredientId == IngredientAddFood.IngredientId && q.FoodId == IngredientAddFood.FoodId);
                if (existentIngredientFood is null)
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
        var CurrentUser = await _userManager.FindByEmailAsync(User.Identity!.Name!);
        var CurrentProvider = await _context.ProviderManagers.Where(q => q.ManagerId == CurrentUser!.Id).FirstOrDefaultAsync();
        if (CurrentProvider == null)
            return BadRequest();

        var existentIngredient = _mapper.Map<IngredientAddFoodDto>(await _context.Ingredients.FindAsync(id));
        if (existentIngredient == null)
        {
            return NotFound();
        }
        var foods = _mapper.Map<List<FoodDto>>(
                    await _context.Foods
                    .Where(q => q.ProviderId == CurrentProvider.ProviderId && !q.IsFinal && !q.IsHomeMade).ToListAsync());
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
