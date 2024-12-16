﻿using System;
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

public class OrderContentController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly UserManager<AppUser> _userManager;

    public OrderContentController(ApplicationDbContext context, IMapper mapper, UserManager<AppUser> userManager)
    {
        _context = context;
        _mapper = mapper;
        _userManager = userManager;
    }

    [Route("OrderContents/{oid}")]
    public async Task<IActionResult> Index(int oid)
    {
        var CurrentCustomer = await _context.Users
            .FirstOrDefaultAsync(q => q.UserName == User.Identity!.Name);
        if (CurrentCustomer == null || CurrentCustomer.UserName == null)
            return Problem("You haven't logged in, or haven't even made an account yet!");
        var ordercontents = _mapper.Map<List<OrderContentVm>>(
            await _context.OrderContents
            .Include(c => c.Order)
            .Include(c => c.Food)
            .Include(c => c.Food!.Offer)
            .Where(q => q.OrderId == oid && q.Order!.CustomerId == CurrentCustomer.Id).ToListAsync());
        var output = new IndexOrderContentsVm()
        {
            OrderId = oid,
            Customer = CurrentCustomer.UserName,
            CustomerId = CurrentCustomer.Id,
            ListOrderContents = ordercontents
        };
        return View(output);
    }

    // GET: OrderContent/Details/5
    [Route("OrderContent/Details/{oid}/{id}")]
    public async Task<IActionResult> Details(int? oid, int? id)
    {
        if (id == null || _context.OrderContents == null)
        {
            return NotFound();
        }

        var CurrentCustomer = await _context.Users
            .FirstOrDefaultAsync(q => q.UserName == User.Identity!.Name);
        if (CurrentCustomer == null || CurrentCustomer.UserName == null)
            return Problem("You haven't logged in, or haven't made an account yet!");

        var ordercontent = await _context.OrderContents
            .Include(c => c.Order)
            .Include(c => c.Food)
            .Include(c => c.Food!.Offer)
            .FirstOrDefaultAsync(m => m.OrderId == oid && m.FoodId == id && m.Order!.CustomerId == CurrentCustomer.Id);
        if (ordercontent == null)
        {
            return NotFound();
        }

        var output = _mapper.Map<OrderContentDetailsVm>(ordercontent);
        output.Customer = string.IsNullOrEmpty(CurrentCustomer.UserName) ? "-" : CurrentCustomer.UserName;
        return View(output);
    }
    /*
    // GET: OrderContent/Create
    public async Task<IActionResult> Create(int oid)
    {
        if (_context.Users == null || _context.Foods == null)
        {
            return NotFound();
        }

        var CurrentCustomer = await _context.Users
            .FirstOrDefaultAsync(q => q.UserName == User.Identity!.Name);

        if (CurrentCustomer == null)
            return NotFound();//redirectionez
        var foods = _mapper.Map<List<Models.DTOs.FoodDto>>(
            await _context.Foods
            .Where(q => q.IsFinal && q.Stock>0).ToListAsync());
        ViewBag.UnaddedFoods = new SelectList(foods, "FoodId", "Title");
        
        var vid = await _context.Foods
            .Where(q => q.IsFinal && q.Stock > 0)
            .OrderBy(q => q.Id)
            .FirstOrDefaultAsync();
        if(vid == null)
        {
            TempData["MessageOrderContent"] = $"No food can be added to the order right now!";
            return RedirectToAction(nameof(Index), new { oid });
        }
        var newOrderContent = new NewOrderContentDto()
        {
            OrderId = oid,
            Quantity = 1,
            FoodId = vid.Id
        };
        
        return View(newOrderContent);
    }

    // POST: OrderContent/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([FromQuery]int oid, [FromForm] NewOrderContentDto newOrderContent)
    {
        var CurrentUser = await _userManager.FindByEmailAsync(User.Identity!.Name!);
        if (CurrentUser == null)
            return BadRequest();

        var redid = oid;

        var food = await _context.Foods
                              .Where(q => q.Id == newOrderContent.FoodId)
                              .FirstOrDefaultAsync();
        if (food == null)
            return BadRequest();

        if (ModelState.IsValid)
        {
            var resource = await _context.Foods
            .Where(q => q.Id == newOrderContent!.FoodId && q.IsFinal)
            .FirstOrDefaultAsync();
            if (resource == null)
            {
                TempData["MessageOrderContent"] = $"The food is not available anymore!";
                return RedirectToAction(nameof(Index), new { oid=newOrderContent.OrderId });
            }

            var history = await _context.OrderContents
            .Where(q => q.Order!.CustomerId == CurrentUser!.Id && q.FoodId == newOrderContent!.FoodId)
            .FirstOrDefaultAsync();
            if (history != null)
            {
                TempData["MessageOrderContent"] = $"The food is already in the order!";
                return RedirectToAction(nameof(Index), new { oid=newOrderContent.OrderId });
            }

            if (resource.Stock < newOrderContent.Quantity)
            {
                TempData["MessageOrderContent"] = $"The addition of {newOrderContent.Quantity} is bigger than what is in stock right now ({resource.Stock})!";
                return RedirectToAction(nameof(Index), new { oid = newOrderContent.OrderId });
            }

            var NewOrderContent = _mapper.Map<OrderContent>(newOrderContent);
            NewOrderContent.OrderId = newOrderContent.OrderId;
            _context.Add(NewOrderContent);
            await _context.SaveChangesAsync();

            var reducedFood = await _context.Foods
                               .Where(i => i.Id == newOrderContent.FoodId)
                               .FirstOrDefaultAsync();
            reducedFood!.Stock -= newOrderContent.Quantity;
            _context.Update(reducedFood);
            await _context.SaveChangesAsync();

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
                .Where(q => q.FoodId == newOrderContent.FoodId && q.Food!.Stock < q.Quantity)
                .ToListAsync();
            foreach (var item in reducedShopCarts)
            {
                item.Quantity = item.Food!.Stock;
                _context.ShopCarts.Update(item);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index), new { oid = newOrderContent.OrderId });
        }
        var foods = _mapper.Map<List<Models.DTOs.FoodDto>>(
            await _context.Foods
            .Where(q => q.IsFinal && q.Stock > 0).ToListAsync());
        ViewBag.UnaddedFoods = new SelectList(foods, "FoodId", "Title");
        var vid = await _context.Foods
            .Where(q => q.IsFinal && q.Stock > 0)
            .OrderBy(q => q.Id)
            .FirstOrDefaultAsync();
        if (vid == null)
        {
            TempData["MessageOrderContent"] = $"No food can be added to the order right now!";
            return RedirectToAction(nameof(Index), new { oid = newOrderContent.OrderId });
        }
        return View(newOrderContent);
    }

    // GET: OrderContent/Edit/5
    public async Task<IActionResult> Edit(int? oid, int? id)
    {
        if (id == null || _context.OrderContents == null || _context.Users == null)
        {
            return NotFound();
        }
        var CurrentCustomer = await _context.Users
            .Where(q => q.UserName == User!.Identity!.Name)
            .FirstOrDefaultAsync();
        if (CurrentCustomer is null)
            return BadRequest();

        var ExistentOrderContent = await _context.OrderContents
            .Include(t => t.Food)
            .Include(t => t.Order)
            .FirstOrDefaultAsync(q => q.OrderId == oid && q.FoodId == id && q.Order!.CustomerId == CurrentCustomer.Id);
        if (ExistentOrderContent == null)
        {
            return NotFound();
        }
        var output = _mapper.Map<ExistentOrderContentDto>(ExistentOrderContent);
        return View(output);
    }

    // POST: OrderContent/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit([FromQuery] int oid, [FromRoute] int id, [FromForm] ExistentOrderContentDto ModifiedOrderContent)
    {
        if (ModifiedOrderContent is null || id != ModifiedOrderContent.FoodId)
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
                    TempData["MessageOrderContent"] = $"The food is not available anymore!";
                    return RedirectToAction(nameof(Index), new { oid = ModifiedOrderContent.OrderId });
                }

                var ExistentOrderContent = await _context.OrderContents
                    .FirstOrDefaultAsync(q => q.FoodId == id && q.Order!.CustomerId == CurrentCustomer.Id);
                if (ExistentOrderContent == null)
                {
                    return NotFound();
                }

                if (resource.Stock < ModifiedOrderContent.Quantity - ExistentOrderContent.Quantity)
                {
                    TempData["MessageOrderContent"] = $"The change to the order ({ModifiedOrderContent.Quantity - ExistentOrderContent.Quantity}) is bigger than what we have in stock right now ({resource.Stock})!";
                    return RedirectToAction(nameof(Index), new { oid = ModifiedOrderContent.OrderId });
                }

                var reducedFood = await _context.Foods
                               .Where(i => i.Id == ModifiedOrderContent.FoodId)
                               .FirstOrDefaultAsync();
                reducedFood!.Stock -= ModifiedOrderContent.Quantity - ExistentOrderContent.Quantity;
                _context.Update(reducedFood);
                await _context.SaveChangesAsync();

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
                    .Where(q => q.FoodId == ModifiedOrderContent.FoodId && q.Food!.Stock < q.Quantity)
                    .ToListAsync();
                foreach (var item in reducedShopCarts)
                {
                    item.Quantity = item.Food!.Stock;
                    _context.ShopCarts.Update(item);
                    await _context.SaveChangesAsync();
                }

                ModifiedOrderContent.ToEntity(ref ExistentOrderContent);
                _context.Update(ExistentOrderContent);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderContentExists(ModifiedOrderContent.OrderId, ModifiedOrderContent.CustomerId, ModifiedOrderContent.FoodId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index), new { oid = ModifiedOrderContent.OrderId });
        }

        var CurrenCustomer = await _context.Users
            .Where(q => q.UserName == User!.Identity!.Name)
            .FirstOrDefaultAsync();
        if (CurrenCustomer is null)
            return BadRequest();

        var ExistenOrderContent = await _context.OrderContents
            .Include(t => t.Food)
            .Include(t => t.Order)
            .FirstOrDefaultAsync(q => q.FoodId == id && q.Order!.CustomerId == CurrenCustomer.Id);
        if (ExistenOrderContent == null)
        {
            return NotFound();
        }
        var output = _mapper.Map<ExistentOrderContentDto>(ExistenOrderContent);
        return View(output);
    }

    // GET: OrderContent/Delete/5
    public async Task<IActionResult> Delete(int? oid, int? id)
    {
        if (id == null || _context.OrderContents == null)
        {
            return NotFound();
        }
        var CurrentCustomer = await _context.Users
                    .Where(q => q.UserName == User!.Identity!.Name)
                    .FirstOrDefaultAsync();
        if (CurrentCustomer is null)
            return BadRequest();

        var ordercontent = await _context.OrderContents
            .Include(c => c.Order)
            .Include(c => c.Food)
            .FirstOrDefaultAsync(m => m.OrderId == oid && m.FoodId == id && m.Order!.CustomerId == CurrentCustomer.Id);
        if (ordercontent == null)
        {
            return NotFound();
        }

        var Customers = await _context.OrderContents
            .Include(t => t.Order)
            .Where(c => c.FoodId == id && c.Order!.CustomerId == CurrentCustomer.Id)
            .ToListAsync();
        var output = _mapper.Map<OrderContentDetailsVm>(ordercontent);
        var customers = "";
        foreach (var customer in Customers)
        {
            customers += $"{customer.Order!.Customer!.UserName}";
        }
        output.Customer = string.IsNullOrEmpty(customers) ? "-" : customers.Substring(0, customers.Length);
        return View(output);
    }

    // POST: OrderContent/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int oid, int id)
    {
        var CurrentUser = await _userManager.FindByEmailAsync(User.Identity!.Name!);
        if (CurrentUser == null)
            return BadRequest();

        if (_context.OrderContents == null)
        {
            return Problem("Entity set 'ApplicationDbContext.OrderContents' is null.");
        }

        var ordercontent = await _context.OrderContents.FindAsync(oid, id);
        if (ordercontent != null)
        {
            _context.OrderContents.Remove(ordercontent);
            await _context.SaveChangesAsync();

            var reducedFood = await _context.Foods
                               .Where(i => i.Id == ordercontent.FoodId)
                               .FirstOrDefaultAsync();
            reducedFood!.Stock += ordercontent.Quantity;
            _context.Update(reducedFood);
            await _context.SaveChangesAsync();

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
                .Where(q => q.FoodId == ordercontent.FoodId && q.Food!.Stock < q.Quantity)
                .ToListAsync();
            foreach (var item in reducedShopCarts)
            {
                item.Quantity = item.Food!.Stock;
                _context.ShopCarts.Update(item);
                await _context.SaveChangesAsync();
            }
        }
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index), new { oid = ordercontent!.OrderId });
    }
    */

    private bool OrderContentExists(int oid, int uid, int vid)
    {
      return (_context.OrderContents?.Any(e => e.OrderId == oid && e.Order!.CustomerId == uid && e.FoodId == vid)).GetValueOrDefault();
    }

    private bool OrderContentOwned(int uid)
    {
      return (_context.OrderContents?.Any(e => e.Order!.CustomerId == uid)).GetValueOrDefault();
    }
}