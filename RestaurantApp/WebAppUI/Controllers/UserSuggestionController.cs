using WebAppUI.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppUI.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebAppUI.Models.DTOs;
using Microsoft.AspNetCore.Identity;
namespace WebAppUI.Controllers;
public class UserSuggestionController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    public UserSuggestionController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [Route("UserSuggestions")]
    public async Task<IActionResult> Index()
    {
        var CurrentCustomer = await _context.Users
                .FirstOrDefaultAsync(q => q.UserName == User.Identity!.Name);
        if (CurrentCustomer == null || CurrentCustomer.UserName == null)
            return Redirect("Identity/Account/Login");

        var UserData = _mapper.Map<UserDataVm>(
             await _context.UserDatas
            .Include(c => c.User)
            .Include(c => c.Gender)
            .Where(q => q.UserId == CurrentCustomer.Id)
            .FirstOrDefaultAsync());
        var OwnedOrders = await _context.Orders
            .Include(c => c.Customer)
            .Include(c => c.PayingMethod)
            .Include(c => c.OrderContents)
            .Where(q => q.CustomerId == CurrentCustomer.Id && q.IsFinal)
            .ToListAsync();
        var OwnedOrderContents = await _context.OrderContents
            .Include(c => c.Food)
            .ToListAsync();

        OwnedOrderContents = OwnedOrderContents
            .Where(q => OwnedOrders.Any(j => j.Id == q.OrderId))
            .ToList();

        var OwnedFoods = await _context.Foods
            .Include(t => t.Ingredients!)
            .ThenInclude(t => t.Ingredient)
            .Include(t => t.Category)
            .Include(t => t.Offer)
            .OrderBy(i => i.Title)
            .ToListAsync();

        OwnedFoods = OwnedFoods
            .Where(i => OwnedOrderContents.Any(j => j.FoodId == i.Id))
            .OrderBy(i => i.Title)
            .ToList();

        var DifferentOrders = await _context.Orders
            .Include(c => c.Customer)
            .Include(c => c.PayingMethod)
            .Include(c => c.OrderContents)
            .Where(q => q.CustomerId != CurrentCustomer.Id && q.IsFinal)
            .ToListAsync();

        var OwnedData = await _context.UserDatas
            .Include(c => c.User)
            .Include(c => c.Gender)
            .Where(q => q.UserId == CurrentCustomer.Id)
            .FirstOrDefaultAsync();
        if(OwnedData == null)
        {
            var erdisplay = new IndexUserSuggestionVm()
            {
                UserData = null,
                ListFoods = null
            };
            return View(erdisplay);
        }
       
        var DifferentUsers = await _context.UserDatas
            .Include(c => c.User)
            .Include(c => c.Gender)
            .Where(q => q.UserId != CurrentCustomer.Id && q.GenderId==OwnedData!.GenderId)
            .ToListAsync();
        DifferentUsers = DifferentUsers
            .Where(q => (q.Birthday - OwnedData.Birthday).Duration().TotalDays <= 365 * 2 + 1)
            .ToList();
        
        DifferentOrders = DifferentOrders
            .Where(q => DifferentUsers.Any(j => j.UserId == q.CustomerId))
            .ToList();
        
        var DifferentOrderContents = await _context.OrderContents
            .Include(c => c.Food)
            .ToListAsync();

        DifferentOrderContents = DifferentOrderContents
            .Where(q => DifferentOrders.Any(j => j.Id == q.OrderId))
            .ToList();

        var DifferentFoods = await _context.Foods
            .Include(t => t.Ingredients!)
            .ThenInclude(t => t.Ingredient)
            .Include(t => t.Category)
            .Include(t => t.Offer)
            .OrderBy(i => i.Title)
            .ToListAsync();

        DifferentFoods = DifferentFoods
            .Where(i => DifferentOrderContents.Any(j => j.FoodId == i.Id))
            .OrderBy(i => i.Title)
            .ToList();

        var Suggestions = await _context.Foods
            .Include(t => t.Ingredients!)
            .ThenInclude(t => t.Ingredient)
            .Include(t => t.Category)
            .Include(t => t.Offer)
            .OrderBy(i => i.Title)
            .ToListAsync();

        Suggestions = Suggestions
            .Where(i => !OwnedFoods.Any(j => j.Id == i.Id) && DifferentFoods.Any(j => j.Id == i.Id) && i.IsFinal)
            .OrderBy(i => i.Title)
            .ToList();

        var output = new IndexUserSuggestionVm()
        {
            UserData = UserData,
            ListFoods = _mapper.Map<List<CardFoodVm>>(Suggestions)
        };
        return View(output);
    }

    // GET: UserSuggestion/Details/5
    [Route("UserSuggestion/Details/{id}")]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || _context.UserDatas == null)
        {
            return NotFound();
        }

        var CurrentCustomer = await _context.Users
            .FirstOrDefaultAsync(q => q.UserName == User.Identity!.Name);
        if (CurrentCustomer == null || CurrentCustomer.UserName == null)
            return Problem("You haven't logged in, or haven't made an account yet!");

        var OwnedData = await _context.UserDatas
            .Include(c => c.User)
            .Include(c => c.Gender)
            .Where(q => q.UserId == CurrentCustomer.Id)
            .FirstOrDefaultAsync();
        if (OwnedData == null)
        {
            return NotFound();
        }

        var output = _mapper.Map<UserDataVm>(OwnedData);
        return View(output);
    }

    // GET: UserSuggestion/Create
    public async Task<IActionResult> Create()
    {
        if (_context.Users == null || _context.UserDatas == null)
        {
            return NotFound();
        }

        var CurrentCustomer = await _context.Users
            .FirstOrDefaultAsync(q => q.UserName == User.Identity!.Name);
        if (CurrentCustomer == null)
            return NotFound();//redirectionez

        var methods = _mapper.Map<List<GenderDto>>(
            await _context.Genders
            .ToListAsync());
        if (methods == null)
        {
            return NotFound();
        }
        ViewBag.AvailableGenders = new SelectList(methods, "GenderId", "Name");
        return View();
    }

    // POST: UserSuggestion/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([FromForm] NewUserDataDto newUserData)
    {
        var CurrentCustomer = await _context.Users
            .FirstOrDefaultAsync(q => q.UserName == User.Identity!.Name);
        if (CurrentCustomer == null)
            return NotFound();

        var gender = await _context.Genders
                              .Where(q => q.Id == newUserData.GenderId)
                              .FirstOrDefaultAsync();
        if (gender == null)
            return BadRequest();

        if (ModelState.IsValid)
        {
            var NewUserData = _mapper.Map<UserData>(newUserData);
            NewUserData.UserId = CurrentCustomer!.Id;
            _context.Add(NewUserData);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        var methods = _mapper.Map<List<GenderDto>>(
            await _context.Genders
            .ToListAsync());
        if (methods == null)
        {
            return NotFound();
        }
        ViewBag.AvailableGenders = new SelectList(methods, "GenderId", "Name");
        return View();
    }


    // GET: UserSuggestion/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || _context.UserDatas == null || _context.Users == null)
        {
            return NotFound();
        }
        var CurrentCustomer = await _context.Users
            .Where(q => q.UserName == User!.Identity!.Name)
            .FirstOrDefaultAsync();
        if (CurrentCustomer is null)
            return BadRequest();

        var OwnedData = await _context.UserDatas
            .Include(c => c.User)
            .Include(c => c.Gender)
            .Where(q => q.UserId == CurrentCustomer.Id)
            .FirstOrDefaultAsync();
        if (OwnedData == null)
        {
            return NotFound();
        }
        var output = _mapper.Map<ExistentUserDataDto>(OwnedData);
        var methods = _mapper.Map<List<GenderDto>>(
            await _context.Genders
            .ToListAsync());
        if (methods == null)
        {
            return NotFound();
        }
        ViewBag.AvailableGenders = new SelectList(methods, "GenderId", "Name");
        return View(output);
    }

    // POST: UserSuggestion/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit([FromRoute] int id, [FromForm] ExistentUserDataDto ModifiedUserData)
    {
        if (ModifiedUserData is null || id != ModifiedUserData.UserId)
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

                var OwnedData = await _context.UserDatas
                        .Include(c => c.User)
                        .Include(c => c.Gender)
                        .Where(q => q.UserId == CurrentCustomer.Id)
                        .FirstOrDefaultAsync();
                if (OwnedData == null)
                {
                    return NotFound();
                }

                ModifiedUserData.ToEntity(ref OwnedData);
                _context.Update(OwnedData);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserDataExists(ModifiedUserData.UserId))
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

        var CurrenCustomer = await _context.Users
            .Where(q => q.UserName == User!.Identity!.Name)
            .FirstOrDefaultAsync();
        if (CurrenCustomer is null)
            return BadRequest();

        var OwnData = await _context.UserDatas
                        .Include(c => c.User)
                        .Include(c => c.Gender)
                        .Where(q => q.UserId == CurrenCustomer.Id)
                        .FirstOrDefaultAsync();
        if (OwnData == null)
        {
            return NotFound();
        }
        var output = _mapper.Map<ExistentUserDataDto>(OwnData);
        TempData["MessageUserDataEdit"] = $"Please complete the necessary information properly!";
        var methods = _mapper.Map<List<GenderDto>>(
            await _context.Genders
            .ToListAsync());
        if (methods == null)
        {
            return NotFound();
        }
        ViewBag.AvailableGenders = new SelectList(methods, "GenderId", "Name");
        return View(output);
    }

    // GET: UserSuggestion/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null || _context.UserDatas == null)
        {
            return NotFound();
        }
        var CurrentCustomer = await _context.Users
                    .Where(q => q.UserName == User!.Identity!.Name)
                    .FirstOrDefaultAsync();
        if (CurrentCustomer is null)
            return BadRequest();

        var userData = await _context.UserDatas
            .Include(c => c.User)
            .Include(c => c.Gender)
            .FirstOrDefaultAsync(m => m.UserId == id && m.UserId == CurrentCustomer.Id);
        if (userData == null)
        {
            return NotFound();
        }

        var output = _mapper.Map<UserDataVm>(userData);
        return View(output);
    }

    // POST: UserSuggestion/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var CurrentCustomer = await _context.Users
                    .Where(q => q.UserName == User!.Identity!.Name)
                    .FirstOrDefaultAsync();
        if (CurrentCustomer is null)
            return BadRequest();

        if (_context.UserDatas == null)
        {
            return Problem("Entity set 'ApplicationDbContext.ShopCarts' is null.");
        }

        var userData = await _context.UserDatas.FindAsync(CurrentCustomer.Id);
        if (userData != null)
        {
            _context.UserDatas.Remove(userData);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool UserDataExists(int id)
    {
        return (_context.UserDatas?.Any(e => e.UserId == id)).GetValueOrDefault();
    }
}