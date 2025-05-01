using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace _003_Razor_Identity.Pages.Account;

public class Login : PageModel
{
    private readonly SignInManager<IdentityUser> _signInManager;

    public Login(SignInManager<IdentityUser> signInManager)
    {
        _signInManager = signInManager;
    }

    [BindProperty] public Credential Credential { get; set; } = new();


    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var resultSignIn = await _signInManager.PasswordSignInAsync(
            Credential.UserName,
            Credential.Password,
            isPersistent: false,
            lockoutOnFailure: false
        );

        if (resultSignIn.Succeeded)
        {
            return RedirectToPage("/Index");
        }
        if (resultSignIn.IsLockedOut)
        {
            ModelState.AddModelError("Login", "User is locked out.");
            return Page();
        }
        if (resultSignIn.IsNotAllowed)
        {
            ModelState.AddModelError("Login", "User is not allowed.");
            return Page();
        }
        
        ModelState.AddModelError("Login", "Failed to login.");
        return Page();
    }
}

public class Credential
{
    [Required]
    [Display(Description = "User Name")]
    public string UserName { get; set; } = default!;

    [Required]
    [Display(Description = "Password")]
    public string Password { get; set; } = default!;
}