using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace _003_Razor_Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        [BindProperty]
        public RegisterInputModel RegisterInputModel { get; set; }
        public void OnGet()
        {
        }
    }

    public class RegisterInputModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
