using LifeTag.Models;
using LifeTag.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LifeTag.Pages
{
    public class EmergencyProfileModel : PageModel
    {
        private readonly UserService _userService;
        private readonly TagService _tagService;

        public EmergencyProfileModel(UserService userService, TagService tagService)
        {
            _userService = userService;
            _tagService = tagService;
        }
        [BindProperty(SupportsGet = true)]
        public Guid TagId { get; set; }
        public MedicalProfile? Profile { get; set; }

        public IActionResult OnGet(Guid tagId)
        {
            // 1️⃣ Check tag exists
            if (!_tagService.TagExists(tagId))
                return NotFound();

            // 2️⃣ Check active
            if (!_tagService.IsTagActive(tagId))
                return BadRequest("Tag is inactive");

            // 3️⃣ Load profile
            Profile = _userService.GetEmergencyProfile(tagId);

            if (Profile == null)
                return NotFound();

            return Page();
        }
    }
}
