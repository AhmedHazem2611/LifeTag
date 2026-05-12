using System.Collections.Generic;

namespace LifeTag.Core.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        // Navigation properties
        public Tag? ActiveTag { get; set; }
        public UserProfile? Profile { get; set; }
    }
}
