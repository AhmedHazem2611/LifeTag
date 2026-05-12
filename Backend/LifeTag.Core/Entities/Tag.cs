using System;

namespace LifeTag.Core.Entities
{
    public class Tag
    {
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public string Pin { get; set; } = string.Empty;
        public bool IsPinProtected { get; set; } = true;
        public bool IsActive { get; set; } = false;
        
        public int? UserId { get; set; }
        public User? User { get; set; }
    }
}
