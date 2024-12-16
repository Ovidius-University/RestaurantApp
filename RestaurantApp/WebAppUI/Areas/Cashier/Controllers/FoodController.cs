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
using WebAppUI.Areas.Cashier.Models.DTOs;
using WebAppUI.Areas.Cashier.Models.ViewModels;
using WebAppUI.Data;
using WebAppUI.Models.CustomIdentity;
namespace WebAppUI.Areas.Cashier.Controllers;

[Area("Cashier")]
[Authorize(Roles = "Cashier")]
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

    // GET: Cashier/Food
    [Route("Cashier/Foods")]
    public async Task<IActionResult> Index()
    {
        var foods = _mapper.Map<List<FoodVm>>(
            await _context.Foods
            .Include(t => t.Offer)
            .Include(t => t.Category)
            .Where(t => t.IsFinal == true)
            .OrderBy(t => t.Title)
            .ToListAsync());
        var output = new IndexFoodsVm()
        {
            ListFoods = foods
        };
        return View(output);
    }

    // GET: Cashier/Food/Details/5
    [Route("Cashier/Food/Details/{id}")]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || _context.Foods == null)
        {
            return NotFound();
        }

        var food = await _context.Foods
            .Include(t => t.Offer)
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
            ingredients += $"{ingredient.Ingredient!.Name}, ";
        }
        output.Ingredients = string.IsNullOrEmpty(ingredients) ? "-" : ingredients.Substring(0, ingredients.Length - 2);
        output.Category = food.Category!.Name;
        return View(output);
    }

    // GET: Cashier/Food/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || _context.Foods == null)
        {
            return NotFound();
        }
        var ExistentFood = await _context.Foods
            .Include(t => t.Offer)
            .Include(c => c.Category)
            .FirstOrDefaultAsync(q => q.Id == id);
        if (ExistentFood == null)
        {
            return NotFound();
        }
        var ingredients = await _context.IngredientsFood
            .Include(t => t.Ingredient)
            .Where(c => c.FoodId == id)
            .OrderBy(o => o.Ingredient!.Name)
            .ToListAsync();
        var output = new FoodEditDto()
        {
            ExistentFood = _mapper.Map<ExistentFoodDto>(ExistentFood),
            ListIngredients = _mapper.Map<List<ShortIngredientVm>>(ingredients)
        };
        return View(output);
    }

    // POST: Cashier/Food/Edit/5
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

        if (ModelState.IsValid)
        {
            try
            {
                var ExistentFood = await _context.Foods
                    .FirstOrDefaultAsync(q => q.Id == id);
                if (ExistentFood == null)
                {
                    return NotFound();
                }
                ModifiedFood.ExistentFood.ToEntity(ref ExistentFood);
                _context.Update(ExistentFood);
                await _context.SaveChangesAsync();

                if (ExistentFood!.Stock == 0)
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
        return View(ModifiedFood);
    }

    private bool FoodExists(int id)
    {
        return (_context.Foods?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}
