using Microsoft.EntityFrameworkCore;

namespace LifeTag.Models
{
    public class LifeTagContext : DbContext
    {

        public LifeTagContext(DbContextOptions<LifeTagContext> options) : base(options) 
        { 

        }
        public DbSet<User> Users { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<MedicalCondition> MedicalConditions { get; set; }
        public DbSet<Allergy> Allergies { get; set; }
        public DbSet<Medication> Medications { get; set; }
        public DbSet<EmergencyContact> EmergencyContacts { get; set; }
        public DbSet<MedicalNote> MedicalNotes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasOne(u => u.Tag)
                .WithOne(t => t.User)
                .HasForeignKey<User>(u => u.TagId);
        }
    }
}
