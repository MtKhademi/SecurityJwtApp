using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPage_Identity.Pages;

[Authorize(Policy = "AdminOnly")]
public class Setting : PageModel
{
    public void OnGet()
    {
        
    }
}