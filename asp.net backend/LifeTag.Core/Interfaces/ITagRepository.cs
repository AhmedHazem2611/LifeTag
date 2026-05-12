using LifeTag.Core.Entities;
using System;
using System.Threading.Tasks;

namespace LifeTag.Core.Interfaces
{
    public interface ITagRepository
    {
        Task<Tag?> GetByGuidAsync(Guid guid);
        Task<Tag?> GetByUserIdAsync(int userId);
        Task UpdateAsync(Tag tag);
        Task SaveChangesAsync();
    }
}
