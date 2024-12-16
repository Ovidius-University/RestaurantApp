using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebAppUI.Data;
using WebAppUI.Models.Entities;
using WebAppUI.Models.CustomIdentity;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using WebAppUI.Models.ViewModels;
using WebAppUI.Models.DTOs;
using Microsoft.IdentityModel.Tokens;
using System.Drawing;

namespace WebAppUI.Controllers;

public class ShopCartController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly UserManager<AppUser> _userManager;
    static readonly Random rnd = new();

    public ShopCartController(ApplicationDbContext context, IMapper mapper, UserManager<AppUser> userManager)
    {
        _context = context;
        _mapper = mapper;
        _userManager = userManager;
    }

    [Route("ShopCarts")]
    public async Task<IActionResult> Index()
    {
        var CurrentCustomer = await _context.Users
            .FirstOrDefaultAsync(q => q.UserName == User.Identity!.Name);
        if (CurrentCustomer == null || CurrentCustomer.UserName == null)
            return Redirect("Identity/Account/Login");
        var shopcarts = _mapper.Map<List<ShopCartVm>>(
            await _context.ShopCarts
            .Include(c => c.Customer)
            .Include(c => c.Food)
            .Include(c => c.Food!.Offer)
            .Where(q => q.CustomerId == CurrentCustomer.Id)
            .OrderBy(c => c.Food!.Title)
            .ToListAsync());

        var OwnedShopcart = await _context.ShopCarts
            .Include(c => c.Customer)
            .Include(c => c.Food)
            .Include(c => c.Food!.Offer)
            .Where(q => q.CustomerId == CurrentCustomer.Id)
            .OrderBy(c => c.Food!.Title)
            .ToListAsync();
        var Foods = await _context.Foods
            .Include(t => t.Ingredients!)
            .ThenInclude(t => t.Ingredient)
            .Include(t => t.Category)
            .Include(t => t.Offer)
            .Where(q => q.IsFinal && q.Stock > 0)
            .OrderBy(i => i.Title)
            .ToListAsync();
        Foods = Foods
            .Where(i => !OwnedShopcart.Any(j => j.FoodId == i.Id))
            .OrderBy(i => i.Title)
            .ToList();
        int r = rnd.Next(Foods.Count);
        var suggestion = Foods[r];

        var output = new IndexShopCartsVm()
        {
            Customer = CurrentCustomer.UserName,
            CustomerId = CurrentCustomer.Id,
            ListShopCarts = shopcarts,
            Suggestion = _mapper.Map<CardFoodVm>(suggestion)
        };
        return View(output);
    }

    // GET: ShopCart/Details/5
    [Route("ShopCart/Details/{id}")]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || _context.ShopCarts == null)
        {
            return NotFound();
        }

        var CurrentCustomer = await _context.Users
            .FirstOrDefaultAsync(q => q.UserName == User.Identity!.Name);
        if (CurrentCustomer == null || CurrentCustomer.UserName == null)
            return Redirect("../Identity/Account/Login");

        var shopcart = await _context.ShopCarts
            .Include(c => c.Customer)
            .Include(c => c.Food)
            .Include(c => c.Food!.Offer)
            .FirstOrDefaultAsync(m => m.FoodId == id && m.CustomerId == CurrentCustomer.Id);
        if (shopcart == null)
        {
            return NotFound();
        }

        var output = _mapper.Map<ShopCartDetailsVm>(shopcart);
        output.Customer = string.IsNullOrEmpty(CurrentCustomer.UserName) ? "-" : CurrentCustomer.UserName;
        return View(output);
    }

    // GET: ShopCart/Create
    public async Task<IActionResult> Create()
    {
        if (_context.Users == null || _context.Foods == null)
        {
            return NotFound();
        }

        var CurrentCustomer = await _context.Users
            .FirstOrDefaultAsync(q => q.UserName == User.Identity!.Name);
        if (CurrentCustomer == null || CurrentCustomer.UserName == null)
            return Redirect("../Identity/Account/Login");

        var foods = _mapper.Map<List<Models.DTOs.FoodDto>>(
                    await _context.Foods
                    .Where(q => q.IsFinal && q.Stock>0)
                    .ToListAsync());
        ViewBag.UnaddedFoods = new SelectList(foods, "FoodId", "Title");

        return View();
    }

    // POST: ShopCart/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([FromForm] NewShopCartDto newShopCart)
    {
        var CurrentUser = await _userManager.FindByEmailAsync(User.Identity!.Name!);
        if (CurrentUser == null)
            return BadRequest();

        var food = await _context.Foods
                              .Where(q => q.Id == newShopCart.FoodId)
                              .FirstOrDefaultAsync();
        if (food == null)
            return BadRequest();

        if (ModelState.IsValid)
        {
            var resource = await _context.Foods
            .Where(q => q.Id == newShopCart!.FoodId && q.IsFinal)
            .FirstOrDefaultAsync();
            if (resource == null)
            {
                TempData["MessageShopCart"] = $"The food item is not available anymore!";
                return RedirectToAction(nameof(Index));
            }

            var history = await _context.ShopCarts
            .Where(q => q.CustomerId == CurrentUser!.Id && q.FoodId == newShopCart!.FoodId)
            .FirstOrDefaultAsync();
            if (history != null)
            {
                TempData["MessageShopCart"] = $"The food item is already in your shopping cart!";
                return RedirectToAction(nameof(Index));
            }

            if (resource.Stock < newShopCart.Quantity)
            {
                TempData["MessageShopCart"] = $"Your order ({newShopCart.Quantity}) is bigger than what we have in stock right now ({resource.Stock})!";
                return RedirectToAction(nameof(Index));
            }

            var NewShopCart = _mapper.Map<ShopCart>(newShopCart);
            NewShopCart.CustomerId = CurrentUser!.Id;
            _context.Add(NewShopCart);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        var foods = _mapper.Map<List<Models.DTOs.FoodDto>>(
            await _context.Foods
            .Where(q => q.IsFinal && q.Stock > 0).ToListAsync());
        ViewBag.UnaddedFoods = new SelectList(foods, "FoodId", "Title");
        return View();
    }

    // GET: ShopCart/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || _context.ShopCarts == null || _context.Users == null)
        {
            return NotFound();
        }
        var CurrentCustomer = await _context.Users
            .Where(q => q.UserName == User!.Identity!.Name)
            .FirstOrDefaultAsync();
        if (CurrentCustomer is null)
            return BadRequest();

        var ExistentShopCart = await _context.ShopCarts
            .Include(t => t.Food)
            .Include(t => t.Customer)
            .FirstOrDefaultAsync(q => q.FoodId == id && q.CustomerId == CurrentCustomer.Id);
        if (ExistentShopCart == null)
        {
            return NotFound();
        }
        var output = _mapper.Map<ExistentShopCartDto>(ExistentShopCart);
        return View(output);
    }

    // POST: ShopCart/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit([FromRoute] int id, [FromForm] ExistentShopCartDto ModifiedShopCart)
    {
        if (ModifiedShopCart is null || id != ModifiedShopCart.FoodId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                var CurrentCustomer = await _context.Users
                    .Where(q => q.UserName == User!.Identity!.Name)
                    .FirstOrDefaultAsync();
                if (CurrentCustomer is null)
                    return BadRequest();

                var resource = await _context.Foods
                    .Where(q => q.Id == id && q.IsFinal)
                    .FirstOrDefaultAsync();

                if (resource == null)
                {
                    TempData["MessageShopCart"] = $"The food item is not available anymore!";
                    return RedirectToAction(nameof(Index));
                }

                if (resource.Stock < ModifiedShopCart.Quantity)
                {
                    TempData["MessageShopCart"] = $"Your order ({ModifiedShopCart.Quantity}) is bigger than what we have in stock right now ({resource.Stock})!";
                    return RedirectToAction(nameof(Index));
                }

                var ExistentShopCart = await _context.ShopCarts
                    .FirstOrDefaultAsync(q => q.FoodId == id && q.CustomerId == CurrentCustomer.Id);
                if (ExistentShopCart == null)
                {
                    return NotFound();
                }
                TempData["MessageShopCart"] = $"Your order has been successfully modified!";
                ModifiedShopCart.ToEntity(ref ExistentShopCart);
                _context.Update(ExistentShopCart);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShopCartExists(ModifiedShopCart.CustomerId, ModifiedShopCart.FoodId))
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

        var CurrenCustomer = await _context.Users
            .Where(q => q.UserName == User!.Identity!.Name)
            .FirstOrDefaultAsync();
        if (CurrenCustomer is null)
            return BadRequest();

        var ExistenShopCart = await _context.ShopCarts
            .Include(t => t.Food)
            .Include(t => t.Customer)
            .FirstOrDefaultAsync(q => q.FoodId == id && q.CustomerId == CurrenCustomer.Id);
        if (ExistenShopCart == null)
        {
            return NotFound();
        }
        var output = _mapper.Map<ExistentShopCartDto>(ExistenShopCart);
        return View(output);
    }

    // GET: ShopCart/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null || _context.ShopCarts == null)
        {
            return NotFound();
        }
        var CurrentCustomer = await _context.Users
                    .Where(q => q.UserName == User!.Identity!.Name)
                    .FirstOrDefaultAsync();
        if (CurrentCustomer is null)
            return BadRequest();

        var shopcart = await _context.ShopCarts
            .Include(c => c.Customer)
            .Include(c => c.Food)
            .FirstOrDefaultAsync(m => m.FoodId == id && m.CustomerId == CurrentCustomer.Id);
        if (shopcart == null)
        {
            return NotFound();
        }

        var Customers = await _context.ShopCarts
            .Include(t => t.Customer)
            .Where(c => c.FoodId == id && c.CustomerId == CurrentCustomer.Id)
            .ToListAsync();
        var output = _mapper.Map<ShopCartDetailsVm>(shopcart);
        var customers = "";
        foreach (var customer in Customers)
        {
            customers += $"{customer.Customer!.UserName}";
        }
        output.Customer = string.IsNullOrEmpty(customers) ? "-" : customers.Substring(0, customers.Length);
        return View(output);
    }

    // POST: ShopCart/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var CurrentUser = await _userManager.FindByEmailAsync(User.Identity!.Name!);
        if (CurrentUser == null)
            return BadRequest();

        if (_context.ShopCarts == null)
        {
            return Problem("Entity set 'ApplicationDbContext.ShopCarts' is null.");
        }

        var shopcart = await _context.ShopCarts.FindAsync(CurrentUser.Id, id);
        if (shopcart != null)
        {
            _context.ShopCarts.Remove(shopcart);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: ShopCart/Order
    public async Task<IActionResult> Order()
    {
        var CurrentUser = await _userManager.FindByEmailAsync(User.Identity!.Name!);
        if (CurrentUser == null)
            return Problem("You haven't logged in, or haven't made an account yet!");

        if (_context.ShopCarts == null)
        {
            return Problem("Entity set 'ApplicationDbContext.ShopCarts' is null.");
        }
        if (_context.Foods == null)
        {
            return Problem("Entity set 'ApplicationDbContext.Foods' is null.");
        }

        var CurrentCustomer = await _context.Users
            .FirstOrDefaultAsync(q => q.UserName == User.Identity!.Name);
        if (CurrentCustomer == null || CurrentCustomer.UserName == null || CurrentCustomer.Email == null)
            return Problem("You haven't logged in, or haven't made an account yet!");

        var OwnedShopcart = await _context.ShopCarts
            .Include(c => c.Customer)
            .Include(c => c.Food)
            .Include(c => c.Food!.Offer)
            .Where(q => q.CustomerId == CurrentCustomer.Id)
            .OrderBy(c => c.Food!.Title)
            .ToListAsync();
        var Foods = await _context.Foods
            .Include(t => t.Ingredients!)
            .ThenInclude(t => t.Ingredient)
            .Include(t => t.Category)
            .Include(t => t.Offer)
            .Where(q => q.IsFinal && q.Stock > 0)
            .OrderBy(i => i.Title)
            .ToListAsync();
        Foods = Foods
            .Where(i => !OwnedShopcart.Any(j => j.FoodId == i.Id))
            .OrderBy(i => i.Title)
            .ToList();
        int r = rnd.Next(Foods.Count);
        var suggestion = Foods[r];

        var shopcarts = _mapper.Map<List<ShopCartVm>>(
            await _context.ShopCarts
            .Include(c => c.Customer)
            .Include(c => c.Food)
            .Include(c => c.Food!.Offer)
            .Where(q => q.CustomerId == CurrentUser.Id).ToListAsync());
        if (shopcarts == null || shopcarts.Count == 0)
        {
            TempData["MessageShopCart"] = $"There are no items in your shopping cart, {CurrentCustomer.UserName}!";
            return RedirectToAction(nameof(Index));
        }
        var newOrder = new NewOrderDto()
        {
            CustomerId = CurrentCustomer.Id,
            Email = CurrentCustomer.Email,
            ShopCart = shopcarts,
            Suggestion = _mapper.Map<CardFoodVm>(suggestion)
        };
        var methods = _mapper.Map<List<PayingMethodDto>>(
            await _context.PayingMethods
            .ToListAsync());
        if (methods == null)
        {
            return NotFound();
        }
        ViewBag.AvailablePayingMethods = new SelectList(methods, "PayingMethodId", "Name");
        return View(newOrder);
    }

    // POST: ShopCart/Order
    [HttpPost, ActionName("Order")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> OrderConfirmed(int id, [FromForm] NewOrderDto? newOrder)
    {
        var CurrentUser = await _userManager.FindByEmailAsync(User.Identity!.Name!);
        if (CurrentUser == null)
            return Problem("You haven't logged in, or haven't made an account yet!");

        if (_context.ShopCarts == null)
        {
            return Problem("Entity set 'ApplicationDbContext.ShopCarts' is null.");
        }
        if (_context.Foods == null)
        {
            return Problem("Entity set 'ApplicationDbContext.Foods' is null.");
        }
        var CurrentCustomer = await _context.Users
                    .FirstOrDefaultAsync(q => q.UserName == User.Identity!.Name);
        if (CurrentCustomer == null || CurrentCustomer.UserName == null)
            return Problem("You haven't logged in, or haven't made an account yet!");

        var tryshopcarts = _mapper.Map<List<ShopCartVm>>(
                    await _context.ShopCarts
                    .Include(c => c.Customer)
                    .Include(c => c.Food)
                    .Include(c => c.Food!.Offer)
                    .Where(q => q.CustomerId == id).ToListAsync());

        if (tryshopcarts.Count != newOrder!.ShopCart!.Count)
        {
            TempData["MessageShopCart"] = $"A food item was either removed from sale or sold out! We updated your shopping cart!";
            return RedirectToAction(nameof(Index));
        }
        for (int i = 0; i < newOrder.ShopCart!.Count; i++)
        {
            if (!tryshopcarts[i].CustomerId.Equals(newOrder.ShopCart[i].CustomerId) || !tryshopcarts[i].FoodId.Equals(newOrder.ShopCart[i].FoodId) || !tryshopcarts[i].UnitPrice.Equals(newOrder.ShopCart[i].UnitPrice))
            {  
                TempData["MessageShopCart"] = $"A food item or an offer was either modified or removed! We updated your shopping cart!";
                return RedirectToAction(nameof(Index));
            }
            if (!tryshopcarts[i].Quantity.Equals(newOrder.ShopCart[i].Quantity))
            {
                TempData["MessageShopCart"] = $"Stock for a food item has decreased below your wanted quantity! We updated your item with the maximum number! Check to see if you still want to make an order!";
                return RedirectToAction(nameof(Index));
            }
            if (!tryshopcarts[i].Food!.Id.Equals(newOrder.ShopCart[i].Food!.Id) || !tryshopcarts[i].Food!.Title.Equals(newOrder.ShopCart[i].Food!.Title))
            {
                TempData["MessageShopCart"] = $"A food item title has been updated! We updated your shopping cart! Check to see if it's what you wanted!";
                return RedirectToAction(nameof(Index));
            }
        }

        var shopcarts = _mapper.Map<List<ShopCartVm>>(
                    await _context.ShopCarts
                    .Include(c => c.Customer)
                    .Include(c => c.Food)
                    .Include(c => c.Food!.Offer)
                    .Where(q => q.CustomerId == id).ToListAsync());

        foreach (var item in shopcarts)
        {
            var nosale = await _context.Foods
                       .Where(i => i.Id == item.FoodId && i.IsFinal == false)
                       .FirstOrDefaultAsync();

            if (nosale != null)
            {
                TempData["MessageShopCart"] = $"One of the food items ({nosale.Title}) is not for sale at the moment anymore! We will remove it automatically from your shopping cart!";
                var shopcart = await _context.ShopCarts.FindAsync(CurrentUser.Id, nosale.Id);
                if (shopcart != null)
                {
                    _context.ShopCarts.Remove(shopcart);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var lowstock = await _context.Foods
                       .Where(i => i.Id == item.FoodId && i.Stock < item.Quantity)
                       .FirstOrDefaultAsync();

            if (lowstock != null)
            {
                if(lowstock.Stock > 0)
                {
                    TempData["MessageShopCart"] = $"One of the food items ({lowstock.Title}) has its stock lower than the quantity you wanted! We brought the quantity down to the maximum option!";
                    var shopcart = await _context.ShopCarts.FindAsync(CurrentUser.Id, lowstock.Id);
                    if (shopcart != null)
                    {
                        shopcart.Quantity = lowstock.Stock;
                        _context.Update(shopcart);
                    }
                }
                
                if(lowstock.Stock == 0)
                {
                    TempData["MessageShopCart"] = $"One of the food items ({lowstock.Title}) is no longer in stock! We will remove it from your shopping cart!";
                    var shopcart = await _context.ShopCarts.FindAsync(CurrentUser.Id, lowstock.Id);
                    if (shopcart != null)
                    {
                        _context.ShopCarts.Remove(shopcart);
                    }
                }
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
        }

        var OwnedShopcart = await _context.ShopCarts
            .Include(c => c.Customer)
            .Include(c => c.Food)
            .Include(c => c.Food!.Offer)
            .Where(q => q.CustomerId == CurrentCustomer.Id)
            .OrderBy(c => c.Food!.Title)
            .ToListAsync();
        var Foods = await _context.Foods
            .Include(t => t.Ingredients!)
            .ThenInclude(t => t.Ingredient)
            .Include(t => t.Category)
            .Include(t => t.Offer)
            .Where(q => q.IsFinal && q.Stock > 0)
            .OrderBy(i => i.Title)
            .ToListAsync();
        Foods = Foods
            .Where(i => !OwnedShopcart.Any(j => j.FoodId == i.Id))
            .OrderBy(i => i.Title)
            .ToList();
        int r = rnd.Next(Foods.Count);
        var suggestion = Foods[r];

        if (ModelState.IsValid)
        {
            try
            {
                var specialday = await _context.SpecialDays
                               .Where(i => i.Day == newOrder.ArrivalTime.Day && i.Month == newOrder.ArrivalTime.Month)
                               .FirstOrDefaultAsync();
                if (specialday != null)
                {
                    if (specialday.IsWorkDay == false)
                    {
                        TempData["MessageOrder"] = $"We don't work on {specialday.Day}/{specialday.Month}! Please choose a different date!";
                        var methods = _mapper.Map<List<PayingMethodDto>>(
                            await _context.PayingMethods
                            .ToListAsync());
                        if (methods == null)
                        {
                            return NotFound();
                        }
                        ViewBag.AvailablePayingMethods = new SelectList(methods, "PayingMethodId", "Name");
                        newOrder.Suggestion = _mapper.Map<CardFoodVm>(suggestion);
                        return View(newOrder);
                    }
                    if (newOrder.ArrivalTime.TimeOfDay < specialday.StartHour || newOrder.ArrivalTime.TimeOfDay > specialday.EndHour)
                    {
                        TempData["MessageOrder"] = $"We don't work at {newOrder.ArrivalTime.TimeOfDay} on {specialday.Day}/{specialday.Month}! Please choose a different time!";
                        var methods = _mapper.Map<List<PayingMethodDto>>(
                            await _context.PayingMethods
                            .ToListAsync());
                        if (methods == null)
                        {
                            return NotFound();
                        }
                        ViewBag.AvailablePayingMethods = new SelectList(methods, "PayingMethodId", "Name");
                        newOrder.Suggestion = _mapper.Map<CardFoodVm>(suggestion);
                        return View(newOrder);
                    }
                }

                var workhour = await _context.WorkHours
                               .Where(i => i.Name.Equals(newOrder.ArrivalTime.DayOfWeek.ToString()))
                               .FirstOrDefaultAsync();
                if (workhour != null)
                {
                    if (workhour.IsWorkDay == false)
                    {
                        TempData["MessageOrder"] = $"We don't work on {workhour.Name}! Please choose a different day!";
                        var methods = _mapper.Map<List<PayingMethodDto>>(
                            await _context.PayingMethods
                            .ToListAsync());
                        if (methods == null)
                        {
                            return NotFound();
                        }
                        ViewBag.AvailablePayingMethods = new SelectList(methods, "PayingMethodId", "Name");
                        newOrder.Suggestion = _mapper.Map<CardFoodVm>(suggestion);
                        return View(newOrder);
                    }
                    if (newOrder.ArrivalTime.TimeOfDay < workhour.StartHour || newOrder.ArrivalTime.TimeOfDay > workhour.EndHour)
                    {
                        TempData["MessageOrder"] = $"We don't work at {newOrder.ArrivalTime.TimeOfDay} on {workhour.Name}! Please choose a different time!";
                        var methods = _mapper.Map<List<PayingMethodDto>>(
                            await _context.PayingMethods
                            .ToListAsync());
                        if (methods == null)
                        {
                            return NotFound();
                        }
                        ViewBag.AvailablePayingMethods = new SelectList(methods, "PayingMethodId", "Name");
                        newOrder.Suggestion = _mapper.Map<CardFoodVm>(suggestion);
                        return View(newOrder);
                    }
                }

                if(newOrder.IsDelivery == true && (newOrder.Address == null || newOrder.Address.Trim() == null || newOrder.Address.Trim().Length == 0 || newOrder.Address.Trim() == string.Empty))
                {
                    TempData["MessageOrder"] = $"For a delivery order, we need the address!";
                    var methods = _mapper.Map<List<PayingMethodDto>>(
                        await _context.PayingMethods
                        .ToListAsync());
                    if (methods == null)
                    {
                        return NotFound();
                    }
                    ViewBag.AvailablePayingMethods = new SelectList(methods, "PayingMethodId", "Name");
                    newOrder.Suggestion = _mapper.Map<CardFoodVm>(suggestion);
                    return View(newOrder);
                }

                if (newOrder.IsDelivery == false && (newOrder.Address != null && newOrder.Address.Trim() != null && newOrder.Address.Trim().Length != 0 && newOrder.Address.Trim() != string.Empty))
                {
                    TempData["MessageOrder"] = $"For a pick-up order, we don't need the address!";
                    var methods = _mapper.Map<List<PayingMethodDto>>(
                        await _context.PayingMethods
                        .ToListAsync());
                    if (methods == null)
                    {
                        return NotFound();
                    }
                    ViewBag.AvailablePayingMethods = new SelectList(methods, "PayingMethodId", "Name");
                    newOrder.Suggestion = _mapper.Map<CardFoodVm>(suggestion);
                    return View(newOrder);
                }

                if (newOrder.IsDelivery == false)
                {
                    newOrder.Address = null;
                }

                var NewOrder = _mapper.Map<Order>(newOrder);
                _context.Add(NewOrder);
                await _context.SaveChangesAsync();

                tryshopcarts = _mapper.Map<List<ShopCartVm>>(
                    await _context.ShopCarts
                    .Include(c => c.Customer)
                    .Include(c => c.Food)
                    .Include(c => c.Food!.Offer)
                    .Where(q => q.CustomerId == id).ToListAsync());

                foreach (var item in tryshopcarts)
                {
                    var newOrderContent = _mapper.Map<OrderContent>(item);
                    var addedOrder = await _context.Orders
                                    .OrderByDescending(i => i.Id)
                                    .Where(i => i.CustomerId == newOrder.CustomerId)
                                    .FirstOrDefaultAsync();
                    newOrderContent.OrderId = addedOrder!.Id;
                    _context.Add(newOrderContent);
                    await _context.SaveChangesAsync();

                    var reducedFood = await _context.Foods
                               .Where(i => i.Id == item.FoodId)
                               .FirstOrDefaultAsync();
                    reducedFood!.Stock -= item.Quantity;
                    _context.Update(reducedFood);
                    await _context.SaveChangesAsync();
                }

                var removedShopCart = await _context.ShopCarts
                                      .Where(i => i.CustomerId == newOrder.CustomerId)
                                      .ToListAsync();
                foreach (var item in removedShopCart)
                {
                    _context.ShopCarts.Remove(item);
                    await _context.SaveChangesAsync();
                }

                var soldoutShopCart = await _context.ShopCarts
                                      .Where(i => i.Food!.Stock == 0)
                                      .ToListAsync();
                foreach (var item in soldoutShopCart)
                {
                    _context.ShopCarts.Remove(item);
                    await _context.SaveChangesAsync();
                }

                var reducedShopCarts = await _context.ShopCarts
                    .Include(c => c.Food)
                    .Include(c => c.Food!.Offer)
                    .Where(q => q.FoodId == id && q.Food!.Stock < q.Quantity)
                    .ToListAsync();
                foreach (var item in reducedShopCarts)
                {
                    item.Quantity = item.Food!.Stock;
                    _context.ShopCarts.Update(item);
                    await _context.SaveChangesAsync();
                }

                TempData["MessageShopCart"] = $"Your order has been sent, {newOrder.Name}!";
                //await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShopCartOwned(newOrder.CustomerId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }
        var CurrenCustomer = await _context.Users
            .Where(q => q.UserName == User!.Identity!.Name)
            .FirstOrDefaultAsync();
        if (CurrenCustomer is null)
            return BadRequest();

        var ExistenShopCart = await _context.ShopCarts
            .Include(t => t.Customer)
            .FirstOrDefaultAsync(q => q.CustomerId == CurrenCustomer.Id);
        if (ExistenShopCart == null)
        {
            return NotFound();
        }
        var output = _mapper.Map<ExistentShopCartDto>(ExistenShopCart);
        TempData["MessageOrder"] = $"Please complete the necessary information properly!";
        var payingmethods = _mapper.Map<List<PayingMethodDto>>(
                            await _context.PayingMethods
                            .ToListAsync());
        if (payingmethods == null)
        {
            return NotFound();
        }
        ViewBag.AvailablePayingMethods = new SelectList(payingmethods, "PayingMethodId", "Name");
        newOrder.Suggestion = _mapper.Map<CardFoodVm>(suggestion);
        return View(newOrder);
    }

    private bool ShopCartExists(int uid, int vid)
    {
      return (_context.ShopCarts?.Any(e => e.CustomerId == uid && e.FoodId == vid)).GetValueOrDefault();
    }

    private bool ShopCartOwned(int uid)
    {
      return (_context.ShopCarts?.Any(e => e.CustomerId == uid)).GetValueOrDefault();
    }
}
