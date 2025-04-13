using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPage_Identity.Pages;


[Authorize(Policy = "MustBelongToHRDepartment")]
public class HumanResouce : PageModel
{
    public void OnGet()
    {
        
    }
}