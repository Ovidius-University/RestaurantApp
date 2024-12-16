using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebAppUI.Models.DTOs;
using WebAppUI.Models;
using WebAppUI.Models.CustomIdentity;
using WebAppUI.Models.ViewModels;

namespace WebAppUI.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly UserManager<AppUser> _userManager;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IMapper mapper, UserManager<AppUser> userManager)
    {
        _logger = logger;
        _context = context;
        _mapper = mapper;
        _userManager = userManager;
    }

    public async Task<IActionResult> IndexAsync()
    {
        /*if (_context.Informations == null || _context.WorkHours == null)
        {
            return NotFound();
        }*/

        var ExistentInformation = _mapper.Map<ExistentInformationDto>(await _context.Informations.FirstOrDefaultAsync());

        var ExistentWorkHours = _mapper.Map<List<ExistentWorkHourDto>>(await _context.WorkHours.ToListAsync());

        var StoreInfo = new StoreInfoVm()
        {
            Information = ExistentInformation,
            WorkHours = ExistentWorkHours
        };

        /*if (ExistentInformation == null || ExistentWorkHours == null)
        {
            return View();
        }*/
        return View(StoreInfo);
    }

    public async Task<IActionResult> PrivacyAsync()
    {
        /*if (_context.Privacies == null)
        {
            return NotFound();
        }*/

        var ExistentPrivacyPolicy = _mapper.Map<ExistentPrivacyDto>(await _context.Privacies.FirstOrDefaultAsync());

        /*if (ExistentPrivacyPolicy == null)
        {
            return View();
        }*/
        return View(ExistentPrivacyPolicy);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
