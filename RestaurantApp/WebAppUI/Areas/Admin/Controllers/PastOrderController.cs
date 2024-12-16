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
using WebAppUI.Areas.Admin.Models.ViewModels;
using WebAppUI.Areas.Admin.Models.DTOs;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;

namespace WebAppUI.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Administrator")]

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

    [Route("Admin/PastOrders")]
    public async Task<IActionResult> Index()
    {
        var Orders = _mapper.Map<List<OrderVm>>(
            await _context.Orders
            .Include(c => c.Customer)
            .Include(c => c.DeliveryName)
            .Include(c => c.PayingMethod)
            .Include(c => c.OrderContents)
            .Include(c => c.FeedBack)
            .Where(c => c.IsFinal)
            .OrderByDescending(c => c.DeliveryTime)
            .ThenBy(c => c.ArrivalTime)
            .ThenBy(c => c.Id)
            .ToListAsync());
        var output = new IndexOrdersVm()
        {
            ListOrders = Orders
        };
        return View(output);
    }

    // GET: PastOrder/Details/5
    [Route("Admin/PastOrder/Details/{id}")]
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

    // GET: PastOrder/FeedBack/5
    [Route("Admin/PastOrder/FeedBack/{id}")]
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

    [Route("Admin/PastOrder/TimePeriod/{id}")]
    public async Task<IActionResult> TimePeriod(int? id)
    {
        var Orders = _mapper.Map<List<OrderVm>>(
            await _context.Orders
            .Include(c => c.Customer)
            .Include(c => c.DeliveryName)
            .Include(c => c.PayingMethod)
            .Include(c => c.OrderContents)
            .Include(c => c.FeedBack)
            .Where(c => c.IsFinal)
            .OrderByDescending(c => c.DeliveryTime)
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
            .Include(c => c.DeliveryName)
            .FirstOrDefaultAsync(q => q.Id == id);
        if (ExistentOrder == null)
        {
            return NotFound();
        }
        var output = _mapper.Map<ExistentOrderDto>(ExistentOrder);
        output.IsFinal = !ExistentOrder.IsFinal;
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

    // POST: PastOrder/Edit/5
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
                var ExistentOrder = await _context.Orders
                    .FirstOrDefaultAsync(q => q.Id == id);
                if (ExistentOrder == null)
                {
                    return NotFound();
                }

                if (ModifiedOrder.IsFinal == true && ExistentOrder.IsFinal == false)
                {
                    ModifiedOrder.DeliveryTime = DateTime.Now;
                    TempData["MessageAdminPastOrder"] = $"Order number {ModifiedOrder.Id} for {ModifiedOrder.Name} has been confirmed!";
                }
                if (ModifiedOrder.IsFinal == false && ExistentOrder.IsFinal == true)
                {
                    TempData["MessageAdminPastOrder"] = $"Order number {ModifiedOrder.Id} for {ModifiedOrder.Name} has been unconfirmed!";

                    var ExistentFeedBack = await _context.FeedBacks
                                        .FirstOrDefaultAsync(q => q.Id == id);
                    if (ExistentFeedBack != null)
                    {
                        _context.Remove(ExistentFeedBack);
                        await _context.SaveChangesAsync();
                    }

                    if (ModifiedOrder.IsDelivery == false)
                    {
                        ModifiedOrder.WorkerId = 0;
                    }
                }
                ModifiedOrder.ToEntity(ref ExistentOrder);
                _context.Update(ExistentOrder);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PastOrderExists(ModifiedOrder.Id))
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
            .Include(c => c.DeliveryName)
            .Include(c => c.PayingMethod)
            .FirstOrDefaultAsync(q => q.Id == id);
        if (ExistenOrder == null)
        {
            return NotFound();
        }
        var output = _mapper.Map<ExistentOrderDto>(ExistenOrder);
        output.IsFinal = !ExistenOrder.IsFinal;
        TempData["MessagePastOrderEdit"] = $"Please complete the necessary information properly!";
        return View(output);
    }

    private bool PastOrderExists(int id)
    {
      return (_context.Orders?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}
