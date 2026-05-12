using LifeTag.Core.Entities;
using System;
using System.Threading.Tasks;

namespace LifeTag.Core.Interfaces
{
    public interface IUserProfileRepository
    {
        Task<UserProfile?> GetByUserIdAsync(int userId);
        Task<UserProfile?> GetByTagGuidAsync(Guid tagGuid);
        Task AddAsync(UserProfile profile);
        Task UpdateAsync(UserProfile profile);
        Task SaveChangesAsync();
        
        // Helper to remove existing sections (useful for template resets)
        Task ClearSectionsAsync(int profileId);
    }
}
