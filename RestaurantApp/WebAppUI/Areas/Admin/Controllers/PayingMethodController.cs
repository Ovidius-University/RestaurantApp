using WebAppUI.Models.CustomIdentity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using WebAppUI.Areas.Admin.Models.DTOs;
using WebAppUI.Areas.Admin.Models.ViewModels;
namespace WebAppUI.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Administrator")]
public class PayingMethodController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly UserManager<AppUser> _userManager;

    public PayingMethodController(ApplicationDbContext context, IMapper mapper, UserManager<AppUser> userManager)
    {
        _context = context;
        _mapper = mapper;
        _userManager = userManager;
    }

    // GET: Admin/PayingMethod
    public async Task<IActionResult> Index()
    {
        if (_context.PayingMethods == null)
            return Problem("Entity set 'ApplicationDbContext.PayingMethods' is null.");
        return View(_mapper.Map<List<ExistentPayingMethodDto>>(await _context.PayingMethods.ToListAsync()));
    }

    // GET: Admin/PayingMethod/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || _context.PayingMethods == null)
        {
            return NotFound();
        }

        var ExistentPayingMethod = _mapper.Map<ExistentPayingMethodDto>(await _context.PayingMethods.FirstOrDefaultAsync(m => m.Id == id));
        if (ExistentPayingMethod == null)
        {
            return NotFound();
        }

        return View(ExistentPayingMethod);
    }

    // GET: Admin/PayingMethod/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Admin/PayingMethod/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([FromForm] NewPayingMethodDto newPayingMethod)
    {
        if (ModelState.IsValid)
        {
            var history = await _context.PayingMethods
                .Where(i => i.Name == newPayingMethod.Name)
                .FirstOrDefaultAsync();
            if (history != null)
            {
                TempData["MessagePayingMethod"] = $"The paying method <strong>{newPayingMethod.Name}</strong> is already added!";
                return RedirectToAction(nameof(Index));
            }

            var payingMethod = _mapper.Map<PayingMethod>(newPayingMethod);
            _context.Add(payingMethod);
            await _context.SaveChangesAsync();
            TempData["MessagePayingMethod"] = $"We added the paying method <strong>{newPayingMethod.Name}</strong>!";
            return RedirectToAction(nameof(Index));
        }
        return View(newPayingMethod);
    }

    // GET: Admin/PayingMethod/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || _context.PayingMethods == null)
        {
            return NotFound();
        }

        var ExistentPayingMethod = _mapper.Map<ExistentPayingMethodDto>(await _context.PayingMethods.FindAsync(id));
        if (ExistentPayingMethod == null)
        {
            return NotFound();
        }
        return View(ExistentPayingMethod);
    }

    // POST: Admin/PayingMethod/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [FromForm] ExistentPayingMethodDto modifiedPayingMethod)
    {
        if (id != modifiedPayingMethod.Id)
        {
            return NotFound();
        }
        var ExistentPayingMethod = await _context.PayingMethods.FindAsync(id);
        if (ExistentPayingMethod == null)
        {
            return NotFound();
        }
        if (ExistentPayingMethod.Id != modifiedPayingMethod.Id)
        {
            return BadRequest();
        }
        var history = await _context.PayingMethods
                .Where(i => i.Name == modifiedPayingMethod.Name && i.Id!=id)
                .FirstOrDefaultAsync();
        if (history != null)
        {
            TempData["MessagePayingMethod"] = $"The paying method <strong>{modifiedPayingMethod.Name}</strong> already exists!";
            return RedirectToAction(nameof(Index));
        }

        if (ModelState.IsValid)
        {
            try
            {
                TempData["MessagePayingMethod"] = $"We modified paying method <strong>{ExistentPayingMethod.Name}</strong> in <strong>{modifiedPayingMethod.Name}</strong>!";
                ExistentPayingMethod.Name = modifiedPayingMethod.Name;
                _context.Update(ExistentPayingMethod);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PayingMethodExists(ExistentPayingMethod.Id))
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
        return View(modifiedPayingMethod);
    }

    // GET: Admin/PayingMethod/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null || _context.PayingMethods == null)
        {
            return NotFound();
        }

        var ExistentPayingMethod = _mapper.Map<ExistentPayingMethodDto>(await _context.PayingMethods.FirstOrDefaultAsync(m => m.Id == id));
        if (ExistentPayingMethod == null)
        {
            return NotFound();
        }

        return View(ExistentPayingMethod);
    }

    // POST: Admin/PayingMethod/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (_context.PayingMethods == null)
        {
            return Problem("Entity set 'ApplicationDbContext.PayingMethods' is null.");
        }
        var ExistentPayingMethod = await _context.PayingMethods
            .Include(t => t.Orders)
            .FirstOrDefaultAsync(i => i.Id == id);
        if (ExistentPayingMethod == null)
        {
            return NotFound();
        }
        else
        {
            if (ExistentPayingMethod.Orders!.Count == 0)
            {
                _context.PayingMethods.Remove(ExistentPayingMethod);
                TempData["MessagePayingMethod"] = $"Paying method <strong>{ExistentPayingMethod.Name}</strong> was successfully deleted!";
            }
            else
            {
                TempData["MessagePayingMethod"] = $"Paying method <strong>{ExistentPayingMethod.Name}</strong> can not be deleted as it has associated orders!";
            }
        }
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    /*
    // GET: Admin/PayingMethod/Edit/5
    public async Task<IActionResult> Orders(int id)
    {
        if (_context.PayingMethods == null)
        {
            return NotFound();
        }

        var ExistentPayingMethod = await _context.PayingMethods
            .Include(t => t.Orders)
            .FirstOrDefaultAsync(i => i.Id == id);
        if (ExistentPayingMethod is null)
        {
            return NotFound();
        }
        var Orders = _mapper.Map<List<ExistentUserDto>>(await _userManager.GetUsersInRoleAsync("Order"));

        var associatedOrders = await _context.PayingMethodOrders.Where(c => c.PayingMethodId == id).ToListAsync();

        for (var i = Orders.Count - 1; i >= 0; i--)
        {
            if (!associatedOrders.Any(m => m.OrderId == Orders[i].Id))
            {
                Orders.Remove(Orders[i]);
            }
        }
        var output = new PayingMethodOrdersVm()
        {
            Id = id,
            Name = ExistentPayingMethod.Name,
            listOrders = Orders
        };
        return View(output);
    }

    // GET: Admin/PayingMethod/DeleteOrder/5
    public async Task<IActionResult> DeleteOrder(int? id, int? pub, string? email)
    {
        if (id == null || pub == null || email == null || _context.PayingMethodOrders == null || _context.PayingMethods == null)
        {
            return NotFound();
        }

        var ExistentOrder = await _context.PayingMethodOrders
            .FirstOrDefaultAsync(i => i.OrderId == id && i.PayingMethodId == pub); 
        if (ExistentOrder == null)
        {
            return NotFound();
        }

        var BasicOrder = await _userManager.FindByEmailAsync(email);
        ExistentOrderDto orderDto = new();
        if (BasicOrder == null || BasicOrder.Email==null)
        {
            return NotFound();
        }
        orderDto.Id = BasicOrder.Id;
        orderDto.Email = BasicOrder.Email;

        return View(orderDto);
    }

    // POST: Admin/PayingMethod/DeleteOrderConfirmed/5
    [HttpPost, ActionName("DeleteOrder")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteOrderConfirmed(int id)
    {
        if (_context.PayingMethodOrders == null)
        {
            return Problem("Entity set 'ApplicationDbContext.PayingMethodOrders' is null.");
        }
        var ExistentOrder = await _context.PayingMethodOrders
            .FirstOrDefaultAsync(i => i.OrderId == id);
        var order = await _userManager.Users.FirstOrDefaultAsync(i => i.Id == id);
        if (ExistentOrder == null || order == null)
        {
            return NotFound();
        }
        else
        {
            _context.PayingMethodOrders.Remove(ExistentOrder);
            TempData["MessagePayingMethod"] = $"Order <strong>{Order.UserName}</strong> was successfully disassociated from the payingMethod!";
        }
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    */

    private bool PayingMethodExists(int id)
    {
        return (_context.PayingMethods?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}