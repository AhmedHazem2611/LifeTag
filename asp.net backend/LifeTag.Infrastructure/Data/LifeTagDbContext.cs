using LifeTag.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace LifeTag.Infrastructure.Data
{
    public class LifeTagDbContext : DbContext
    {
        public LifeTagDbContext(DbContextOptions<LifeTagDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<ProfileSection> ProfileSections { get; set; }
        public DbSet<ProfileEntry> ProfileEntries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User Configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
                entity.Property(e => e.PasswordHash).IsRequired();
            });

            // Tag Configuration
            modelBuilder.Entity<Tag>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Guid).IsUnique();
                entity.HasIndex(e => e.UserId).IsUnique(); // Enforce 1:1 User-Tag relationship

                entity.Property(e => e.Guid).IsRequired();
                entity.Property(e => e.Pin).IsRequired();
                entity.Property(e => e.IsPinProtected).HasDefaultValue(true);
                entity.Property(e => e.IsActive).HasDefaultValue(false);

                // Relationship: One User has One Tag (Optional)
                entity.HasOne(t => t.User)
                    .WithOne(u => u.ActiveTag)
                    .HasForeignKey<Tag>(t => t.UserId)
                    .OnDelete(DeleteBehavior.SetNull); // Re-assignable
            });

            // UserProfile Configuration
            modelBuilder.Entity<UserProfile>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.UserId).IsUnique();

                entity.Property(e => e.TemplateType).IsRequired().HasMaxLength(50);

                // Relationship: One User has One Profile (Required)
                entity.HasOne(p => p.User)
                    .WithOne(u => u.Profile)
                    .HasForeignKey<UserProfile>(p => p.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ProfileSection Configuration
            modelBuilder.Entity<ProfileSection>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(100);
                entity.Property(e => e.SectionType).IsRequired().HasMaxLength(50);
                entity.Property(e => e.DisplayOrder).HasDefaultValue(0);

                // Relationship: Profile 1:N Section
                entity.HasOne(s => s.UserProfile)
                    .WithMany(p => p.Sections)
                    .HasForeignKey(s => s.UserProfileId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ProfileEntry Configuration
            modelBuilder.Entity<ProfileEntry>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.DataJson).IsRequired();

                // Relationship: Section 1:N Entry
                entity.HasOne(e => e.ProfileSection)
                    .WithMany(s => s.Entries)
                    .HasForeignKey(e => e.ProfileSectionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Seed Initial Tags (5 records) with fresh GUIDs and Pin "1234"
            modelBuilder.Entity<Tag>().HasData(
                new Tag { Id = 1, Guid = new Guid("b1e84d41-35b8-4c3e-9088-2c26f04f29a0"), Pin = "1234", IsActive = false, UserId = null },
                new Tag { Id = 2, Guid = new Guid("c3a881e1-e17f-4422-901d-5baab2c792f9"), Pin = "1234", IsActive = false, UserId = null },
                new Tag { Id = 3, Guid = new Guid("a9c14bc6-8d69-450f-90db-cd0811eef2a8"), Pin = "1234", IsActive = false, UserId = null },
                new Tag { Id = 4, Guid = new Guid("ff9c0179-11ba-447f-859a-f4327ea6eeb4"), Pin = "1234", IsActive = false, UserId = null },
                new Tag { Id = 5, Guid = new Guid("d2e5a781-bc5a-474c-83b0-6ef184a4f826"), Pin = "1234", IsActive = false, UserId = null }
            );
        }
    }
}
