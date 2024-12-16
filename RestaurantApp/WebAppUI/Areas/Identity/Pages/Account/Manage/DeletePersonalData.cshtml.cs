// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebAppUI.Models.CustomIdentity;

namespace WebAppUI.Areas.Identity.Pages.Account.Manage
{
    public class DeletePersonalDataModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<DeletePersonalDataModel> _logger;

        public DeletePersonalDataModel(
            ApplicationDbContext context,
            IMapper mapper,
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ILogger<DeletePersonalDataModel> logger)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public bool RequirePassword { get; set; }

        public int Roles { get; set; } = 0;

        public async Task<IActionResult> OnGet()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            RequirePassword = await _userManager.HasPasswordAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            RequirePassword = await _userManager.HasPasswordAsync(user);
            if (RequirePassword)
            {
                if (!await _userManager.CheckPasswordAsync(user, Input.Password))
                {
                    ModelState.AddModelError(string.Empty, "Incorrect password.");
                    return Page();
                }
            }

            if (await _userManager.IsInRoleAsync(user, "Administrator")) Roles++;
            if (await _userManager.IsInRoleAsync(user, "Manager")) Roles++;
            if (await _userManager.IsInRoleAsync(user, "Critic")) Roles++;
            if (await _userManager.IsInRoleAsync(user, "Chef")) Roles++;
            if (await _userManager.IsInRoleAsync(user, "Cashier")) Roles++;
            if (await _userManager.IsInRoleAsync(user, "Delivery")) Roles++;
            if (Roles>0)
            {
                ModelState.AddModelError(string.Empty, "You cannot delete your account as long as you still have a registered role in the application.");
                return Page();
            }
            //Roles++;
            /*
            var order = await _context.ShopCarts
            .Include(c => c.Customer)
            .Include(c => c.Food)
            .Include(c => c.Food!.Offer)
            .Where(q => q.CustomerId == user.Id)
            .ToListAsync();
            */

            var orders = await _context.Orders
            .Include(c => c.Customer)
            .Where(q => q.CustomerId == user.Id && q.IsFinal == false)
            .ToListAsync();
            if (orders == null || orders.Count > 0)
            {
                ModelState.AddModelError(string.Empty, "You cannot delete your account as long as you have an unfinished order. Wait until it is over or contact our email to cancel it.");
                return Page();
            }
            var result = await _userManager.DeleteAsync(user);
            var userId = await _userManager.GetUserIdAsync(user);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Unexpected error occurred deleting user.");
            }

            await _signInManager.SignOutAsync();

            _logger.LogInformation("User with ID '{UserId}' deleted themselves.", userId);

            return Redirect("~/");
        }
    }
}
