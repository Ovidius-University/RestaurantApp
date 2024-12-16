using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebAppUI.Areas.Manager.Models.DTOs;
using WebAppUI.Areas.Manager.Models.ViewModels;
using WebAppUI.Data;
using WebAppUI.Models.Entities;

namespace WebAppUI.Areas.Manager.Controllers
{
    [Area("Manager")]
    [Authorize(Roles = "Manager")]
    public class OfferController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public OfferController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: Manager/Offer
        [Route("Manager/Offers")]
        public async Task<IActionResult> Index()
        {
            var CurrentProvider = await _context.ProviderManagers
                        .Include(t => t.Manager)
                        .Where(q => q.Manager!.UserName == User!.Identity!.Name).FirstOrDefaultAsync();
            if (CurrentProvider == null)
                return Problem("You haven't been assigned to a provider yet!");
            var offers = _context.Offers
                .Include(t => t.Food)
                .Where(c => c.Food!.ProviderId == CurrentProvider.ProviderId && c.Food!.IsHomeMade == false);
            return View(_mapper.Map<List<FoodOfferVm>>(await offers.ToListAsync()));
        }
        

        // GET: Manager/Offer/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Offers == null)
            {
                return NotFound();
            }
            var CurrentProvider = await _context.ProviderManagers
                        .Include(t => t.Manager)
                        .Where(q => q.Manager!.UserName == User!.Identity!.Name).FirstOrDefaultAsync();
            if (CurrentProvider is null)
                return BadRequest();

            var ExistentOffer = await _context.Offers
                .Include(t => t.Food)
                .FirstOrDefaultAsync(q => q.Id == id && q.Food!.ProviderId == CurrentProvider.ProviderId && q.Food!.IsHomeMade == false);
            if (ExistentOffer == null)
            {
                return NotFound();
            }
            var output = _mapper.Map<FoodOfferDto>(ExistentOffer);

            var ListIngredients = await _context.IngredientsFood
                .Include(t => t.Ingredient).Where(c => c.FoodId == id)
                .OrderBy(o => o.Ingredient!.Name)
                .ToListAsync();
            var ingredients = "";
            foreach (var ingredient in ListIngredients)
            {
                ingredients += $"{ingredient.Ingredient!.Name}, ";
            }
            output.Ingredients = string.IsNullOrEmpty(ingredients) ? "-" : ingredients.Substring(0, ingredients.Length - 2);

            return View(output);
        }

        // GET: Manager/Offer/Create
        public IActionResult Create()
        {
            ViewData["Id"] = new SelectList(_context.Foods, "Id", "Id");
            return View();
        }

        // POST: Manager/Offer/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PromoText,NewPrice")] FoodOffer foodOffer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(foodOffer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Id"] = new SelectList(_context.Foods, "Id", "Id", foodOffer.Id);
            return View(foodOffer);
        }

        // GET: Manager/Offer/Edit/5
        [Route("Manager/Offer/Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Offers == null)
            {
                return NotFound();
            }

            var foodOffer = await _context.Offers.FindAsync(id);
            if (foodOffer == null)
            {
                return NotFound();
            }
            ViewData["Id"] = new SelectList(_context.Foods, "Id", "Id", foodOffer.Id);
            return View(foodOffer);
        }

        // POST: Manager/Offer/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PromoText,NewPrice")] FoodOffer foodOffer)
        {
            if (id != foodOffer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(foodOffer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FoodOfferExists(foodOffer.Id))
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
            ViewData["Id"] = new SelectList(_context.Foods, "Id", "Id", foodOffer.Id);
            return View(foodOffer);
        }

        // GET: Manager/Offer/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Offers == null)
            {
                return NotFound();
            }

            var CurrentProvider = await _context.ProviderManagers
                        .Include(t => t.Manager)
                        .Where(q => q.Manager!.UserName == User!.Identity!.Name).FirstOrDefaultAsync();
            if (CurrentProvider is null)
                return BadRequest();

            var foodOffer = await _context.Offers
                .Include(c => c.Food)
                .FirstOrDefaultAsync(m => m.Id == id && m.Food!.ProviderId == CurrentProvider.ProviderId && m.Food!.IsHomeMade == false);
            if (foodOffer == null)
            {
                return NotFound();
            }

            return View(foodOffer);
        }

        // POST: Manager/Offer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Offers == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Offers' is null.");
            }
            var foodOffer = await _context.Offers.FindAsync(id);
            if (foodOffer != null)
            {
                _context.Offers.Remove(foodOffer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FoodOfferExists(int id)
        {
            return (_context.Offers?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public async Task<ActionResult> GetOffer([FromRoute] int id)
        {
            var food = await _context.Foods
                .Include(c => c.Provider)
                .FirstOrDefaultAsync(m => m.Id == id && m.IsFinal);
            if (food == null)
            {
                return Json(new { success = false, errors = new string[] { "Food item does not exist!" } });
            }
            var ListIngredients = await _context.IngredientsFood
                .Include(t => t.Ingredient).Where(c => c.FoodId == id)
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
            if (Offer is null)
            {
                //return Json(new { success = false, errors = new string[] { "Offer does not exist!" } });
            }
            return Json(new { success = true, item = output });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateOffer([FromRoute] int id, [FromForm] AddEditFoodOfferDto IntroducedOffer)
        {
            var ExistentOffer = await _context.Offers.FindAsync(id);
            var ExistentFood = await _context.Foods
                .FirstOrDefaultAsync(i => i.Id == id && i.IsFinal);
            if (ExistentFood is null)
            {
                return Json(new { success = false, errors = new string[] { "Food item does not exist!" } });
            }

            if (IntroducedOffer.Id != ExistentFood.Id)
            {
                return Json(new { success = false, errors = new string[] { "Bad Request!" } });
            }
            if (ModelState.IsValid)
            {
                if (IntroducedOffer.NewPrice >= ExistentFood.Price)
                {
                    return Json(new { success = false, priceError = true, item=_mapper.Map<FoodPriceVm>(ExistentFood), errors = new string[] { "New price needs to be smaller than the old price!" } });
                }
                if (ExistentOffer is null)
                {
                    // insert new offer
                    var NewOffer = _mapper.Map<FoodOffer>(IntroducedOffer);
                    Console.WriteLine($"New offer: {IntroducedOffer}, {NewOffer}");

                    //NewOffer.Id = id;
                    await _context.AddAsync(NewOffer);
                }
                else
                {
                    // modify existent offer
                    ExistentOffer.PromoText = IntroducedOffer.PromoText;
                    ExistentOffer.NewPrice = IntroducedOffer.NewPrice;
                    _context.Update(ExistentOffer);
                }
                await _context.SaveChangesAsync();
                return Json(new { success = true, item = IntroducedOffer });
            }
            var errors = ModelState.SelectMany(x => x.Value!.Errors.Select(e => e.ErrorMessage));
            return Json(new { success = false, errors });
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteOffer([FromRoute] int id, int FoodId)
        {
            var ExistentOffer = await _context.Offers.FindAsync(id);
            if (ExistentOffer is null)
            {
                return Json(new { success = false, errors = new string[] { "Offer does not exist" } });
            }
            if (id != FoodId)
            {
                return Json(new { success = false, errors = new string[] { "Bad Request!" } });
            }
            _context.Offers.Remove(ExistentOffer);
            await _context.SaveChangesAsync();
            return Json(new { success = true, item = ExistentOffer });
        }
    }
}
