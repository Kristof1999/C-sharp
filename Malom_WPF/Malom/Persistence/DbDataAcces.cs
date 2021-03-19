using System.Threading.Tasks;
using System.Linq;
using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Malom.Persistence
{
    class DbDataAcces : IPersistence
    {
        private GameContext context;
        public DbDataAcces(DbContextOptions<GameContext> options)
        {
            context = new GameContext(options);
            context.Database.EnsureCreated();
        }
        public async Task<Helper> LoadAsync(string name)
        {
            try
            {
                DbGame game = await context.Games.Include(g => g.Fields).SingleAsync(g => g.Name == name);
                Helper helper = new Helper();
                helper.canAttack = game.CanAttack;
                helper.curPlayer = game.CurPlayer;
                helper.player1FigureCount = game.Player1FigureCount;
                helper.player2FigureCount = game.Player2FigureCount;
                helper.table = new Table();
                for(int i = 0; i < 7; i++)
                {
                    for(int j = 0; j < 7; j++)
                    {
                        foreach(DbField field in game.Fields)
                        {
                            if (field.Row == i && field.Col == j)
                                helper.table.table[i, j].value = field.Value;
                        }
                    }
                }
                return helper;
            }
            catch
            {
                throw new AccesException();
            }
        }

        public async Task SaveAsync(string name, Helper helper)
        {
            try
            {
                DbGame overwriteGame = await context.Games.Include(g => g.Fields).SingleOrDefaultAsync(g => g.Name == name);
                if (overwriteGame != null)
                    context.Games.Remove(overwriteGame);
                DbGame game = new DbGame
                {
                    Name = name,
                    CurPlayer = helper.curPlayer,
                    Player1FigureCount = helper.player1FigureCount,
                    Player2FigureCount = helper.player2FigureCount,
                    CanAttack = helper.canAttack,
                    Time = DateTime.Now
                };
                for (int i = 0; i < 7; i++)
                {
                    for(int j = 0; j < 7; j++)
                    {
                        DbField field = new DbField
                        {
                            Row = i,
                            Col = j,
                            Value = helper.table.table[i, j].value
                        };
                        game.Fields.Add(field);
                    }
                }
                context.Games.Add(game);
                await context.SaveChangesAsync();
            }
            catch
            {
                throw new AccesException();
            }
        }
        public async Task<ICollection<SaveEntry>> ListAsync()
        {
            try
            {
                return await context.Games
                    .OrderByDescending(g => g.Time)
                    .Select(g => new SaveEntry { Name = g.Name, Time = g.Time })
                    .ToListAsync();
            }
            catch
            {
                throw new AccesException();
            }
        }
    }
}
