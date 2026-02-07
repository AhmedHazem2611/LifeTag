using LifeTag.Models;
using LifeTag.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LifeTag.Pages
{
    public class MedicalFormModel : PageModel
    {
        private readonly UserService _userService;
        private readonly MedicalProfileService _medicalProfileService;
        private readonly TagService _tagService;

        public MedicalFormModel(
            UserService userService,
            MedicalProfileService medicalProfileService,
            TagService tagService)
        {
            _userService = userService;
            _medicalProfileService = medicalProfileService;
            _tagService = tagService;
        }

        [BindProperty]
        public MedicalFormInput Input { get; set; } = new();
        public void OnGet()
        {
            Input.EmergencyContacts = new List<EmergencyContactInput>
            {
                new EmergencyContactInput(),
                new EmergencyContactInput(),
                new EmergencyContactInput()
            };
        }
        public IActionResult OnPost(Guid tagId)
        {
            if (!ModelState.IsValid)
                return Page();

            // 1. Create User using UserService
            var user = new User
            {
                FullName = Input.FullName,
                TagId = tagId,
                DateOfBirth = Input.DateOfBirth,
                BloodType = Input.BloodType
            };

            user = _userService.AddUser(user);

            // 2. Create Medical Profile
            _medicalProfileService.CreateMedicalProfile(user.Id, Input);

            // 3. Activate Tag
            _tagService.ActivateTag(tagId, user.Id);

            return RedirectToPage("/Index");
        }
    }
}
