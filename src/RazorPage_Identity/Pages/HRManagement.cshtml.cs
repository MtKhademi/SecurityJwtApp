using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPage_Identity.Pages;

[Authorize(Policy = "HRManagerOnly")]
public class HRManagement : PageModel
{
    public void OnGet()
    {
        
    }
}