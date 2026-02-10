using LifeTag.Models;

namespace LifeTag.Services
{
    public class MedicalProfileService
    {
        private readonly LifeTagContext _context;

        public MedicalProfileService(LifeTagContext context)
        {
            _context = context;
        }

        public void CreateMedicalProfile(Guid userId, MedicalFormInput input)
        {
            // Chronic Diseases
            foreach (var name in Split(input.ChronicDiseases))
            {
                _context.MedicalConditions.Add(new MedicalCondition
                {
                    Name = name,
                    UserId = userId
                });
            }

            // Allergies
            foreach (var name in Split(input.Allergies))
            {
                _context.Allergies.Add(new Allergy
                {
                    Name = name,
                    UserId = userId
                });
            }

            // Medications
            foreach (var name in Split(input.Medications))
            {
                _context.Medications.Add(new Medication
                {
                    Name = name,
                    UserId = userId
                });
            }
            foreach (var content in Split(input.MedicalNotes))
            {
                _context.MedicalNotes.Add(new MedicalNote
                {
                    Content = content,
                    UserId = userId
                });
            }

            // Emergency Contacts
            foreach (var c in input.EmergencyContacts)
            {
                _context.EmergencyContacts.Add(new EmergencyContact
                {
                    Name = c.Name,
                    Relation = c.Relationship,
                    PhoneNumber = c.PhoneNumber,
                    UserId = userId
                });
            }

            _context.SaveChanges();
        }

        private IEnumerable<string> Split(string input)
        {
            return string.IsNullOrWhiteSpace(input)
                ? Enumerable.Empty<string>()
                : input.Split(',', StringSplitOptions.RemoveEmptyEntries)
                       .Select(x => x.Trim());
        }
    }
}