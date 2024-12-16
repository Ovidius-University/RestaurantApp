using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WebAppUI.Areas.Manager.Models.DTOs;
using WebAppUI.Areas.Manager.Models.ViewModels;
using WebAppUI.Data;
using WebAppUI.Models.CustomIdentity;
namespace WebAppUI.Areas.Manager.Controllers;

[Area("Manager")]
[Authorize(Roles = "Manager")]
public class FoodController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly UserManager<AppUser> _userManager;

    public FoodController(ApplicationDbContext context, IMapper mapper,
        UserManager<AppUser> userManager)
    {
        _context = context;
        _mapper = mapper;
        _userManager = userManager;
    }

    // GET: Manager/Food
    [Route("Manager/Foods")]
    public async Task<IActionResult> Index()
    {
        var CurrentProvider = await _context.ProviderManagers
            .Include(t => t.Provider)
            .Include(t => t.Manager)
            .FirstOrDefaultAsync(q => q.Manager!.UserName == User.Identity!.Name);
        if (CurrentProvider == null)
            return Problem("You haven't been assigned to a provider yet!");
        var foods = _mapper.Map<List<FoodVm>>(
            await _context.Foods
            .Include(t => t.Category)
            .Where(q => q.ProviderId == CurrentProvider.ProviderId && q.IsHomeMade == false).ToListAsync());
        var output = new IndexFoodsVm()
        {
            Provider = CurrentProvider.Provider!.Name,
            ListFoods = foods
        };
        return View(output);
    }

    // GET: Manager/Food/Details/5
    [Route("Manager/Food/Details/{id}")]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || _context.Foods == null)
        {
            return NotFound();
        }

        var food = await _context.Foods
            .Include(c => c.Provider)
            .Include(c => c.Category)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (food == null)
        {
            return NotFound();
        }
        var ListIngredients = await _context.IngredientsFood
            .Include(t => t.Ingredient)
            .Where(c => c.FoodId == id)
            .OrderBy(o => o.Ingredient!.Name)
            .ToListAsync();

        var output = _mapper.Map<FoodDetailsVm>(food);
        var ingredients = "";
        foreach (var ingredient in ListIngredients)
        {
            ingredients += $"{ingredient.Ingredient!.Name}";
        }
        output.Ingredients = string.IsNullOrEmpty(ingredients) ? "-" : ingredients.Substring(0, ingredients.Length - 2);
        output.Category = food.Category!.Name;
        return View(output);
    }

    // GET: Manager/Food/Create
    public async Task<IActionResult> Create()
    {
        if (_context.Categories == null || _context.Foods == null)
        {
            return NotFound();
        }

        var CurrentManager = await _context.ProviderManagers
            .Include(t => t.Provider)
            .Include(t => t.Manager)
            .FirstOrDefaultAsync(q => q.Manager!.UserName == User.Identity!.Name);

        if (CurrentManager == null)
            return NotFound();//redirectionez
        var categories = _mapper.Map<List<CategoryDto>>(
            await _context.Categories
            .ToListAsync());
        if(categories == null)
        {
            return NotFound();
        }
        ViewBag.AvailableCategories = new SelectList(categories, "CategoryId", "Name");
        return View();
    }

    // POST: Manager/Food/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([FromForm] NewFoodDto newFood)
    {
        var CurrentUser = await _userManager.FindByEmailAsync(User.Identity!.Name!);
        var CurrentProvider = await _context.ProviderManagers.Where(q => q.ManagerId == CurrentUser!.Id).FirstOrDefaultAsync();
        if (CurrentProvider == null)
            return BadRequest();
        var CurrentCategory = await _context.Categories
            .Where(q => q.Id == newFood!.CategoryId).FirstOrDefaultAsync();
        if (CurrentCategory == null)
            return BadRequest();
        if (ModelState.IsValid)
        {
            var history = await _context.Foods
            .Where(i => i.Title == newFood.Title)
            .FirstOrDefaultAsync();
            if (history != null)
            {
                TempData["MessageFood"] = $"The food item <strong>{newFood.Title}</strong> is already added!";
                return RedirectToAction(nameof(Index));
            }
            var NewFood = _mapper.Map<Food>(newFood);
            NewFood.ProviderId = CurrentProvider!.ProviderId;
            _context.Add(NewFood);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        var categories = _mapper.Map<List<CategoryDto>>(
            await _context.Categories
            .ToListAsync());
        if (categories == null)
        {
            return NotFound();
        }
        ViewBag.AvailableCategories = new SelectList(categories, "CategoryId", "Name");
        return View(newFood);
    }

    // GET: Manager/Food/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || _context.Foods == null)
        {
            return NotFound();
        }
        var CurrentProvider = await _context.ProviderManagers
            .Include(t => t.Manager)
            .Where(q => q.Manager!.UserName == User!.Identity!.Name).FirstOrDefaultAsync();
        if (CurrentProvider is null)
            return BadRequest();
        var ExistentFood = await _context.Foods
            .Include(c => c.Category)
            .FirstOrDefaultAsync(q => q.Id == id && q.ProviderId == CurrentProvider.ProviderId && q.IsHomeMade == false);
        if (ExistentFood == null)
        {
            return NotFound();
        }
        var ingredients = await _context.IngredientsFood
            .Include(t => t.Ingredient).Where(c => c.FoodId == id)
            .OrderBy(o => o.Ingredient!.Name)
            .ToListAsync();
        var output = new FoodEditDto()
        {
            ExistentFood = _mapper.Map<ExistentFoodDto>(ExistentFood),
            ListIngredients = _mapper.Map<List<ShortIngredientVm>>(ingredients)
        };
        var categories = _mapper.Map<List<CategoryDto>>(
            await _context.Categories
            .ToListAsync());
        if (categories == null)
        {
            return NotFound();
        }
        ViewBag.ExistentCategories = new SelectList(categories, "CategoryId", "Name", ExistentFood.CategoryId);
        return View(output);
    }

    // POST: Manager/Food/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit([FromRoute] int id, [FromForm] FoodEditDto ModifiedFood)
    {
        if (ModifiedFood.ExistentFood is null || id != ModifiedFood.ExistentFood!.Id)
        {
            return NotFound();
        }

        var ExistentFoodOffer = await _context.Offers
                    .FirstOrDefaultAsync(q => q.Id == id);

        if (ExistentFoodOffer is not null && ModifiedFood.ExistentFood.IsFinal == false)
        {
            TempData["MessageFood"] = $"Offer for <strong>{ModifiedFood.ExistentFood.Title}</strong> needs to be removed before food item can be made not final!";
            return RedirectToAction(nameof(Index));
        }

        if (ExistentFoodOffer is not null && ExistentFoodOffer.NewPrice >= ModifiedFood.ExistentFood.Price)
        {
            TempData["MessageFood"] = $"Food item <strong>{ModifiedFood.ExistentFood.Title}</strong> price cannot be equal or lower than the price of its offer!";
            return RedirectToAction(nameof(Index));
        }

        var history = await _context.Foods
            .Where(i => i.Title == ModifiedFood.ExistentFood.Title && i.Id!=id)
            .FirstOrDefaultAsync();
        if (history != null)
        {
            TempData["MessageFood"] = $"The food item <strong>{ModifiedFood.ExistentFood.Title}</strong> already exists!";
            return RedirectToAction(nameof(Index));
        }

        if (ModelState.IsValid)
        {
            try
            {
                var CurrentProvider = await _context.ProviderManagers
                    .Include(t => t.Manager)
                    .Where(q => q.Manager!.UserName == User!.Identity!.Name).FirstOrDefaultAsync();
                if (CurrentProvider is null)
                    return BadRequest();
                var ExistentFood = await _context.Foods
                    .FirstOrDefaultAsync(q => q.Id == id && q.ProviderId == CurrentProvider.ProviderId && q.IsHomeMade == false);
                if (ExistentFood == null)
                {
                    return NotFound();
                }
                ModifiedFood.ExistentFood.ToEntity(ref ExistentFood);
                _context.Update(ExistentFood);
                await _context.SaveChangesAsync();

                if (ExistentFood!.IsFinal == false || ExistentFood!.Stock == 0)
                {
                    var unavailableshopcarts = await _context.ShopCarts
                    .Include(c => c.Food)
                    .Include(c => c.Food!.Offer)
                    .Where(q => q.FoodId == id).ToListAsync();

                    foreach (var item in unavailableshopcarts)
                    {
                        _context.ShopCarts.Remove(item);
                        await _context.SaveChangesAsync();
                    }
                }

                var shopcarts = await _context.ShopCarts
                    .Include(c => c.Food)
                    .Include(c => c.Food!.Offer)
                    .Where(q => q.FoodId == id && q.Food!.Stock < q.Quantity)
                    .ToListAsync();
                foreach (var item in shopcarts)
                {
                    item.Quantity = ExistentFood.Stock;
                    _context.ShopCarts.Update(item);
                    await _context.SaveChangesAsync();
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FoodExists(ModifiedFood.ExistentFood.Id))
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
        var ingredients = await _context.IngredientsFood
            .Include(t => t.Ingredient)
            .Where(c => c.FoodId == id)
            .ToListAsync();
        ModifiedFood.ListIngredients = _mapper.Map<List<ShortIngredientVm>>(ingredients);
        var categories = _mapper.Map<List<CategoryDto>>(
            await _context.Categories
            .ToListAsync());
        if (categories == null)
        {
            return NotFound();
        }
        ViewBag.ExistentCategories = new SelectList(categories, "CategoryId", "Name", ModifiedFood.ExistentFood.CategoryId);
        return View(ModifiedFood);
    }

    // GET: Manager/Food/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null || _context.Foods == null)
        {
            return NotFound();
        }

        var food = await _context.Foods
            .Include(c => c.Provider)
            .Include(t => t.Category)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (food == null)
        {
            return NotFound();
        }

        if (food != null && food.IsFinal is true)
        {
            TempData["MessageFood"] = $"The food item <strong>{food.Title}</strong> cannot be deleted if it is in the final stage!";
            return RedirectToAction(nameof(Index));
        }

        var ExistentFoodOffer = await _context.Offers
                    .FirstOrDefaultAsync(q => q.Id == id);

        if (ExistentFoodOffer is not null)
        {
            TempData["MessageFood"] = $"The offer for <strong>{food!.Title}</strong> needs to be removed before the food item can be deleted!";
            return RedirectToAction(nameof(Index));
        }

        var orderContent = await _context.OrderContents
            .Include(c => c.Food)
            .Where(m => m.FoodId == id)
            .ToListAsync();

        var order = await _context.Orders
            .Include(c => c.OrderContents)
            .Where(m => m.IsFinal == false)
            .ToListAsync();

        orderContent = orderContent
        .Where(q => order.Any(j => j.Id == q.OrderId))
        .ToList();

        if (orderContent is not null && orderContent!.Count>0)
        {
            TempData["MessageFood"] = $"The <strong>{food!.Title}</strong> cannot be removed, as it is requested on an unfinished order!";
            return RedirectToAction(nameof(Index));
        }

        var ListIngredients = await _context.IngredientsFood
            .Include(t => t.Ingredient).Where(c => c.FoodId == id)
            .OrderBy(o => o.Ingredient!.Name)
            .ToListAsync();
        var output = _mapper.Map<FoodDetailsVm>(food);
        var ingredients = "";
        foreach (var ingredient in ListIngredients)
        {
            ingredients += $"{ingredient.Ingredient!.Name}, ";
        }
        output.Ingredients = string.IsNullOrEmpty(ingredients) ? "-" : ingredients.Substring(0, ingredients.Length - 2);
        return View(output);
    }

    // POST: Manager/Food/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (_context.Foods == null)
        {
            return Problem("Entity set 'ApplicationDbContext.Foods' is null.");
        }

        var ExistentFoodOffer = await _context.Offers
                    .FirstOrDefaultAsync(q => q.Id == id);

        if (ExistentFoodOffer is not null)
        {
            TempData["MessageFood"] = $"Its offer needs to be removed before the food item can be deleted!";
            return RedirectToAction(nameof(Index));
        }

        var food = await _context.Foods.FindAsync(id);
        if (food != null)
        {
            if(food.IsFinal is false)
                _context.Foods.Remove(food);
            else TempData["MessageFood"] = $"The food item <strong>{food.Title}</strong> cannot be deleted if it is in the final stage!";
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: Manager/Food/Offer/5
    public async Task<IActionResult> Offer(int? id)
    {
        if (id == null || _context.Foods == null)
        {
            return NotFound();
        }

        var food = await _context.Foods
            .Include(c => c.Provider)
            .FirstOrDefaultAsync(m => m.Id == id && m.IsFinal);
        if (food == null)
        {
            return RedirectToAction(nameof(Index));
        }
        var ListIngredients = await _context.IngredientsFood
            .Include(t => t.Ingredient)
            .Where(c => c.FoodId == id)
            .OrderBy(o => o.Ingredient!.Name)
            .ToListAsync();

        var output = _mapper.Map<FoodOfferDto>(food);
        var ingredients = "";
        foreach (var ingredient in ListIngredients)
        {
            ingredients += $"{ingredient.Ingredient!.Name}, ";
        }
        output.Ingredients = string.IsNullOrEmpty(ingredients) ? "-" : ingredients.Substring(0, ingredients.Length - 2);
        var Offer = await _context.Offers.FindAsync(id);
        if (Offer is not null)
        {
            output.PromoText = Offer.PromoText;
            output.NewPrice = Offer.NewPrice;
            output.IsNewOffer = false;
        }
        return View(output);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Offer([FromRoute] int? id, [FromForm] FoodOfferDto NewOffer)
    {
        if (id == null || _context.Foods == null)
        {
            return NotFound();
        }
        var CurrentProvider = await _context.ProviderManagers
                    .Include(t => t.Manager)
                    .Where(q => q.Manager!.UserName == User!.Identity!.Name).FirstOrDefaultAsync();
        if (CurrentProvider is null)
            return BadRequest();
        var ExistentFood = await _context.Foods
            .FirstOrDefaultAsync(m => m.Id == id && m.IsFinal && m.ProviderId == CurrentProvider.ProviderId && m.IsHomeMade == false);
        if (ExistentFood == null)
        {
            return RedirectToAction(nameof(Index));
        }
        if (ModelState.IsValid)
        {
            var ExistentOffer = await _context.Offers
                .Include(t => t.Food)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (ExistentOffer is null)
            {
                // add offer
                var Offer = _mapper.Map<FoodOffer>(NewOffer);
                await _context.AddAsync(Offer);
            }
            else
            {
                // modify offer
                ExistentOffer.NewPrice = NewOffer.NewPrice;
                ExistentOffer.PromoText = NewOffer.PromoText;
                _context.Update(ExistentOffer);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(NewOffer);
    }

    private bool FoodExists(int id)
    {
        return (_context.Foods?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}
