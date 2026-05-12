namespace LifeTag.Core.Entities
{
    public class ProfileEntry
    {
        public int Id { get; set; }
        public int ProfileSectionId { get; set; }
        public string DataJson { get; set; } = string.Empty;

        // Navigation properties
        public ProfileSection? ProfileSection { get; set; }
    }
}
