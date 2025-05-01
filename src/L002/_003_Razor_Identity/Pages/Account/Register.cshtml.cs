using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace _003_Razor_Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public RegisterModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty] public RegisterInputModel RegisterInputModel { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // validation email [optional => we set in program.cs the user to be unique]

            // create the user
            var user = new IdentityUser
            {
                Email = RegisterInputModel.Email,
                UserName = RegisterInputModel.Email
            };

            var resultCreateUser = await _userManager.CreateAsync(user, RegisterInputModel.Password);
            if (resultCreateUser.Succeeded)
            {
                // generate token 
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                return Redirect(Url.PageLink(pageName: "/Account/ConfirmEmail",
                    values: new { userId = user.Id, token }) ?? "");
                
                return RedirectToPage("/account/login");
            }

            foreach (var error in resultCreateUser.Errors)
            {
                ModelState.AddModelError("Register", error.Description);
            }

            return Page();
        }
    }

    public class RegisterInputModel
    {
        [Required] public string Email { get; set; }
        [Required] public string Password { get; set; }
    }
}