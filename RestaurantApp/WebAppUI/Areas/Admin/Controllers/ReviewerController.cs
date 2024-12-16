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
public class ReviewerController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly UserManager<AppUser> _userManager;

    public ReviewerController(ApplicationDbContext context, IMapper mapper, UserManager<AppUser> userManager)
    {
        _context = context;
        _mapper = mapper;
        _userManager = userManager;
    }

    // GET: Admin/Reviewer
    [Route("Admin/Reviewers")]
    public async Task<IActionResult> Index()
    {
        if (_context.Reviewers == null)
            return Problem("Entity set 'ApplicationDbContext.Reviewers' is null.");
        return View(_mapper.Map<List<ExistentReviewerDto>>(await _context.Reviewers.ToListAsync()));
    }

    // GET: Admin/Reviewer/Details/5
    [Route("Admin/Reviewer/Details/{id}")]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || _context.Reviewers == null)
        {
            return NotFound();
        }

        var ExistentReviewer = _mapper.Map<ExistentReviewerDto>(await _context.Reviewers.FirstOrDefaultAsync(m => m.Id == id));
        if (ExistentReviewer == null)
        {
            return NotFound();
        }

        return View(ExistentReviewer);
    }

    // GET: Admin/Reviewer/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Admin/Reviewer/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([FromForm] NewReviewerDto newReviewer)
    {
        if (ModelState.IsValid)
        {
            var history = await _context.Reviewers
                .Where(i => i.Name == newReviewer.Name)
                .FirstOrDefaultAsync();
            if (history != null)
            {
                TempData["MessageReviewer"] = $"The reviewer <strong>{newReviewer.Name}</strong> is already added!";
                return RedirectToAction(nameof(Index));
            }

            var reviewer = _mapper.Map<Reviewer>(newReviewer);
            _context.Add(reviewer);
            await _context.SaveChangesAsync();
            TempData["MessageReviewer"] = $"We added reviewer <strong>{newReviewer.Name}</strong>!";
            return RedirectToAction(nameof(Index));
        }
        return View(newReviewer);
    }

    // GET: Admin/Reviewer/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || _context.Reviewers == null)
        {
            return NotFound();
        }

        var ExistentReviewer = _mapper.Map<ExistentReviewerDto>(await _context.Reviewers.FindAsync(id));
        if (ExistentReviewer == null)
        {
            return NotFound();
        }
        return View(ExistentReviewer);
    }

    // POST: Admin/Reviewer/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [FromForm] ExistentReviewerDto modifiedReviewer)
    {
        if (id != modifiedReviewer.Id)
        {
            return NotFound();
        }
        var ExistentReviewer = await _context.Reviewers.FindAsync(id);
        if (ExistentReviewer == null)
        {
            return NotFound();
        }
        if (ExistentReviewer.Id != modifiedReviewer.Id)
        {
            return BadRequest();
        }
        var history = await _context.Reviewers
                .Where(i => i.Name == modifiedReviewer.Name && i.Id!=id)
                .FirstOrDefaultAsync();
        if (history != null)
        {
            TempData["MessageReviewer"] = $"The reviewer <strong>{modifiedReviewer.Name}</strong> already exists!";
            return RedirectToAction(nameof(Index));
        }
        if (ModelState.IsValid)
        {
            try
            {
                TempData["MessageReviewer"] = $"We modified reviewer <strong>{ExistentReviewer.Name}</strong> to <strong>{modifiedReviewer.Name}</strong>!";
                ExistentReviewer.Name = modifiedReviewer.Name;
                _context.Update(ExistentReviewer);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReviewerExists(ExistentReviewer.Id))
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
        return View(modifiedReviewer);
    }

    // GET: Admin/Reviewer/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null || _context.Reviewers == null)
        {
            return NotFound();
        }

        var ExistentReviewer = _mapper.Map<ExistentReviewerDto>(await _context.Reviewers.FirstOrDefaultAsync(m => m.Id == id));
        if (ExistentReviewer == null)
        {
            return NotFound();
        }

        return View(ExistentReviewer);
    }

    // POST: Admin/Reviewer/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (_context.Reviewers == null)
        {
            return Problem("Entity set 'ApplicationDbContext.Reviewers' is null.");
        }
        var ExistentReviewer = await _context.Reviewers
            .Include(t => t.Reviews)
            .Include(t => t.Critics)
            .FirstOrDefaultAsync(i => i.Id == id);
        if (ExistentReviewer == null)
        {
            return NotFound();
        }
        else
        {
            if (ExistentReviewer.Reviews!.Count == 0 && ExistentReviewer.Critics!.Count == 0)
            {
                _context.Reviewers.Remove(ExistentReviewer);
                TempData["MessageReviewer"] = $"Reviewer <strong>{ExistentReviewer.Name}</strong> was successfully deleted!";
            }
            else
            {
                TempData["MessageReviewer"] = $"Reviewer <strong>{ExistentReviewer.Name}</strong> can not be deleted as it has associated reviews or critics!";
            }
        }
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: Admin/Reviewer/Edit/5
    public async Task<IActionResult> Critics(int id)
    {
        if (_context.Reviewers == null)
        {
            return NotFound();
        }

        var ExistentReviewer = await _context.Reviewers
            .Include(t => t.Critics)
            .FirstOrDefaultAsync(i => i.Id == id);
        if (ExistentReviewer is null)
        {
            return NotFound();
        }
        var Critics = _mapper.Map<List<ExistentUserDto>>(await _userManager.GetUsersInRoleAsync("Critic"));

        var associatedCritics = await _context.ReviewerCritics.Where(c => c.ReviewerId == id).ToListAsync();

        for (var i = Critics.Count - 1; i >= 0; i--)
        {
            if (!associatedCritics.Any(m => m.CriticId == Critics[i].Id))
            {
                Critics.Remove(Critics[i]);
            }
        }
        var output = new ReviewerCriticsVm()
        {
            Id = id,
            Name = ExistentReviewer.Name,
            ListCritics = Critics
        };
        return View(output);
    }

    public async Task<IActionResult> AddCritic(int id)
    {
        if (_context.Reviewers == null)
        {
            return NotFound();
        }

        var ExistentReviewer = await _context.Reviewers
            .FirstOrDefaultAsync(i => i.Id == id);
        if (ExistentReviewer is null)
        {
            return NotFound();
        }

        var Critics = _mapper.Map<List<ExistentUserDto>>(await _userManager.GetUsersInRoleAsync("Critic"));
        ViewBag.Critics = new SelectList(Critics, "Id", "Email");
        return View(new ReviewerCriticDto()
        {
            ReviewerId = id,
            Reviewer = ExistentReviewer.Name
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddCritic([FromRoute] int id, [FromForm] ReviewerCriticDto userReviewer)
    {
        if (_context.Reviewers == null)
        {
            return NotFound();
        }

        var ExistentReviewer = await _context.Reviewers
            .Include(t => t.Critics)
            .FirstOrDefaultAsync(i => i.Id == id);
        if (ExistentReviewer is null)
        {
            return NotFound();
        }
        
        if (ModelState.IsValid)
        {
            //var verify = _context.ReviewerCritics.Where(q => q.ReviewerId == id).FirstOrDefault();
            //if (verify != null) { TempData["MessageReviewerCritic"] = "Reviewer already has a critic!"; }

            var verify = _context.ReviewerCritics.Where(q => q.ReviewerId != id && q.CriticId == userReviewer.CriticId).FirstOrDefault();
            if (verify != null) 
            { 
                TempData["MessageReviewerCritic"] = "The critic is already associated with a different reviewer!";
                return RedirectToAction(nameof(AddCritic), new { id });
            }
            verify = null;

            var existentCritic = _context.ReviewerCritics.Where(q => q.ReviewerId == id && q.CriticId == userReviewer.CriticId).FirstOrDefault();
            if (existentCritic is null)
            {
                var reviewerCritic = new ReviewerCritic()
                {
                    ReviewerId = id,
                    CriticId = userReviewer.CriticId
                };
                await _context.ReviewerCritics.AddAsync(reviewerCritic);
                await _context.SaveChangesAsync();
                TempData["MessageReviewerCritic"] = "The critic was associated with the reviewer!";
            }
            else
            {
                TempData["MessageReviewerCritic"] = "The critic is already associated with the reviewer!";
            }
            return RedirectToAction(nameof(AddCritic), new { id });
        }
        var Critics = _mapper.Map<List<ExistentUserDto>>(await _userManager.GetUsersInRoleAsync("Critic"));
        ViewBag.Critics = new SelectList(Critics, "Id", "Email", userReviewer.CriticId);
        return View(userReviewer);
    }

    // GET: Admin/Reviewer/DeleteCritic/5
    public async Task<IActionResult> DeleteCritic(int? id, int? pub, string? email)
    {
        if (id == null || pub == null || email == null || _context.ReviewerCritics == null || _context.Reviewers == null)
        {
            return NotFound();
        }

        var ExistentCritic = await _context.ReviewerCritics
            .FirstOrDefaultAsync(i => i.CriticId == id && i.ReviewerId == pub); 
        if (ExistentCritic == null)
        {
            return NotFound();
        }

        var BasicCritic = await _userManager.FindByEmailAsync(email);
        ExistentCriticDto criticDto = new();
        if (BasicCritic == null || BasicCritic.Email==null)
        {
            return NotFound();
        }
        criticDto.Id = BasicCritic.Id;
        criticDto.Email = BasicCritic.Email;

        return View(criticDto);
    }

    // POST: Admin/Reviewer/DeleteCriticConfirmed/5
    [HttpPost, ActionName("DeleteCritic")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCriticConfirmed(int id)
    {
        if (_context.ReviewerCritics == null)
        {
            return Problem("Entity set 'ApplicationDbContext.ReviewerCritics' is null.");
        }
        var ExistentCritic = await _context.ReviewerCritics
            .FirstOrDefaultAsync(i => i.CriticId == id);
        var critic = await _userManager.Users.FirstOrDefaultAsync(i => i.Id == id);
        if (ExistentCritic == null || critic == null)
        {
            return NotFound();
        }
        else
        {
            _context.ReviewerCritics.Remove(ExistentCritic);
            TempData["MessageReviewer"] = $"The critic <strong>{critic.UserName}</strong> was successfully disassociated from the reviewer!";
        }
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ReviewerExists(int id)
    {
        return (_context.Reviewers?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}