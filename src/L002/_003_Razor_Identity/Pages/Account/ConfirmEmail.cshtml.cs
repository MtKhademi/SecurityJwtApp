using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace _003_Razor_Identity.Pages.Account;

public class ConfirmEmail : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;

    [BindProperty] public string Message { get; set; } = default!;

    public ConfirmEmail(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IActionResult> OnGetAsync(string userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            Message = "User not found";
            return Page();
        }
        
        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (result.Succeeded)
        {
            Message = "Email confirmed";
            return Page();
        }
        Message = "Email not confirmed";
        return Page();
    }
}