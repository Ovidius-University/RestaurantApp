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
using WebAppUI.Areas.Chef.Models.ViewModels;
using WebAppUI.Areas.Chef.Models.DTOs;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;

namespace WebAppUI.Areas.Chef.Controllers;

[Area("Chef")]
[Authorize(Roles = "Chef")]

public class OrderController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly UserManager<AppUser> _userManager;

    public OrderController(ApplicationDbContext context, IMapper mapper, UserManager<AppUser> userManager)
    {
        _context = context;
        _mapper = mapper;
        _userManager = userManager;
    }

    [Route("Chef/Orders")]
    public async Task<IActionResult> Index()
    {
        var output = new IndexOrdersVm()
        {
            ListOrders = new List<OrderVm>()
        };

        var empSched = await _context.EmployeeSchedules
                        .Include(i => i.Employee)
                        .Include(i => i.Day)
                        .Where(i => i.Employee!.UserName == User.Identity!.Name)
                        .ToListAsync();

        foreach (var item in empSched)
        {
            var weekday = item.Day!.Name;

            var Orders = _mapper.Map<List<OrderVm>>(
                await _context.Orders
                .Include(c => c.Customer)
                .Include(c => c.DeliveryName)
                .Include(c => c.PayingMethod)
                .Include(c => c.OrderContents)
                .Include(c => c.FeedBack)
                .Where(c => c.ArrivalTime.TimeOfDay >= item.StartHour && c.ArrivalTime.TimeOfDay <= item.EndHour && c.IsFinal == false)
                .ToListAsync());

            if (Orders != null)
            {
                foreach (var i in Orders)
                {
                    if (i.ArrivalTime.DayOfWeek.ToString().Equals(weekday))
                        output.ListOrders.Add(i);
                }
            }
        }

        output.ListOrders = output.ListOrders
                            .OrderBy(c => c.ArrivalTime)
                            .ThenBy(c => c.Id)
                            .ToList();

        return View(output);
    }

    // GET: Order/Details/5
    [Route("Chef/Order/Details/{id}")]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || _context.Orders == null)
        {
            return NotFound();
        }

        var order = await _context.Orders
            .Include(c => c.Customer)
            .Include(c => c.DeliveryName)
            .Include(c => c.PayingMethod)
            .Include(c => c.OrderContents)
            .Include(c => c.FeedBack)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (order == null||order.Customer == null)
        {
            return NotFound();
        }

        var output = _mapper.Map<OrderDetailsVm>(order);

        output.Customer = string.IsNullOrEmpty(order.Customer.UserName) ? "-" : order.Customer.UserName;
        return View(output);
    }
    /*
    // GET: Order/FeedBack/5
    [Route("Chef/Order/FeedBack/{id}")]
    public async Task<IActionResult> FeedBack(int? id)
    {
        if (id == null || _context.Orders == null)
        {
            return RedirectToAction(nameof(Index));
        }

        var order = await _context.Orders
            .Include(c => c.Customer)
            .Include(c => c.DeliveryName)
            .Include(c => c.PayingMethod)
            .Include(c => c.OrderContents)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (order == null || order.Customer == null)
        {
            return NotFound();
        }

        var ListFoods = await _context.OrderContents
                .Include(t => t.Food)
                .Where(c => c.OrderId == id)
                .ToListAsync();

        var output = _mapper.Map<OrderFeedBackDto>(order);
        var foods = _mapper.Map<List<ExistentOrderContentDto>>(ListFoods);

        output.Foods = foods;
        var FeedBack = await _context.FeedBacks.FindAsync(id);
        if (FeedBack is not null)
        {
            output.Comment = FeedBack.Comment;
            output.IsNewFeedBack = false;
        }
        return View(output);
    }*/

    [Route("Chef/Order/TimePeriod/{id}")]
    public async Task<IActionResult> TimePeriod(int? id)
    {
        var output = new OrderTimeVm()
        {
            id = (int)id!,
            ListOrders = new List<OrderVm>()
        };

        var empSched = await _context.EmployeeSchedules
                        .Include(i => i.Employee)
                        .Include(i => i.Day)
                        .Where(i => i.Employee!.UserName == User.Identity!.Name)
                        .ToListAsync();

        foreach (var item in empSched)
        {
            var weekday = item.Day!.Name;

            var Orders = _mapper.Map<List<OrderVm>>(
                await _context.Orders
                .Include(c => c.Customer)
                .Include(c => c.DeliveryName)
                .Include(c => c.PayingMethod)
                .Include(c => c.OrderContents)
                .Include(c => c.FeedBack)
                .Where(c => c.ArrivalTime.TimeOfDay >= item.StartHour && c.ArrivalTime.TimeOfDay <= item.EndHour && c.IsFinal == false)
                .OrderBy(c => c.IsFinal)
                .ThenByDescending(c => c.DeliveryTime)
                .ThenBy(c => c.ArrivalTime)
                .ThenBy(c => c.Id)
                .ToListAsync());

            if (Orders != null)
            {
                foreach (var i in Orders)
                {
                    if (i.ArrivalTime.DayOfWeek.ToString().Equals(weekday))
                        output.ListOrders.Add(i);
                }
            }
        }

        if (id == 1)
        {
            output.ListOrders = output.ListOrders
            .Where(c => c.IsFinal ? c.DeliveryTime.AddDays(1) >= DateTime.Now : c.ArrivalTime.AddDays(1) >= DateTime.Now)
            .ToList();
        }
        else if (id == 2)
        {
            output.ListOrders = output.ListOrders
            .Where(c => c.IsFinal ? c.DeliveryTime.AddMonths(1) >= DateTime.Now : c.ArrivalTime.AddMonths(1) >= DateTime.Now)
            .ToList();
        }
        else
        {
            output.ListOrders = output.ListOrders
            .Where(c => c.IsFinal ? c.DeliveryTime.AddYears(1) >= DateTime.Now : c.ArrivalTime.AddYears(1) >= DateTime.Now)
            .ToList();
        }

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
        var ExistentOrder = await _context.Orders
            .Include(t => t.Customer)
            .Include(c => c.DeliveryName)
            .Include(c => c.PayingMethod)
            .Include(c => c.OrderContents)
            .FirstOrDefaultAsync(q => q.Id == id);
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

        var deliveries = _mapper.Map<List<ExistentUserDto>>(
            await _userManager.GetUsersInRoleAsync("Delivery"));
        deliveries.Add(new ExistentUserDto() { Id = 0, IsDelivery = true });
        ViewBag.UnaddedWorkers = new SelectList(deliveries, "Id", "Email");
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
        var deliveries = _mapper.Map<List<ExistentUserDto>>(
            await _userManager.GetUsersInRoleAsync("Delivery"));
        deliveries.Add(new ExistentUserDto() { Id = 0, IsDelivery = true });

        if (ModelState.IsValid)
        {
            try
            {
                var ExistentOrder = await _context.Orders
                    .FirstOrDefaultAsync(q => q.Id == id);
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
                        ViewBag.UnaddedWorkers = new SelectList(deliveries, "Id", "Email");
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
                        ViewBag.UnaddedWorkers = new SelectList(deliveries, "Id", "Email");
                        return View(ModifiedOrder);
                    }
                }

                var workhour = await _context.WorkHours
                               .Where(i => i.Name.Equals(ModifiedOrder.ArrivalTime.DayOfWeek.ToString()))
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
                        ViewBag.UnaddedWorkers = new SelectList(deliveries, "Id", "Email");
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
                        ViewBag.UnaddedWorkers = new SelectList(deliveries, "Id", "Email");
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
                    ViewBag.UnaddedWorkers = new SelectList(deliveries, "Id", "Email");
                    return View(ModifiedOrder);
                }

                var empSched = await _context.EmployeeSchedules
                        .Include(i => i.Employee)
                        .Include(i => i.Day)
                        .Where(i => i.EmployeeId == ModifiedOrder.WorkerId)
                        .ToListAsync();

                var psblty = false;

                if(empSched != null)
                {
                    foreach (var item in empSched)
                    {
                        var weekday = item.Day!.Name;

                        if (ModifiedOrder.ArrivalTime.TimeOfDay >= item.StartHour
                            && ModifiedOrder.ArrivalTime.TimeOfDay <= item.EndHour
                            && ModifiedOrder.ArrivalTime.DayOfWeek.ToString().Equals(weekday))

                            psblty = true;
                    }
                }

                if (psblty==false && ModifiedOrder.IsDelivery==true && ModifiedOrder.WorkerId!=0)
                {
                    TempData["MessageOrderEdit"] = $"The employee you picked for the delivery does not work at that time! Please choose someone else!";
                    var methods = _mapper.Map<List<PayingMethodDto>>(
                        await _context.PayingMethods
                        .ToListAsync());
                    if (methods == null)
                    {
                        return NotFound();
                    }
                    ViewBag.AvailablePayingMethods = new SelectList(methods, "PayingMethodId", "Name");
                    ViewBag.UnaddedWorkers = new SelectList(deliveries, "Id", "Email");
                    return View(ModifiedOrder);
                }
                if(ModifiedOrder.IsFinal == true)
                {
                    specialday = await _context.SpecialDays
                               .Where(i => i.Day == DateTime.Now.Day && i.Month == DateTime.Now.Month)
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
                            ViewBag.UnaddedWorkers = new SelectList(deliveries, "Id", "Email");
                            return View(ModifiedOrder);
                        }
                        if (DateTime.Now.TimeOfDay < specialday.StartHour || DateTime.Now.TimeOfDay > specialday.EndHour)
                        {
                            TempData["MessageOrderEdit"] = $"No work is done at {DateTime.Now.TimeOfDay} on {specialday.Day}/{specialday.Month}! Please choose a different time!";
                            var methods = _mapper.Map<List<PayingMethodDto>>(
                                await _context.PayingMethods
                                .ToListAsync());
                            if (methods == null)
                            {
                                return NotFound();
                            }
                            ViewBag.AvailablePayingMethods = new SelectList(methods, "PayingMethodId", "Name");
                            ViewBag.UnaddedWorkers = new SelectList(deliveries, "Id", "Email");
                            return View(ModifiedOrder);
                        }
                    }

                    var weekDay = DateTime.Now.DayOfWeek.ToString();
                    workhour = await _context.WorkHours
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
                            ViewBag.UnaddedWorkers = new SelectList(deliveries, "Id", "Email");
                            return View(ModifiedOrder);
                        }
                        if (DateTime.Now.TimeOfDay < workhour.StartHour || DateTime.Now.TimeOfDay > workhour.EndHour)
                        {
                            TempData["MessageOrderEdit"] = $"No work is done at {DateTime.Now.TimeOfDay} on {workhour.Name}! Please choose a different time!";
                            var methods = _mapper.Map<List<PayingMethodDto>>(
                                await _context.PayingMethods
                                .ToListAsync());
                            if (methods == null)
                            {
                                return NotFound();
                            }
                            ViewBag.AvailablePayingMethods = new SelectList(methods, "PayingMethodId", "Name");
                            ViewBag.UnaddedWorkers = new SelectList(deliveries, "Id", "Email");
                            return View(ModifiedOrder);
                        }
                    }
                }
                
                if (ModifiedOrder.IsDelivery == false)
                {
                    ModifiedOrder.WorkerId = 0;
                }
                
                if (ModifiedOrder.IsFinal == true && ExistentOrder.IsFinal == false)
                {
                    ModifiedOrder.DeliveryTime = DateTime.Now;
                }
                if (ModifiedOrder.IsFinal == false && ExistentOrder.IsFinal == true)
                {
                    var ExistentFeedBack = await _context.FeedBacks
                                        .FirstOrDefaultAsync(q => q.Id == id);
                    if (ExistentFeedBack != null)
                    {
                        _context.Remove(ExistentFeedBack);
                        await _context.SaveChangesAsync();
                    }
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

        var ExistenOrder = await _context.Orders
            .Include(t => t.Customer)
            .FirstOrDefaultAsync(q => q.Id == id);
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
        ViewBag.UnaddedWorkers = new SelectList(deliveries, "Id", "Email");
        return View(output);
    }

    // GET: Order/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null || _context.Orders == null)
        {
            return NotFound();
        }
        
        var Order = await _context.Orders
            .Include(c => c.Customer)
            .Include(c => c.DeliveryName)
            .Include(c => c.PayingMethod)
            .Include(c => c.OrderContents)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (Order == null|| Order.Customer == null)
        {
            return NotFound();
        }

        if (Order.IsFinal == true)
        {
            TempData["MessageChefOrder"] = $"Order number {Order.Id} cannot be removed as it has been completed! You'll need to make it not final!";
            return RedirectToAction(nameof(Index));
        }

        var Customers = await _context.Orders
            .Include(t => t.Customer)
            .Where(c => c.Id == id)
            .ToListAsync();
        var output = _mapper.Map<OrderDetailsVm>(Order);
        output.Customer = string.IsNullOrEmpty(Order.Customer.UserName) ? "-" : Order.Customer.UserName;
        return View(output);
    }

    // POST: Order/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
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
            var stockedFood = await _context.Foods
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

    private bool OrderExists(int id)
    {
      return (_context.Orders?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}
