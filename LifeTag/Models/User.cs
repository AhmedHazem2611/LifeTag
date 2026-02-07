namespace LifeTag.Models
{
    public class User
    {
        public Guid Id {  get; set; }
        public string FullName {  get; set; }
        public DateTime DateOfBirth { get; set; }
        public string BloodType { get; set; }
        public Guid TagId { get; set; }
        public Tag Tag { get; set; }

        public ICollection<MedicalCondition> MedicalConditions { get; set; } = new List<MedicalCondition>();
        public ICollection<Allergy> Allergies { get; set; } = new List<Allergy>();
        public ICollection<Medication> Medications { get; set; } = new List<Medication>();
        public ICollection<EmergencyContact> EmergencyContacts { get; set; } = new List<EmergencyContact>();
        public ICollection<MedicalNote> MedicalNotes { get; set; } = new List<MedicalNote>();
    }
}
