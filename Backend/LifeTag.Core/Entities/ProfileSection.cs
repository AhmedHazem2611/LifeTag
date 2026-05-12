using System.Collections.Generic;

namespace LifeTag.Core.Entities
{
    public class ProfileSection
    {
        public int Id { get; set; }
        public int UserProfileId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string SectionType { get; set; } = "text"; // text, list, contact, address, note
        public int DisplayOrder { get; set; }

        // Navigation properties
        public UserProfile? UserProfile { get; set; }
        public ICollection<ProfileEntry> Entries { get; set; } = new List<ProfileEntry>();
    }
}
