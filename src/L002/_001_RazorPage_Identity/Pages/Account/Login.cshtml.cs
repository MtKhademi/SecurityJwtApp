using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPage_Identity.Pages.Account;

public class Login : PageModel
{
    [BindProperty] public Credential Credential { get; set; } = new();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPost()
    {
        if (!ModelState.IsValid) return Page();

        if (Credential.UserName == "admin" && Credential.Password == "123")
        {
            // creating security context
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, Credential.UserName),
                new Claim(ClaimTypes.Email, "admin@email.com"),
                new Claim("Department", "HR"),
                new Claim("Admin", "true"),
                new Claim("Manager", "true"),
                new Claim("EmploymentDate", "2025-01-01")
            };

            var identity = new ClaimsIdentity(claims, "MyCookieAuth");
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("MyCookieAuth", principal);

            return RedirectToPage("/Index");
        }

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