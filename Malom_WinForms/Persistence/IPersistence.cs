using System;
using System.Threading.Tasks;

namespace Persistence
{
    public struct Helper
    {
        public Table table;
        public int curPlayer, player1FigureCount, player2FigureCount;
        public bool canAttack;
    }
    public interface IPersistence
    {
        Task<Helper> LoadAsync(String path);
        Task SaveAsync(String path, Helper helper);
    }
}
