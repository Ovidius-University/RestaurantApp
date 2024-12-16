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
public class InformationController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly UserManager<AppUser> _userManager;

    public InformationController(ApplicationDbContext context, IMapper mapper, UserManager<AppUser> userManager)
    {
        _context = context;
        _mapper = mapper;
        _userManager = userManager;
    }

    // GET: Admin/Information
    public async Task<IActionResult> Index()
    {
        if (_context.Informations == null)
            return Problem("Entity set 'ApplicationDbContext.Informations' is null.");
        return View(_mapper.Map<List<ExistentInformationDto>>(await _context.Informations.ToListAsync()));
    }

    // GET: Admin/Information/Details/5
    public async Task<IActionResult> Details(int?id)
    {
        if (_context.Informations == null || id == null)
        {
            return NotFound();
        }

        var ExistentInformation = _mapper.Map<ExistentInformationDto>(await _context.Informations.FirstOrDefaultAsync(i => i.Id == id));
        if (ExistentInformation == null)
        {
            return NotFound();
        }

        return View(ExistentInformation);
    }

    // GET: Admin/Information/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (_context.Informations == null)
        {
            return NotFound();
        }

        var ExistentInformation = _mapper.Map<ExistentInformationDto>(await _context.Informations.FirstOrDefaultAsync(i => i.Id == id));
        /*if (ExistentInformation == null)
        {
            return NotFound();
        }*/
        return View(ExistentInformation);
    }

    // POST: Admin/Information/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [FromForm] ExistentInformationDto modifiedInformation)
    {
        var ExistentInformation = await _context.Informations.FirstOrDefaultAsync(i => i.Id == id);
        /*if (ExistentInformation == null)
        {
            return NotFound();
        }*/
        if (ModelState.IsValid)
        {
            try
            {if (ExistentInformation != null)
                {
                    TempData["MessageInformation"] = $"We modified the information of the restaurant!";
                    modifiedInformation.ToEntity(ref ExistentInformation);
                    _context.Update(ExistentInformation);
                }
             else
                {
                    TempData["MessageInformation"] = $"We added the information of the restaurant!";
                    var newInformation = _mapper.Map<Information>(modifiedInformation);
                    _context.Add(newInformation);
                }
             await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InformationExists())
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
        return View(modifiedInformation);
    }

    private bool InformationExists()
    {
        return (_context.Informations?.Any()).GetValueOrDefault();
    }
}