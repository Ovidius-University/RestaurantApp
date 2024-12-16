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
using WebAppUI.Areas.Critic.Models.ViewModels;
using WebAppUI.Areas.Critic.Models.DTOs;
using System.Security.Cryptography;
using WebAppUI.Models.ViewModels;
using WebAppUI.Models.DTOs;
using Microsoft.IdentityModel.Tokens;

namespace WebAppUI.Controllers;

public class OrderController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly UserManager<AppUser> _userManager;
    static readonly Random rnd = new();

    public OrderController(ApplicationDbContext context, IMapper mapper, UserManager<AppUser> userManager)
    {
        _context = context;
        _mapper = mapper;
        _userManager = userManager;
    }

    [Route("Orders")]
    public async Task<IActionResult> Index()
    {
        var CurrentCustomer = await _context.Users
            .FirstOrDefaultAsync(q => q.UserName == User.Identity!.Name);
        if (CurrentCustomer == null || CurrentCustomer.UserName == null)
            return Redirect("Identity/Account/Login");
        var Orders = _mapper.Map<List<OrderVm>>(
            await _context.Orders
            .Include(c => c.Customer)
            .Include(c => c.PayingMethod)
            .Include(c => c.OrderContents)
            .Include(c => c.DeliveryName)
            .Where(q => q.CustomerId == CurrentCustomer.Id)
            .OrderBy(c => c.IsFinal)
            .ThenByDescending(c => c.DeliveryTime)
            .ThenBy(c => c.ArrivalTime)
            .ThenBy(c => c.Id)
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

        var output = new IndexOrdersVm()
        {
            Customer = CurrentCustomer.UserName,
            CustomerId = CurrentCustomer.Id,
            ListOrders = Orders,
            Suggestion = _mapper.Map<CardFoodVm>(suggestion)
        };
        return View(output);
    }

    // GET: Order/Details/5
    [Route("Order/Details/{id}")]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || _context.Orders == null)
        {
            return NotFound();
        }

        var CurrentCustomer = await _context.Users
            .FirstOrDefaultAsync(q => q.UserName == User.Identity!.Name);
        if (CurrentCustomer == null || CurrentCustomer.UserName == null)
            return Problem("You haven't logged in, or haven't made an account yet!");

        var order = await _context.Orders
            .Include(c => c.Customer)
            .Include(c => c.PayingMethod)
            .Include(c => c.OrderContents)
            .FirstOrDefaultAsync(m => m.Id == id && m.CustomerId == CurrentCustomer.Id);
        if (order == null)
        {
            return NotFound();
        }

        var output = _mapper.Map<OrderDetailsVm>(order);
        output.Customer = string.IsNullOrEmpty(CurrentCustomer.UserName) ? "-" : CurrentCustomer.UserName;
        return View(output);
    }

    [Route("Orders/TimePeriod/{id}")]
    public async Task<IActionResult> TimePeriod(int? id)
    {
        var CurrentCustomer = await _context.Users
            .FirstOrDefaultAsync(q => q.UserName == User.Identity!.Name);
        if (CurrentCustomer == null || CurrentCustomer.UserName == null)
            return Redirect("Identity/Account/Login");
        var Orders = _mapper.Map<List<OrderVm>>(
            await _context.Orders
            .Include(c => c.Customer)
            .Include(c => c.PayingMethod)
            .Include(c => c.OrderContents)
            .Include(c => c.DeliveryName)
            .Where(q => q.CustomerId == CurrentCustomer.Id)
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

        if (id == 1)
        {
            Orders = Orders
            .Where(c => c.IsFinal ? c.DeliveryTime.AddDays(1) >= DateTime.Now : c.ArrivalTime.AddDays(1) >= DateTime.Now)
            .ToList();
        }
        else if (id == 2)
        {
            Orders = Orders
            .Where(c => c.IsFinal ? c.DeliveryTime.AddMonths(1) >= DateTime.Now : c.ArrivalTime.AddMonths(1) >= DateTime.Now)
            .ToList();
        }
        else
        {
            Orders = Orders
            .Where(c => c.IsFinal ? c.DeliveryTime.AddYears(1) >= DateTime.Now : c.ArrivalTime.AddYears(1) >= DateTime.Now)
            .ToList();
        }

        Orders = Orders
                .OrderBy(c => c.IsFinal)
                .ThenByDescending(c => c.DeliveryTime)
                .ThenBy(c => c.ArrivalTime)
                .ThenBy(c => c.Id)
                .ToList();

        var output = new IndexOrdersVm()
        {
            id = id,
            Customer = CurrentCustomer.UserName,
            CustomerId = CurrentCustomer.Id,
            ListOrders = Orders,
            Suggestion = _mapper.Map<CardFoodVm>(suggestion)
        };
        return View(output);
    }
    /*
    // GET: Order/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || _context.Orders == null || _context.Users == null)
        {
            return NotFound();
        }
        var CurrentCustomer = await _context.Users
            .Where(q => q.UserName == User!.Identity!.Name)
            .FirstOrDefaultAsync();
        if (CurrentCustomer is null)
            return BadRequest();

        var ExistentOrder = await _context.Orders
            .Include(t => t.Customer)
            .Include(c => c.PayingMethod)
            .Include(c => c.OrderContents)
            .FirstOrDefaultAsync(q => q.Id == id && q.CustomerId == CurrentCustomer.Id);
        if (ExistentOrder == null)
        {
            return NotFound();
        }
        var output = _mapper.Map<ExistentOrderDto>(ExistentOrder);
        var methods = _mapper.Map<List<PayingMethodDto>>(
            await _context.PayingMethods
            .ToListAsync());
        if (methods == null)
        {
            return NotFound();
        }
        ViewBag.AvailablePayingMethods = new SelectList(methods, "PayingMethodId", "Name");
        return View(output);
    }

    // POST: Order/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit([FromRoute] int id, [FromForm] ExistentOrderDto ModifiedOrder)
    {
        if (ModifiedOrder is null || id != ModifiedOrder.Id)
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
                
                var ExistentOrder = await _context.Orders
                    .FirstOrDefaultAsync(q => q.Id == id && q.CustomerId == CurrentCustomer.Id);
                if (ExistentOrder == null)
                {
                    return NotFound();
                }

                var specialday = await _context.SpecialDays
                               .Where(i => i.Day == ModifiedOrder.ArrivalTime.Day && i.Month == ModifiedOrder.ArrivalTime.Month)
                               .FirstOrDefaultAsync();
                if (specialday != null)
                {
                    if (specialday.IsWorkDay == false)
                    {
                        TempData["MessageOrderEdit"] = $"No work is done on {specialday.Day}/{specialday.Month}! Please choose a different date!";
                        var methods = _mapper.Map<List<PayingMethodDto>>(
                            await _context.PayingMethods
                            .ToListAsync());
                        if (methods == null)
                        {
                            return NotFound();
                        }
                        ViewBag.AvailablePayingMethods = new SelectList(methods, "PayingMethodId", "Name");
                        return View(ModifiedOrder);
                    }
                    if (ModifiedOrder.ArrivalTime.TimeOfDay < specialday.StartHour || ModifiedOrder.ArrivalTime.TimeOfDay > specialday.EndHour)
                    {
                        TempData["MessageOrderEdit"] = $"No work is done at {ModifiedOrder.ArrivalTime.TimeOfDay} on {specialday.Day}/{specialday.Month}! Please choose a different time!";
                        var methods = _mapper.Map<List<PayingMethodDto>>(
                            await _context.PayingMethods
                            .ToListAsync());
                        if (methods == null)
                        {
                            return NotFound();
                        }
                        ViewBag.AvailablePayingMethods = new SelectList(methods, "PayingMethodId", "Name");
                        return View(ModifiedOrder);
                    }
                }

                var weekDay = DateTime.Now.DayOfWeek.ToString();
                var workhour = await _context.WorkHours
                               .Where(i => i.Name.Equals(weekDay))
                               .FirstOrDefaultAsync();
                if (workhour != null)
                {
                    if (workhour.IsWorkDay == false)
                    {
                        TempData["MessageOrderEdit"] = $"No work is done on {workhour.Name}! Please choose a different day!";
                        var methods = _mapper.Map<List<PayingMethodDto>>(
                            await _context.PayingMethods
                            .ToListAsync());
                        if (methods == null)
                        {
                            return NotFound();
                        }
                        ViewBag.AvailablePayingMethods = new SelectList(methods, "PayingMethodId", "Name");
                        return View(ModifiedOrder);
                    }
                    if (ModifiedOrder.ArrivalTime.TimeOfDay < workhour.StartHour || ModifiedOrder.ArrivalTime.TimeOfDay > workhour.EndHour)
                    {
                        TempData["MessageOrderEdit"] = $"No work is done at {ModifiedOrder.ArrivalTime.TimeOfDay} on {workhour.Name}! Please choose a different time!";
                        var methods = _mapper.Map<List<PayingMethodDto>>(
                            await _context.PayingMethods
                            .ToListAsync());
                        if (methods == null)
                        {
                            return NotFound();
                        }
                        ViewBag.AvailablePayingMethods = new SelectList(methods, "PayingMethodId", "Name");
                        return View(ModifiedOrder);
                    }
                }

                if (ModifiedOrder.IsDelivery == true && (ModifiedOrder.Address == null || ModifiedOrder.Address.Length == 0))
                {
                    TempData["MessageOrderEdit"] = $"For a delivery, we need the address!";
                    var methods = _mapper.Map<List<PayingMethodDto>>(
                        await _context.PayingMethods
                        .ToListAsync());
                    if (methods == null)
                    {
                        return NotFound();
                    }
                    ViewBag.AvailablePayingMethods = new SelectList(methods, "PayingMethodId", "Name");
                    return View(ModifiedOrder);
                }

                if (ModifiedOrder.IsFinal == true && ExistentOrder.IsFinal == false)
                {
                    ModifiedOrder.DeliveryTime = DateTime.Now;
                }
                ModifiedOrder.ToEntity(ref ExistentOrder);
                _context.Update(ExistentOrder);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(ModifiedOrder.Id))
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

        var ExistenOrder = await _context.Orders
            .Include(t => t.Customer)
            .FirstOrDefaultAsync(q => q.Id == id && q.CustomerId == CurrenCustomer.Id);
        if (ExistenOrder == null)
        {
            return NotFound();
        }
        var output = _mapper.Map<ExistentOrderDto>(ExistenOrder);
        TempData["MessageOrderEdit"] = $"Please complete the necessary information properly!";
        var payingmethods = _mapper.Map<List<PayingMethodDto>>(
                            await _context.PayingMethods
                            .ToListAsync());
        if (payingmethods == null)
        {
            return NotFound();
        }
        ViewBag.AvailablePayingMethods = new SelectList(payingmethods, "PayingMethodId", "Name");
        return View(output);
    }

    // GET: Order/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null || _context.Orders == null)
        {
            return NotFound();
        }
        var CurrentCustomer = await _context.Users
                    .Where(q => q.UserName == User!.Identity!.Name)
                    .FirstOrDefaultAsync();
        if (CurrentCustomer is null)
            return BadRequest();

        var Order = await _context.Orders
            .Include(c => c.Customer)
            .Include(c => c.PayingMethod)
            .Include(c => c.OrderContents)
            .FirstOrDefaultAsync(m => m.Id == id && m.CustomerId == CurrentCustomer.Id);
        if (Order == null)
        {
            return NotFound();
        }

        if (Order.IsFinal == true)
        {
            TempData["MessageOwnedOrder"] = $"Order number {Order.Id} cannot be removed as it has been completed! You'll need to make it not final!";
            return RedirectToAction(nameof(Index));
        }

        var Customers = await _context.Orders
            .Include(t => t.Customer)
            .Where(c => c.Id == id && c.CustomerId == CurrentCustomer.Id)
            .ToListAsync();
        var output = _mapper.Map<OrderDetailsVm>(Order);
        var customers = "";
        foreach (var customer in Customers)
        {
            customers += $"{customer.Customer!.UserName}";
        }
        output.Customer = string.IsNullOrEmpty(customers) ? "-" : customers.Substring(0, customers.Length);
        return View(output);
    }

    // POST: Order/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var CurrentUser = await _userManager.FindByEmailAsync(User.Identity!.Name!);
        if (CurrentUser == null)
            return BadRequest();

        if (_context.Orders == null)
        {
            return Problem("Entity set 'ApplicationDbContext.Orders' is null.");
        }

        var orderContents = await _context.OrderContents
                    .Include(c => c.Order)
                    .Include(c => c.Food)
                    .Include(c => c.Food!.Offer)
                    .Where(q => q.OrderId == id).ToListAsync();

        foreach (var item in orderContents)
        {
            var stockedFood = await _context.Food
                       .Where(i => i.Id == item.FoodId)
                       .FirstOrDefaultAsync();
            stockedFood!.Stock += item.Quantity;
            _context.Update(stockedFood);
            await _context.SaveChangesAsync();
        }

        var Order = await _context.Orders.FindAsync(id);
        if (Order != null)
        {
            _context.Orders.Remove(Order);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    */

    // GET: Order/FeedBack/5
    public async Task<IActionResult> FeedBack(int? id)
    {
        if (id == null || _context.Orders == null)
        {
            return NotFound();
        }

        var order = await _context.Orders
            .Include(c => c.Customer)
            .Include(c => c.PayingMethod)
            .Include(c => c.OrderContents)
            .Include(c => c.Customer)
            .FirstOrDefaultAsync(m => m.Id == id && m.IsFinal);
        if (order == null)
        {
            return RedirectToAction(nameof(Index));
        }
        var ListFood = await _context.OrderContents
            .Include(t => t.Food)
            .Where(c => c.OrderId == id)
            .ToListAsync();

        var output = _mapper.Map<OrderFeedBackDto>(order);
        var foods = _mapper.Map<List<ExistentOrderContentDto>>(ListFood);

        output.Foods = foods;
        var FeedBack = await _context.FeedBacks.FindAsync(id);
        if (FeedBack is not null)
        {
            output.Comment = FeedBack.Comment;
            output.IsNewFeedBack = false;
        }
        return View(output);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> FeedBack([FromRoute] int? id, [FromForm] OrderFeedBackDto NewFeedBack)
    {
        if (id == null || _context.Orders == null)
        {
            return NotFound();
        }
        var ExistentOrder = await _context.Orders
            .FirstOrDefaultAsync(m => m.Id == id && m.IsFinal);
        if (ExistentOrder == null)
        {
            return RedirectToAction(nameof(Index));
        }
        if (ModelState.IsValid)
        {
            var ExistentFeedBack = await _context.FeedBacks
                .Include(t => t.Order)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (ExistentFeedBack is null)
            {
                // add feedback
                var FeedBack = _mapper.Map<FeedBack>(NewFeedBack);
                await _context.AddAsync(FeedBack);
                TempData["MessageOwnedOrder"] = $"Feedback for order number {id} has been sent!";
            }
            else
            {
                // modify feedback
                ExistentFeedBack.Comment = NewFeedBack.Comment;
                _context.Update(ExistentFeedBack);
                TempData["MessageOwnedOrder"] = $"Feedback for order number {id} has been modified!";
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(NewFeedBack);
    }

    private bool OrderExists(int id)
    {
      return (_context.Orders?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}
