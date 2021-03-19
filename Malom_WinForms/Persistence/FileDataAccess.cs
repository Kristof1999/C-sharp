using System;
using System.IO;
using System.Threading.Tasks;

namespace Persistence
{
    public class FileDataAccess : IPersistence
    {
        public async Task<Helper> LoadAsync(string path)
        {
            using(StreamReader reader = new StreamReader(path))
            {
                Helper helper = new Helper();
                var items = (await reader.ReadLineAsync()).Split(' ');
                helper.curPlayer = Convert.ToInt32(items[0]);
                helper.player1FigureCount = Convert.ToInt32(items[1]);
                helper.player2FigureCount = Convert.ToInt32(items[2]);
                helper.canAttack = items[3] == "true";
                helper.table = new Table();
                for(int i = 0; i < 7; i++)
                {
                    string line = await reader.ReadLineAsync();
                    var fields = line.Split(' ');
                    for (int j = 0; j < 7; j++)
                    {
                        helper.table.table[i, j].value = (FIELDS)Convert.ToInt32(fields[j]);
                    }
                }
                return helper;
            }
        }
        public async Task SaveAsync(string path, Helper helper)
        {
            using(StreamWriter writer = new StreamWriter(path))
            {
                await writer.WriteAsync(helper.curPlayer + " " + helper.player1FigureCount + " " + helper.player2FigureCount + " " + (helper.canAttack == true ? "true" : "false") + "\n");
                for(int i = 0; i < 7; i++)
                {
                    for(int j = 0; j < 7; j++)
                    {
                        await writer.WriteAsync((int)helper.table.table[i, j].value + " ");
                    }
                    await writer.WriteAsync("\n");
                }
            }
        }
    }
}
