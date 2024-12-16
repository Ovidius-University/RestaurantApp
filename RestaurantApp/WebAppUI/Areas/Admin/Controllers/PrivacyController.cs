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
public class PrivacyController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly UserManager<AppUser> _userManager;

    public PrivacyController(ApplicationDbContext context, IMapper mapper, UserManager<AppUser> userManager)
    {
        _context = context;
        _mapper = mapper;
        _userManager = userManager;
    }

    // GET: Admin/Privacy
    public async Task<IActionResult> Index()
    {
        if (_context.Privacies == null)
            return Problem("Entity set 'ApplicationDbContext.Privacies' is null.");
        return View(_mapper.Map<List<ExistentPrivacyDto>>(await _context.Privacies.ToListAsync()));
    }

    // GET: Admin/Privacy/Details/5
    public async Task<IActionResult> Details(int?id)
    {
        if (_context.Privacies == null || id == null)
        {
            return NotFound();
        }

        var ExistentPrivacy = _mapper.Map<ExistentPrivacyDto>(await _context.Privacies.FirstOrDefaultAsync(i => i.Id == id));
        if (ExistentPrivacy == null)
        {
            return NotFound();
        }

        return View(ExistentPrivacy);
    }

    // GET: Admin/Privacy/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (_context.Privacies == null)
        {
            return NotFound();
        }

        var ExistentPrivacy = _mapper.Map<ExistentPrivacyDto>(await _context.Privacies.FirstOrDefaultAsync(i => i.Id == id));
        /*if (ExistentPrivacy == null)
        {
            return NotFound();
        }*/
        return View(ExistentPrivacy);
    }

    // POST: Admin/Privacy/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [FromForm] ExistentPrivacyDto modifiedPrivacy)
    {
        var ExistentPrivacy = await _context.Privacies.FirstOrDefaultAsync(i => i.Id == id);
        /*if (ExistentPrivacy == null)
        {
            return NotFound();
        }*/
        if (ModelState.IsValid)
        {
            try
            {if (ExistentPrivacy != null)
                {
                    TempData["MessagePrivacy"] = $"We modified the Privacy Policy of the website!";
                    modifiedPrivacy.ToEntity(ref ExistentPrivacy);
                    _context.Update(ExistentPrivacy);
                }
             else
                {
                    TempData["MessagePrivacy"] = $"We added the Privacy Policy of the website!";
                    var newPrivacy = _mapper.Map<Privacy>(modifiedPrivacy);
                    _context.Add(newPrivacy);
                }
             await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PrivacyExists())
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
        return View(modifiedPrivacy);
    }

    private bool PrivacyExists()
    {
        return (_context.Privacies?.Any()).GetValueOrDefault();
    }
}