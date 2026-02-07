using LifeTag.Models;
using Microsoft.EntityFrameworkCore;

namespace LifeTag.Services
{
    public class UserService
    {
        private readonly LifeTagContext _context;

        public UserService(LifeTagContext context)
        {
            _context = context;
        }

        public User AddUser(User user)
        {
            _context.Add(user);
            _context.SaveChanges();
            return user;
        }

        public void RemoveUser(User user)
        {
            _context.Remove(user);
            _context.SaveChanges();
        }
        public void UpdateUser(User user)
        {
            _context.Update(user);
            _context.SaveChanges();
        }
        public User? GetUserByTagId(Guid tagId)
        {
            return _context.Users
                .Include(u => u.MedicalConditions)
                .Include(u => u.Medications)
                .Include(u => u.Allergies)
                .Include(u => u.EmergencyContacts)
                .Include(u => u.MedicalNotes)
                .FirstOrDefault(u => u.TagId == tagId);
        }

        public MedicalProfile? GetEmergencyProfile(Guid tagId)
        {
            var user = GetUserByTagId(tagId);

            if (user == null)
                return null;

            return new MedicalProfile
            {
                FullName = user.FullName,
                BloodType = user.BloodType,
                MedicalConditions = user.MedicalConditions,
                Medications = user.Medications,
                Allergies = user.Allergies,
                EmergencyContacts = user.EmergencyContacts,
                MedicalNotes = user.MedicalNotes
            };
        }

    }
}