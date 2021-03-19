using System;
using System.ComponentModel.DataAnnotations;

namespace Malom.Persistence
{
    public class DbField
    {
        [Key]
        public int Id { get; set; }
        public int Row { get; set; }
        public int Col { get; set; }
        public FIELDS Value { get; set; }
        public DbGame Game { get; set; }
    }
}
