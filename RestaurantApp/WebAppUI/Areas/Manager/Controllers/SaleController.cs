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
using WebAppUI.Areas.Manager.Models.ViewModels;
using WebAppUI.Areas.Manager.Models.DTOs;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using System.Drawing;

namespace WebAppUI.Areas.Manager.Controllers;

[Area("Manager")]
[Authorize(Roles = "Manager")]

public class SaleController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly UserManager<AppUser> _userManager;

    public SaleController(ApplicationDbContext context, IMapper mapper, UserManager<AppUser> userManager)
    {
        _context = context;
        _mapper = mapper;
        _userManager = userManager;
    }

    [Route("Manager/Sales")]
    public async Task<IActionResult> Index()
    {
        var CurrentProvider = await _context.ProviderManagers
            .Include(t => t.Provider)
            .Include(t => t.Manager)
            .FirstOrDefaultAsync(q => q.Manager!.UserName == User.Identity!.Name);
        if (CurrentProvider == null)
            return Problem("You haven't been assigned to a provider yet!");

        var orders = await _context.Orders
            .Include(c => c.OrderContents)
            .Include(c => c.Customer)
            .Where(c => c.IsFinal)
            .ToListAsync();

        var ordercontents = _mapper.Map<List<OrderContentVm>>(
            await _context.OrderContents
            .Include(c => c.Order)
            .Include(c => c.Order!.Customer)
            .Include(c => c.Food)
            .Include(c => c.Food!.Offer)
            .Where(c => c.Food!.IsHomeMade == false && c.Food.ProviderId == CurrentProvider.ProviderId)
            .ToListAsync());

        ordercontents = ordercontents
            .Where(c => orders.Any(j => j.Id == c.OrderId))
            .ToList();

        var query = ordercontents.GroupBy(name => new { name.FoodId, name.Food!.Title })
            .Select(sale => new
            {
                FoodId = sale.Key.FoodId,
                Title = sale.Key.Title,
                Quantity = sale.Sum(name => name.Quantity),
                TotalSales = sale.Sum(name => name.TotalPrice)
            });

        var sales = new List<SaleVm> ();

        foreach (var item in query)
        {
            sales.Add(new SaleVm()
            {
                FoodId = item.FoodId,
                Title = item.Title,
                Quantity = item.Quantity,
                TotalSales = item.TotalSales
            });
        }
        
        var output = new IndexSalesVm()
        {
            Provider = CurrentProvider.Provider!.Name,
            ListSales = sales
        };
        return View(output);
    }

    // GET: Sale/DetailsItem/5
    [Route("Manager/Sale/DetailsItem")]
    public async Task<IActionResult> DetailsItem(int? id)
    {
        var CurrentProvider = await _context.ProviderManagers
            .Include(t => t.Provider)
            .Include(t => t.Manager)
            .FirstOrDefaultAsync(q => q.Manager!.UserName == User.Identity!.Name);
        if (CurrentProvider == null)
            return Problem("You haven't been assigned to a provider yet!");

        var orders = await _context.Orders
            .Include(c => c.OrderContents)
            .Include(c => c.Customer)
            .Where(c => c.IsFinal)
            .ToListAsync();

        var ordercontents = _mapper.Map<List<OrderContentVm>>(
            await _context.OrderContents
            .Include(c => c.Order)
            .Include(c => c.Order!.Customer)
            .Include(c => c.Food)
            .Include(c => c.Food!.Offer)
            .Where(c => c.Food!.IsHomeMade == false && c.Food.ProviderId == CurrentProvider.ProviderId && c.FoodId == id)
            .ToListAsync());

        ordercontents = ordercontents
            .Where(c => orders.Any(j => j.Id == c.OrderId))
            .ToList();

        var query = ordercontents.GroupBy(name => new { name.FoodId, name.Food!.Title })
            .Select(sale => new
            {
                FoodId = sale.Key.FoodId,
                Title = sale.Key.Title,
                Quantity = sale.Sum(name => name.Quantity),
                TotalSales = sale.Sum(name => name.TotalPrice)
            });

        var sales = new SaleVm();
        foreach (var item in query)
        {
            sales = new SaleVm()
            {
                FoodId = item.FoodId,
                Title = item.Title,
                Quantity = item.Quantity,
                TotalSales = item.TotalSales
            };
        }

        return View(sales);
    }

    // GET: Sale/DetailsTime/5
    [Route("Manager/Sales/DetailsTime")]
    public async Task<IActionResult> DetailsTime(int? id)
    {
        var CurrentProvider = await _context.ProviderManagers
            .Include(t => t.Provider)
            .Include(t => t.Manager)
            .FirstOrDefaultAsync(q => q.Manager!.UserName == User.Identity!.Name);
        if (CurrentProvider == null)
            return Problem("You haven't been assigned to a provider yet!");

        var orders = await _context.Orders
            .Include(c => c.OrderContents)
            .Include(c => c.Customer)
            .Where(c => c.IsFinal)
            .ToListAsync();

        if (id == 1)
        {
            orders = orders
            .Where(c => c.IsFinal ? c.DeliveryTime.AddDays(1) >= DateTime.Now : c.ArrivalTime.AddDays(1) >= DateTime.Now)
            .ToList();
        }
        else if (id == 2)
        {
            orders = orders
            .Where(c => c.IsFinal ? c.DeliveryTime.AddMonths(1) >= DateTime.Now : c.ArrivalTime.AddMonths(1) >= DateTime.Now)
            .ToList();
        }
        else
        {
            orders = orders
            .Where(c => c.IsFinal ? c.DeliveryTime.AddYears(1) >= DateTime.Now : c.ArrivalTime.AddYears(1) >= DateTime.Now)
            .ToList();
        }

        var ordercontents = _mapper.Map<List<OrderContentVm>>(
            await _context.OrderContents
            .Include(c => c.Order)
            .Include(c => c.Order!.Customer)
            .Include(c => c.Food)
            .Include(c => c.Food!.Offer)
            .Where(c => c.Food!.IsHomeMade == false && c.Food.ProviderId == CurrentProvider.ProviderId)
            .ToListAsync());

        ordercontents = ordercontents
            .Where(c => orders.Any(j => j.Id == c.OrderId))
            .ToList();

        var query = ordercontents.GroupBy(name => new { name.FoodId, name.Food!.Title })
            .Select(sale => new
            {
                FoodId = sale.Key.FoodId,
                Title = sale.Key.Title,
                Quantity = sale.Sum(name => name.Quantity),
                TotalSales = sale.Sum(name => name.TotalPrice)
            });

        var sales = new List<SaleVm>();

        foreach (var item in query)
        {
            sales.Add(new SaleVm()
            {
                FoodId = item.FoodId,
                Title = item.Title,
                Quantity = item.Quantity,
                TotalSales = item.TotalSales
            });
        }

        var output = new SaleTimeVm()
        {
            id = (int)id!,
            Provider = CurrentProvider.Provider!.Name,
            ListSales = sales
        };
        return View(output);
    }

    // GET: Sale/DetailsTimeItem/5
    [Route("Manager/Sales/DetailsTimeItem")]
    public async Task<IActionResult> DetailsTimeItem(int? id, int? tid)
    {
        var CurrentProvider = await _context.ProviderManagers
            .Include(t => t.Provider)
            .Include(t => t.Manager)
            .FirstOrDefaultAsync(q => q.Manager!.UserName == User.Identity!.Name);
        if (CurrentProvider == null)
            return Problem("You haven't been assigned to a provider yet!");

        var orders = await _context.Orders
            .Include(c => c.OrderContents)
            .Include(c => c.Customer)
            .Where(c => c.IsFinal)
            .ToListAsync();

        if (tid == 1)
        {
            orders = orders
            .Where(c => c.IsFinal ? c.DeliveryTime.AddDays(1) >= DateTime.Now : c.ArrivalTime.AddDays(1) >= DateTime.Now)
            .ToList();
        }
        else if (tid == 2)
        {
            orders = orders
            .Where(c => c.IsFinal ? c.DeliveryTime.AddMonths(1) >= DateTime.Now : c.ArrivalTime.AddMonths(1) >= DateTime.Now)
            .ToList();
        }
        else
        {
            orders = orders
            .Where(c => c.IsFinal ? c.DeliveryTime.AddYears(1) >= DateTime.Now : c.ArrivalTime.AddYears(1) >= DateTime.Now)
            .ToList();
        }

        var ordercontents = _mapper.Map<List<OrderContentVm>>(
            await _context.OrderContents
            .Include(c => c.Order)
            .Include(c => c.Order!.Customer)
            .Include(c => c.Food)
            .Include(c => c.Food!.Offer)
            .Where(c => c.Food!.IsHomeMade == false && c.Food.ProviderId == CurrentProvider.ProviderId && c.FoodId == id)
            .ToListAsync());

        ordercontents = ordercontents
            .Where(c => orders.Any(j => j.Id == c.OrderId))
            .ToList();

        var query = ordercontents.GroupBy(name => new { name.FoodId, name.Food!.Title })
            .Select(sale => new
            {
                FoodId = sale.Key.FoodId,
                Title = sale.Key.Title,
                Quantity = sale.Sum(name => name.Quantity),
                TotalSales = sale.Sum(name => name.TotalPrice)
            });

        var sales = new SaleTimeItemVm();

        foreach (var item in query)
        {
            sales = new SaleTimeItemVm()
            {
                id = (int)tid!,
                FoodId = item.FoodId,
                Title = item.Title,
                Quantity = item.Quantity,
                TotalSales = item.TotalSales
            };
        }

        return View(sales);
    }
}
