using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace Malom.Persistence
{
    public interface IPersistence
    {
        Task<Helper> LoadAsync(String path);
        Task SaveAsync(String path, Helper helper);
        Task<ICollection<SaveEntry>> ListAsync();
    }
}
