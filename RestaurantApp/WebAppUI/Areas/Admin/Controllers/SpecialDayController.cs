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
public class SpecialDayController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly UserManager<AppUser> _userManager;

    public SpecialDayController(ApplicationDbContext context, IMapper mapper, UserManager<AppUser> userManager)
    {
        _context = context;
        _mapper = mapper;
        _userManager = userManager;
    }

    // GET: Admin/SpecialDay
    public async Task<IActionResult> Index()
    {
        if (_context.SpecialDays == null)
            return Problem("Entity set 'ApplicationDbContext.SpecialDays' is null.");
        return View(_mapper.Map<List<ExistentSpecialDayDto>>(await _context.SpecialDays.ToListAsync()));
    }

    // GET: Admin/SpecialDay/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || _context.SpecialDays == null)
        {
            return NotFound();
        }

        var ExistentSpecialDay = _mapper.Map<ExistentSpecialDayDto>(await _context.SpecialDays.FirstOrDefaultAsync(m => m.Id == id));
        if (ExistentSpecialDay == null)
        {
            return NotFound();
        }

        return View(ExistentSpecialDay);
    }

    // GET: Admin/SpecialDay/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Admin/SpecialDay/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([FromForm] NewSpecialDayDto newSpecialDay)
    {
        if (ModelState.IsValid)
        {
            var specialDays = await _context.SpecialDays
                                        .Where(i => i.Day == newSpecialDay.Day && i.Month == newSpecialDay.Month)
                                        .ToListAsync();
            if (specialDays.Any())
            {
                TempData["MessageSpecialDay"] = $"There is already an entry for <strong>{newSpecialDay.Day}/{newSpecialDay.Month}</strong>!";
                return RedirectToAction(nameof(Index));
            }
            var specialDay = _mapper.Map<SpecialDay>(newSpecialDay);
            _context.Add(specialDay);
            await _context.SaveChangesAsync();
            TempData["MessageSpecialDay"] = $"We added the special day <strong>{newSpecialDay.Day}/{newSpecialDay.Month}</strong>!";
            return RedirectToAction(nameof(Index));
        }
        return View(newSpecialDay);
    }

    // GET: Admin/SpecialDay/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || _context.SpecialDays == null)
        {
            return NotFound();
        }

        var ExistentSpecialDay = _mapper.Map<ExistentSpecialDayDto>(await _context.SpecialDays.FindAsync(id));
        if (ExistentSpecialDay == null)
        {
            return NotFound();
        }
        return View(ExistentSpecialDay);
    }

    // POST: Admin/SpecialDay/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [FromForm] ExistentSpecialDayDto modifiedSpecialDay)
    {
        if (id != modifiedSpecialDay.Id)
        {
            return NotFound();
        }
        var ExistentSpecialDay = await _context.SpecialDays.FindAsync(id);
        if (ExistentSpecialDay == null)
        {
            return NotFound();
        }
        if (ExistentSpecialDay.Id != modifiedSpecialDay.Id)
        {
            return BadRequest();
        }
        if (ModelState.IsValid)
        {
            try
            {
                var specialDays = await _context.SpecialDays
                                        .Where(i => i.Day == modifiedSpecialDay.Day && i.Month == modifiedSpecialDay.Month && i.Id!=id)
                                        .ToListAsync();
                if (specialDays.Any())
                {
                    TempData["MessageSpecialDay"] = $"There is already an entry for <strong>{modifiedSpecialDay.Day}/{modifiedSpecialDay.Month}</strong>!";
                    return RedirectToAction(nameof(Index));
                }

                TempData["MessageSpecialDay"] = $"We modified work hours for <strong>{ExistentSpecialDay.Day}/{ExistentSpecialDay.Month}</strong>!";
                modifiedSpecialDay.ToEntity(ref ExistentSpecialDay);
                _context.Update(ExistentSpecialDay);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SpecialDayExists(ExistentSpecialDay.Id))
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
        return View(modifiedSpecialDay);
    }

    // GET: Admin/SpecialDay/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null || _context.SpecialDays == null)
        {
            return NotFound();
        }

        var ExistentSpecialDay = _mapper.Map<ExistentSpecialDayDto>(await _context.SpecialDays.FirstOrDefaultAsync(m => m.Id == id));
        if (ExistentSpecialDay == null)
        {
            return NotFound();
        }

        return View(ExistentSpecialDay);
    }

    // POST: Admin/SpecialDay/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (_context.SpecialDays == null)
        {
            return Problem("Entity set 'ApplicationDbContext.SpecialDays' is null.");
        }
        var ExistentSpecialDay = await _context.SpecialDays
            .FirstOrDefaultAsync(i => i.Id == id);
        if (ExistentSpecialDay == null)
        {
            return NotFound();
        }
        else
        {
            _context.SpecialDays.Remove(ExistentSpecialDay);
            TempData["MessageSpecialDay"] = $"Special Day <strong>{ExistentSpecialDay.Day}/{ExistentSpecialDay.Month}</strong> was successfully deleted!";
        }
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool SpecialDayExists(int id)
    {
        return (_context.SpecialDays?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}