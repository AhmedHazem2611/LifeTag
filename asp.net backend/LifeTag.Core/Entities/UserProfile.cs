using System.Collections.Generic;

namespace LifeTag.Core.Entities
{
    public class UserProfile
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string TemplateType { get; set; } = "Medical";

        // Navigation properties
        public User? User { get; set; }
        public ICollection<ProfileSection> Sections { get; set; } = new List<ProfileSection>();
    }
}
