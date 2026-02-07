using LifeTag.Models;
using LifeTag.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LifeTag.Pages.Forms
{
    public class addUserProfileModel : PageModel
    {
        private readonly UserService _userService;
        public addUserProfileModel (UserService userService)
        {
            _userService = userService;
        }

        [BindProperty]
        public User User {  get; set; }

        public void OnGet()
        {
            User = new User();
        }
        public IActionResult OnPost()
        {

            if (ModelState.IsValid == false)
            {
                return Page();
            }
            _userService.AddUser(User);
            return RedirectToPage("/Index");
        }
    }
}
