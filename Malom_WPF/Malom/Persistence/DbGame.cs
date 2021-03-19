using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Malom.Persistence
{
    public class DbGame
    {
        [Key]
        [MaxLength(32)]
        public string Name { get; set; }
        public int CurPlayer { get; set; }
        public int Player1FigureCount { get; set; }
        public int Player2FigureCount { get; set; }
        public Boolean CanAttack { get; set; }
        public List<DbField> Fields { get; set; }
        public DateTime Time { get; set; }
        public DbGame()
        {
            Fields = new List<DbField>();
            Time = DateTime.Now;
        }
    }
}
