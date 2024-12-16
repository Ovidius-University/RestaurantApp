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
using System.Drawing;

namespace WebAppUI.Areas.Admin.Controllers;
[Area("Admin")]
[Authorize(Roles = "Administrator")]

public class EmployeeWorkController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly UserManager<AppUser> _userManager;

    public EmployeeWorkController(ApplicationDbContext context, IMapper mapper, UserManager<AppUser> userManager)
    {
        _context = context;
        _mapper = mapper;
        _userManager = userManager;
    }

    [Route("Admin/EmployeeWorks")]
    public async Task<IActionResult> Index()
    {
        var orders = await _context.Orders
            .Where(c => c.WorkerId != null)
            .ToListAsync();

        var query = orders.GroupBy(name => new { name.WorkerId })
            .Select(work => new
            {
                WorkerId = work.Key.WorkerId,
                Orders = work.Count()
            })
            .OrderBy(item => item.WorkerId);

        var works = new List<WorkVm>();

        foreach (var item in query)
        {   
            var empSched = await _context.EmployeeSchedules
                        .Include(i => i.Employee)
                        .Include(i => i.Day)
                        .Where(i => i.EmployeeId == item.WorkerId!)
                        .FirstOrDefaultAsync();

            var email = empSched!.Employee!.Email;

            works.Add(new WorkVm()
            {
                WorkerId = (int)item.WorkerId!,
                Email = email!,
                Orders = item.Orders
            });
        }
        
        var output = new IndexWorkVm()
        {
            ListWorks = works
        };
        return View(output);
    }

    // GET: EmployeeWork/OrdersEmp/5
    [Route("Admin/EmployeeWork/OrdersEmp")]
    public async Task<IActionResult> OrdersEmp(int? id)
    {
        var orders = await _context.Orders
            .Include(c => c.Customer)
            .Include(c => c.DeliveryName)
            .Include(c => c.PayingMethod)
            .Include(c => c.OrderContents)
            .Include(c => c.FeedBack)
            .Where(c => c.WorkerId == id)
            .OrderBy(c => c.IsFinal)
            .ThenByDescending(c => c.DeliveryTime)
            .ThenBy(c => c.ArrivalTime)
            .ThenBy(c => c.Id)
            .ToListAsync();

        var query = orders.GroupBy(name => new { name.WorkerId })
            .Select(work => new
            {
                WorkerId = work.Key.WorkerId,
                Orders = work.Count()
            })
            .OrderBy(item => item.WorkerId);

        var output = new IndexEmpWorkVm();

        foreach (var item in query)
        {
            var empSched = await _context.EmployeeSchedules
                        .Include(i => i.Employee)
                        .Include(i => i.Day)
                        .Where(i => i.EmployeeId == item.WorkerId!)
                        .FirstOrDefaultAsync();

            var email = empSched!.Employee!.Email;

            output.WorkerId = (int)item.WorkerId!;
            output.Email = email!;
            output.Orders = item.Orders;
        }

        var Orders = _mapper.Map<List<OrderVm>>(orders);

        output.ListOrders = Orders;
        return View(output);
    }

    // GET: EmployeeWork/OrdersTime/5
    [Route("Admin/EmployeeWork/OrdersTime")]
    public async Task<IActionResult> OrdersTime(int? id)
    {
        var orders = await _context.Orders
            .Where(c => c.WorkerId != null)
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

        var query = orders.GroupBy(name => new { name.WorkerId })
            .Select(work => new
            {
                WorkerId = work.Key.WorkerId,
                Orders = work.Count()
            })
            .OrderBy(item => item.WorkerId);

        var works = new List<WorkVm>();

        foreach (var item in query)
        {
            var empSched = await _context.EmployeeSchedules
                        .Include(i => i.Employee)
                        .Include(i => i.Day)
                        .Where(i => i.EmployeeId == item.WorkerId!)
                        .FirstOrDefaultAsync();

            var email = empSched!.Employee!.Email;

            works.Add(new WorkVm()
            {
                WorkerId = (int)item.WorkerId!,
                Email = email!,
                Orders = item.Orders
            });
        }

        var output = new WorkTimeVm()
        {
            id = (int)id!,
            ListWorks = works
        };
        return View(output);
    }

    // GET: EmployeeWork/OrdersTimeEmp/5
    [Route("Admin/EmployeeWork/OrdersTimeEmp")]
    public async Task<IActionResult> OrdersTimeEmp(int? id, int? tid)
    {
        var orders = await _context.Orders
            .Include(c => c.Customer)
            .Include(c => c.DeliveryName)
            .Include(c => c.PayingMethod)
            .Include(c => c.OrderContents)
            .Include(c => c.FeedBack)
            .Where(c => c.WorkerId == id)
            .OrderBy(c => c.IsFinal)
            .ThenByDescending(c => c.DeliveryTime)
            .ThenBy(c => c.ArrivalTime)
            .ThenBy(c => c.Id)
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

        var query = orders.GroupBy(name => new { name.WorkerId })
            .Select(work => new
            {
                WorkerId = work.Key.WorkerId,
                Orders = work.Count()
            })
            .OrderBy(item => item.WorkerId);

        var output = new EmpWorkTimeVm();

        foreach (var item in query)
        {
            var empSched = await _context.EmployeeSchedules
                        .Include(i => i.Employee)
                        .Include(i => i.Day)
                        .Where(i => i.EmployeeId == item.WorkerId!)
                        .FirstOrDefaultAsync();

            var email = empSched!.Employee!.Email;

            output.WorkerId = (int)item.WorkerId!;
            output.Email = email!;
            output.Orders = item.Orders;
        }

        var Orders = _mapper.Map<List<OrderVm>>(orders);

        output.ListOrders = Orders;
        output.id = (int)tid!;
        return View(output);
    }
}
