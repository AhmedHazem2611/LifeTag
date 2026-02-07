namespace LifeTag.Models
{
    public class Tag
    {
        public Guid Id { get; set; }
        public string Pin { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        public User User { get; set; }
        public Guid UserId { get; set; }
    }
}
