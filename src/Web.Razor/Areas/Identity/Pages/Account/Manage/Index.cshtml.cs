﻿using System.ComponentModel.DataAnnotations;
using Core.Application.DTOs;
using Core.Application.Interfaces.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Razor.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;

        public IndexModel(
            IUserService userService, IAuthService authService)
        {
            _userService = userService;
            _authService = authService;         
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
        }

        private async Task LoadAsync(ApplicationUserDto user)
        {
            var userName = await _userService.GetUserNameAsync(user);
            var phoneNumber = await _userService.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _authService.GetCurrentUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _authService.GetCurrentUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userService.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userService.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            await _authService.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
