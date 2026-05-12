using LifeTag.Core.Entities;
using LifeTag.Core.Interfaces;
using LifeTag.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LifeTag.Infrastructure.Repositories
{
    public class UserProfileRepository : IUserProfileRepository
    {
        private readonly LifeTagDbContext _context;

        public UserProfileRepository(LifeTagDbContext context)
        {
            _context = context;
        }

        public async Task<UserProfile?> GetByUserIdAsync(int userId)
        {
            return await _context.UserProfiles
                .Include(p => p.Sections)
                    .ThenInclude(s => s.Entries)
                .FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task<UserProfile?> GetByTagGuidAsync(Guid tagGuid)
        {
            // First find the user linked to the tag
            var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Guid == tagGuid && t.IsActive);
            if (tag == null || tag.UserId == null) return null;

            return await GetByUserIdAsync(tag.UserId.Value);
        }

        public async Task AddAsync(UserProfile profile)
        {
            await _context.UserProfiles.AddAsync(profile);
        }

        public async Task UpdateAsync(UserProfile profile)
        {
            _context.UserProfiles.Update(profile);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task ClearSectionsAsync(int profileId)
        {
            var sections = await _context.ProfileSections
                .Where(s => s.UserProfileId == profileId)
                .ToListAsync();

            _context.ProfileSections.RemoveRange(sections);
            // Entries will be deleted via Cascade delete
        }
    }
}
