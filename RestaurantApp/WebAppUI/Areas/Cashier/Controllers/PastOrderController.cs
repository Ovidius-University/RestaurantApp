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
using WebAppUI.Areas.Cashier.Models.ViewModels;
using WebAppUI.Areas.Cashier.Models.DTOs;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebAppUI.Areas.Cashier.Controllers;

[Area("Cashier")]
[Authorize(Roles = "Cashier")]

public class PastOrderController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly UserManager<AppUser> _userManager;

    public PastOrderController(ApplicationDbContext context, IMapper mapper, UserManager<AppUser> userManager)
    {
        _context = context;
        _mapper = mapper;
        _userManager = userManager;
    }

    [Route("Cashier/PastOrders")]
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
                        .FirstOrDefaultAsync();

        if (empSched == null)
        {
            return NotFound();
        }

        var Orders = _mapper.Map<List<OrderVm>>(
                await _context.Orders
                .Include(c => c.Customer)
                .Include(c => c.PayingMethod)
                .Include(c => c.OrderContents)
                .Include(c => c.FeedBack)
                .Where(c => c.IsDelivery == false && c.WorkerId == empSched.EmployeeId)
                .OrderBy(c => c.IsFinal)
                .ThenByDescending(c => c.DeliveryTime)
                .ThenBy(c => c.ArrivalTime)
                .ThenBy(c => c.Id)
                .ToListAsync());

        output.ListOrders = Orders;
        return View(output);
    }

    // GET: PastOrder/Details/5
    [Route("Cashier/PastOrder/Details/{id}")]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || _context.Orders == null)
        {
            return NotFound();
        }

        var order = await _context.Orders
            .Include(c => c.Customer)
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

    // GET: Cashier/PastOrder/FeedBack/5
    [Route("Cashier/PastOrder/FeedBack/{id}")]
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
    }

    [Route("Cashier/PastOrder/TimePeriod/{id}")]
    public async Task<IActionResult> TimePeriod(int? id)
    {
        var empSched = await _context.EmployeeSchedules
                        .Include(i => i.Employee)
                        .Include(i => i.Day)
                        .Where(i => i.Employee!.UserName == User.Identity!.Name)
                        .FirstOrDefaultAsync();

        if (empSched == null)
        {
            return NotFound();
        }

        var Orders = _mapper.Map<List<OrderVm>>(
                await _context.Orders
                .Include(c => c.Customer)
                .Include(c => c.PayingMethod)
                .Include(c => c.OrderContents)
                .Include(c => c.FeedBack)
                .Where(c => c.IsDelivery == false && c.WorkerId == empSched.EmployeeId)
                .OrderBy(c => c.IsFinal)
                .ThenByDescending(c => c.DeliveryTime)
                .ThenBy(c => c.ArrivalTime)
                .ThenBy(c => c.Id)
                .ToListAsync());

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

        var output = new OrderTimeVm()
        {
            id = (int)id!,
            ListOrders = Orders
        };
        return View(output);
    }

    // GET: PastOrder/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || _context.Orders == null || _context.Users == null)
        {
            return NotFound();
        }
        var ExistentOrder = await _context.Orders
            .Include(t => t.Customer)
            .Include(c => c.PayingMethod)
            .Include(c => c.OrderContents)
            .FirstOrDefaultAsync(q => q.Id == id);
        if (ExistentOrder == null)
        {
            return NotFound();
        }
        var empSched = await _context.EmployeeSchedules
                        .Include(i => i.Employee)
                        .Include(i => i.Day)
                        .Where(i => i.Employee!.UserName == User.Identity!.Name)
                        .FirstOrDefaultAsync();
        if (empSched == null)
        {
            return NotFound();
        }

        var output = _mapper.Map<ExistentOrderDto>(ExistentOrder);
        output.IsFinal = !ExistentOrder.IsFinal;
        if (ExistentOrder.WorkerId == null || ExistentOrder.WorkerId == 0)
        {
            output.WorkerId = empSched!.EmployeeId;
        }
        else output.WorkerId = null;
        /*
        var methods = _mapper.Map<List<PayingMethodDto>>(
            await _context.PayingMethods
            .ToListAsync());
        if (methods == null)
        {
            return NotFound();
        }
        ViewBag.AvailablePayingMethods = new SelectList(methods, "PayingMethodId", "Name");
        */
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
                var empSched = await _context.EmployeeSchedules
                        .Include(i => i.Employee)
                        .Include(i => i.Day)
                        .Where(i => i.Employee!.UserName == User.Identity!.Name)
                        .ToListAsync();

                var psbl = false;

                foreach (var item in empSched)
                {
                    var weekday = item.Day!.Name;

                    var Order = await _context.Orders
                        .Include(c => c.Customer)
                        .Include(c => c.PayingMethod)
                        .Include(c => c.OrderContents)
                        .Include(c => c.DeliveryName)
                        .Where(c => c.Id == id && c.IsDelivery == false
                         && c.ArrivalTime.TimeOfDay >= item.StartHour && c.ArrivalTime.TimeOfDay <= item.EndHour
                         && (c.WorkerId == null || c.WorkerId == 0 || c.WorkerId == item.EmployeeId))
                        .FirstOrDefaultAsync();

                    if (Order != null)
                    {
                        if (Order.ArrivalTime.DayOfWeek.ToString().Equals(weekday))
                            psbl = true;
                    }
                }

                if (!psbl)
                {
                    TempData["MessagePickedOrder"] = $"Order number {ModifiedOrder.Id} is either no longer a pick-up, no longer within your schedule, or was given by someone else already!";
                    return RedirectToAction(nameof(Index));
                }

                var ExistentOrder = await _context.Orders
                    .FirstOrDefaultAsync(q => q.Id == id);
                if (ExistentOrder == null)
                {
                    return NotFound();
                }
                /*
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
                //
                if (ModifiedOrder.IsFinal == true)
                {
                    var specialday = await _context.SpecialDays
                               .Where(i => i.Day == DateTime.Now.Day && i.Month == DateTime.Now.Month)
                               .FirstOrDefaultAsync();
                    if (specialday != null)
                    {
                        if (specialday.IsWorkDay == false)
                        {
                            TempData["MessageOrderEdit"] = $"No work is done on {specialday.Day}/{specialday.Month}! You'll have to confirm the pick-up at a later date!";
                            return View(ModifiedOrder);
                        }
                        if (DateTime.Now.TimeOfDay < specialday.StartHour || DateTime.Now.TimeOfDay > specialday.EndHour)
                        {
                            TempData["MessageOrderEdit"] = $"No work is done at {DateTime.Now.TimeOfDay} on {specialday.Day}/{specialday.Month}! You'll have to confirm the pick-up at a later time!";
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
                            TempData["MessageOrderEdit"] = $"No work is done on {workhour.Name}! You'll have to confirm the pick-up at a later date!";
                            return View(ModifiedOrder);
                        }
                        if (DateTime.Now.TimeOfDay < workhour.StartHour || DateTime.Now.TimeOfDay > workhour.EndHour)
                        {
                            TempData["MessageOrderEdit"] = $"No work is done at {DateTime.Now.TimeOfDay} on {workhour.Name}! You'll have to confirm the pick-up at a later time!";
                            return View(ModifiedOrder);
                        }
                    }
                }
                */

                if (ModifiedOrder.IsFinal == true && ExistentOrder.IsFinal == false)
                {
                    ModifiedOrder.DeliveryTime = DateTime.Now;
                    TempData["MessagePickedOrder"] = $"Order number {ModifiedOrder.Id} for {ModifiedOrder.Name} has been confirmed!";
                }
                if (ModifiedOrder.IsFinal == false && ExistentOrder.IsFinal == true)
                {
                    TempData["MessagePickedOrder"] = $"Order number {ModifiedOrder.Id} for {ModifiedOrder.Name} has been unconfirmed!";

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
            .Include(c => c.PayingMethod)
            .FirstOrDefaultAsync(q => q.Id == id);
        if (ExistenOrder == null)
        {
            return NotFound();
        }

        var empName = await _context.EmployeeSchedules
                        .Include(i => i.Employee)
                        .Include(i => i.Day)
                        .Where(i => i.Employee!.UserName == User.Identity!.Name)
                        .FirstOrDefaultAsync();
        var output = _mapper.Map<ExistentOrderDto>(ExistenOrder);
        output.IsFinal = !ExistenOrder.IsFinal;
        if (ExistenOrder.WorkerId == null || ExistenOrder.WorkerId == 0)
        {
            output.WorkerId = empName!.EmployeeId;
        }
        else output.WorkerId = null;
        TempData["MessageOrderEdit"] = $"Please complete the necessary information properly!";
        /*
        var payingmethods = _mapper.Map<List<PayingMethodDto>>(
                            await _context.PayingMethods
                            .ToListAsync());
        if (payingmethods == null)
        {
            return NotFound();
        }
        ViewBag.AvailablePayingMethods = new SelectList(payingmethods, "PayingMethodId", "Name");
        */
        return View(output);
    }
    /*
    // GET: Order/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null || _context.Orders == null)
        {
            return NotFound();
        }
        
        var Order = await _context.Orders
            .Include(c => c.Customer)
            .Include(c => c.PayingMethod)
            .Include(c => c.OrderContents)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (Order == null|| Order.Customer == null)
        {
            return NotFound();
        }

        if (Order.IsFinal == true)
        {
            TempData["MessagePickedOrder"] = $"Order number {Order.Id} cannot be removed as it has been completed!";
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
