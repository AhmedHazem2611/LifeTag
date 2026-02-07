using LifeTag.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LifeTag.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly TagService _tagService;

        public IndexModel(ILogger<IndexModel> logger, TagService tagService)
        {
            _logger = logger;
            _tagService = tagService;
        }

        [BindProperty(SupportsGet = true)]
        public Guid TagId { get; set; } = Guid.Parse("A9DD6742-6120-4A26-9310-AEF5B67BD898");
        [BindProperty]
        public string Pin { get; set; }

        public IActionResult OnGet()
        {
            if (TagId == Guid.Empty || !_tagService.TagExists(TagId))
            {
                _logger.LogWarning("Invalid or missing TagId: {TagId}", TagId);
                return RedirectToPage("/Error");
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            if (string.IsNullOrWhiteSpace(Pin) || Pin.Length != 4)
            {
                ModelState.AddModelError(nameof(Pin), "Invalid PIN");
                return Page();
            }

            if (!_tagService.ValidatePin(TagId, Pin))
            {
                ModelState.AddModelError(nameof(Pin), "Invalid PIN");
                return Page();
            }

            if(!_tagService.IsTagActive(TagId))
            {
                return RedirectToPage("/MedicalForm");
            }

            return RedirectToPage("/EmergencyProfile");
        }
    }
}
