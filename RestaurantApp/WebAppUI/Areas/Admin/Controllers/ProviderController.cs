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
public class ProviderController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly UserManager<AppUser> _userManager;

    public ProviderController(ApplicationDbContext context, IMapper mapper, UserManager<AppUser> userManager)
    {
        _context = context;
        _mapper = mapper;
        _userManager = userManager;
    }

    // GET: Admin/Provider
    [Route("Admin/Providers")]
    public async Task<IActionResult> Index()
    {
        if (_context.Providers == null)
            return Problem("Entity set 'ApplicationDbContext.Providers' is null.");
        return View(_mapper.Map<List<ExistentProviderDto>>(await _context.Providers.ToListAsync()));
    }

    // GET: Admin/Provider/Details/5
    [Route("Admin/Provider/Details/{id}")]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || _context.Providers == null)
        {
            return NotFound();
        }

        var ExistentProvider = _mapper.Map<ExistentProviderDto>(await _context.Providers.FirstOrDefaultAsync(m => m.Id == id));
        if (ExistentProvider == null)
        {
            return NotFound();
        }

        return View(ExistentProvider);
    }

    // GET: Admin/Provider/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Admin/Provider/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([FromForm] NewProviderDto newProvider)
    {
        if (ModelState.IsValid)
        {
            var history = await _context.Providers
                .Where(i => i.Name == newProvider.Name)
                .FirstOrDefaultAsync();
            if (history != null)
            {
                TempData["MessageProvider"] = $"The provider <strong>{newProvider.Name}</strong> is already added!";
                return RedirectToAction(nameof(Index));
            }

            var provider = _mapper.Map<Provider>(newProvider);
            _context.Add(provider);
            await _context.SaveChangesAsync();
            TempData["MessageProvider"] = $"We added provider <strong>{newProvider.Name}</strong>!";
            return RedirectToAction(nameof(Index));
        }
        return View(newProvider);
    }

    // GET: Admin/Provider/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || _context.Providers == null)
        {
            return NotFound();
        }

        var ExistentProvider = _mapper.Map<ExistentProviderDto>(await _context.Providers.FindAsync(id));
        if (ExistentProvider == null)
        {
            return NotFound();
        }
        return View(ExistentProvider);
    }

    // POST: Admin/Provider/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [FromForm] ExistentProviderDto modifiedProvider)
    {
        if (id != modifiedProvider.Id)
        {
            return NotFound();
        }
        var ExistentProvider = await _context.Providers.FindAsync(id);
        if (ExistentProvider == null)
        {
            return NotFound();
        }
        if (ExistentProvider.Id != modifiedProvider.Id)
        {
            return BadRequest();
        }
        var history = await _context.Providers
                .Where(i => i.Name == modifiedProvider.Name && i.Id!=id)
                .FirstOrDefaultAsync();
        if (history != null)
        {
            TempData["MessageProvider"] = $"The provider <strong>{modifiedProvider.Name}</strong> already exists!";
            return RedirectToAction(nameof(Index));
        }
        if (ModelState.IsValid)
        {
            try
            {
                TempData["MessageProvider"] = $"We modified provider <strong>{ExistentProvider.Name}</strong> to <strong>{modifiedProvider.Name}</strong>!";
                ExistentProvider.Name = modifiedProvider.Name;
                _context.Update(ExistentProvider);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProviderExists(ExistentProvider.Id))
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
        return View(modifiedProvider);
    }

    // GET: Admin/Provider/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null || _context.Providers == null)
        {
            return NotFound();
        }

        var ExistentProvider = _mapper.Map<ExistentProviderDto>(await _context.Providers.FirstOrDefaultAsync(m => m.Id == id));
        if (ExistentProvider == null)
        {
            return NotFound();
        }

        return View(ExistentProvider);
    }

    // POST: Admin/Provider/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (_context.Providers == null)
        {
            return Problem("Entity set 'ApplicationDbContext.Providers' is null.");
        }
        var ExistentProvider = await _context.Providers
            .Include(t => t.Ingredients)
            .Include(t => t.Managers)
            .FirstOrDefaultAsync(i => i.Id == id);

        var ProviderFoods = await _context.Foods
            .Where(i => i.IsHomeMade == false && i.ProviderId == id)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (ExistentProvider == null)
        {
            return NotFound();
        }
        else
        {
            if (ExistentProvider.Ingredients!.Count == 0 && ExistentProvider.Managers!.Count == 0 && ProviderFoods == null)
            {
                _context.Providers.Remove(ExistentProvider);
                TempData["MessageProvider"] = $"Provider <strong>{ExistentProvider.Name}</strong> was successfully deleted!";
            }
            else
            {
                TempData["MessageProvider"] = $"Provider <strong>{ExistentProvider.Name}</strong> can not be deleted as it has associated ingredients, food items or managers!";
            }
        }
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: Admin/Provider/Edit/5
    public async Task<IActionResult> Managers(int id)
    {
        if (_context.Providers == null)
        {
            return NotFound();
        }

        var ExistentProvider = await _context.Providers
            .Include(t => t.Managers)
            .FirstOrDefaultAsync(i => i.Id == id);
        if (ExistentProvider is null)
        {
            return NotFound();
        }
        var Managers = _mapper.Map<List<ExistentUserDto>>(await _userManager.GetUsersInRoleAsync("Manager"));

        var associatedManagers = await _context.ProviderManagers.Where(c => c.ProviderId == id).ToListAsync();

        for (var i = Managers.Count - 1; i >= 0; i--)
        {
            if (!associatedManagers.Any(m => m.ManagerId == Managers[i].Id))
            {
                Managers.Remove(Managers[i]);
            }
        }
        var output = new ProviderManagersVm()
        {
            Id = id,
            Name = ExistentProvider.Name,
            ListManagers = Managers
        };
        return View(output);
    }

    public async Task<IActionResult> AddManager(int id)
    {
        if (_context.Providers == null)
        {
            return NotFound();
        }

        var ExistentProvider = await _context.Providers
            .FirstOrDefaultAsync(i => i.Id == id);
        if (ExistentProvider is null)
        {
            return NotFound();
        }

        var Managers = _mapper.Map<List<ExistentUserDto>>(await _userManager.GetUsersInRoleAsync("Manager"));
        ViewBag.Managers = new SelectList(Managers, "Id", "Email");
        return View(new ProviderManagerDto()
        {
            ProviderId = id,
            Provider = ExistentProvider.Name
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddManager([FromRoute] int id, [FromForm] ProviderManagerDto userProvider)
    {
        if (_context.Providers == null)
        {
            return NotFound();
        }

        var ExistentProvider = await _context.Providers
            .Include(t => t.Managers)
            .FirstOrDefaultAsync(i => i.Id == id);
        if (ExistentProvider is null)
        {
            return NotFound();
        }
        
        if (ModelState.IsValid)
        {
            //var verify = _context.ProviderManagers.Where(q => q.ProviderId == id).FirstOrDefault();
            //if (verify != null) { TempData["MessageProviderManager"] = "Provider already has a manager!"; }

            var verify = _context.ProviderManagers.Where(q => q.ProviderId != id && q.ManagerId == userProvider.ManagerId).FirstOrDefault();
            if (verify != null) 
            { 
                TempData["MessageProviderManager"] = "The manager is already associated with a different provider!";
                return RedirectToAction(nameof(AddManager), new { id });
            }
            verify = null;

            var existentManager = _context.ProviderManagers.Where(q => q.ProviderId == id && q.ManagerId == userProvider.ManagerId).FirstOrDefault();
            if (existentManager is null)
            {
                var providerManager = new ProviderManager()
                {
                    ProviderId = id,
                    ManagerId = userProvider.ManagerId
                };
                await _context.ProviderManagers.AddAsync(providerManager);
                await _context.SaveChangesAsync();
                TempData["MessageProviderManager"] = "Manager was associated with the provider!";
            }
            else
            {
                TempData["MessageProviderManager"] = "The manager is already associated with the provider!";
            }
            return RedirectToAction(nameof(AddManager), new { id });
        }
        var Managers = _mapper.Map<List<ExistentUserDto>>(await _userManager.GetUsersInRoleAsync("Manager"));
        ViewBag.Managers = new SelectList(Managers, "Id", "Email", userProvider.ManagerId);
        return View(userProvider);
    }

    // GET: Admin/Provider/DeleteManager/5
    public async Task<IActionResult> DeleteManager(int? id, int? pub, string? email)
    {
        if (id == null || pub == null || email == null || _context.ProviderManagers == null || _context.Providers == null)
        {
            return NotFound();
        }

        var ExistentManager = await _context.ProviderManagers
            .FirstOrDefaultAsync(i => i.ManagerId == id && i.ProviderId == pub); 
        if (ExistentManager == null)
        {
            return NotFound();
        }

        var BasicManager = await _userManager.FindByEmailAsync(email);
        ExistentManagerDto managerDto = new();
        if (BasicManager == null || BasicManager.Email==null)
        {
            return NotFound();
        }
        managerDto.Id = BasicManager.Id;
        managerDto.Email = BasicManager.Email;

        return View(managerDto);
    }

    // POST: Admin/Provider/DeleteManagerConfirmed/5
    [HttpPost, ActionName("DeleteManager")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteManagerConfirmed(int id)
    {
        if (_context.ProviderManagers == null)
        {
            return Problem("Entity set 'ApplicationDbContext.ProviderManagers' is null.");
        }
        var ExistentManager = await _context.ProviderManagers
            .FirstOrDefaultAsync(i => i.ManagerId == id);
        var manager = await _userManager.Users.FirstOrDefaultAsync(i => i.Id == id);
        if (ExistentManager == null || manager == null)
        {
            return NotFound();
        }
        else
        {
            _context.ProviderManagers.Remove(ExistentManager);
            TempData["MessageProvider"] = $"Manager <strong>{manager.UserName}</strong> was successfully disassociated from the provider!";
        }
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ProviderExists(int id)
    {
        return (_context.Providers?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}