using Microsoft.EntityFrameworkCore;

namespace Malom.Persistence
{
    public class GameContext : DbContext
    {
        public GameContext(DbContextOptions<GameContext> options) : base(options)
        {
        }

        public DbSet<DbGame> Games { get; set; }
        public DbSet<DbField> Fields { get; set; }
    }
}
