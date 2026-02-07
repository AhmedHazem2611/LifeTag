namespace LifeTag.Models
{
    public class EmergencyContact
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Relation { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
