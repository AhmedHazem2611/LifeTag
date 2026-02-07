namespace LifeTag.Models
{
    public class MedicalProfile
    {
        public string FullName { get; set; }
        public string BloodType { get; set; }

        public ICollection<MedicalCondition> MedicalConditions { get; set; } = new List<MedicalCondition>();
        public ICollection<Allergy> Allergies { get; set; } = new List<Allergy>();
        public ICollection<Medication> Medications { get; set; } = new List<Medication>();
        public ICollection<EmergencyContact> EmergencyContacts { get; set; } = new List<EmergencyContact>();
        public ICollection<MedicalNote> MedicalNotes { get; set; } = new List<MedicalNote>();
    }
}
