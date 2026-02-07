namespace LifeTag.Models
{
    public class MedicalFormInput
    {
        // Step 1
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int? WeightKg { get; set; }
        public int? HeightCm { get; set; }

        // Step 2
        public string BloodType { get; set; }
        public string ChronicDiseases { get; set; }
        public string Allergies { get; set; }
        public string Medications { get; set; }

        // Step 3
        public List<EmergencyContactInput> EmergencyContacts { get; set; } = new();

        //Step 4
        public string MedicalNotes {  get; set; }
    }
}
