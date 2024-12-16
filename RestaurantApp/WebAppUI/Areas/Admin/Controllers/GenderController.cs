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
public class GenderController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly UserManager<AppUser> _userManager;

    public GenderController(ApplicationDbContext context, IMapper mapper, UserManager<AppUser> userManager)
    {
        _context = context;
        _mapper = mapper;
        _userManager = userManager;
    }

    // GET: Admin/Gender
    public async Task<IActionResult> Index()
    {
        if (_context.Genders == null)
            return Problem("Entity set 'ApplicationDbContext.Genders' is null.");
        return View(_mapper.Map<List<ExistentGenderDto>>(await _context.Genders.ToListAsync()));
    }

    // GET: Admin/Gender/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || _context.Genders == null)
        {
            return NotFound();
        }

        var ExistentGender = _mapper.Map<ExistentGenderDto>(await _context.Genders.FirstOrDefaultAsync(m => m.Id == id));
        if (ExistentGender == null)
        {
            return NotFound();
        }

        return View(ExistentGender);
    }

    // GET: Admin/Gender/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Admin/Gender/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([FromForm] NewGenderDto newGender)
    {
        if (ModelState.IsValid)
        {
            var history = await _context.Genders
                .Where(i => i.Name == newGender.Name)
                .FirstOrDefaultAsync();
            if (history != null)
            {
                TempData["MessageGender"] = $"The gender <strong>{newGender.Name}</strong> is already added!";
                return RedirectToAction(nameof(Index));
            }
            var gender = _mapper.Map<Gender>(newGender);
            _context.Add(gender);
            await _context.SaveChangesAsync();
            TempData["MessageGender"] = $"We added the gender <strong>{newGender.Name}</strong>!";
            return RedirectToAction(nameof(Index));
        }
        return View(newGender);
    }

    // GET: Admin/Gender/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || _context.Genders == null)
        {
            return NotFound();
        }

        var ExistentGender = _mapper.Map<ExistentGenderDto>(await _context.Genders.FindAsync(id));
        if (ExistentGender == null)
        {
            return NotFound();
        }
        return View(ExistentGender);
    }

    // POST: Admin/Gender/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [FromForm] ExistentGenderDto modifiedGender)
    {
        if (id != modifiedGender.Id)
        {
            return NotFound();
        }
        var ExistentGender = await _context.Genders.FindAsync(id);
        if (ExistentGender == null)
        {
            return NotFound();
        }
        if (ExistentGender.Id != modifiedGender.Id)
        {
            return BadRequest();
        }

        var history = await _context.Genders
                .Where(i => i.Name == modifiedGender.Name && i.Id!=id)
                .FirstOrDefaultAsync();
            if (history != null)
            {
                TempData["MessageGender"] = $"We already have the gender <strong>{modifiedGender.Name}</strong>!";
                return RedirectToAction(nameof(Index));
            }
        if (ModelState.IsValid)
        {
            try
            {
                TempData["MessageGender"] = $"We modified gender <strong>{ExistentGender.Name}</strong> in <strong>{modifiedGender.Name}</strong>!";
                ExistentGender.Name = modifiedGender.Name;
                _context.Update(ExistentGender);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GenderExists(ExistentGender.Id))
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
        return View(modifiedGender);
    }

    // GET: Admin/Gender/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null || _context.Genders == null)
        {
            return NotFound();
        }

        var ExistentGender = _mapper.Map<ExistentGenderDto>(await _context.Genders.FirstOrDefaultAsync(m => m.Id == id));
        if (ExistentGender == null)
        {
            return NotFound();
        }

        return View(ExistentGender);
    }

    // POST: Admin/Gender/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (_context.Genders == null)
        {
            return Problem("Entity set 'ApplicationDbContext.Genders' is null.");
        }
        var ExistentGender = await _context.Genders
            .FirstOrDefaultAsync(i => i.Id == id);
        if (ExistentGender == null)
        {
            return NotFound();
        }
        TempData["MessageGender"] = $"Gender <strong>{ExistentGender.Name}</strong> was successfully deleted!";
        _context.Genders.Remove(ExistentGender);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool GenderExists(int id)
    {
        return (_context.Genders?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}