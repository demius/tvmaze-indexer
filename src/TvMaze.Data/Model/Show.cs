using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TvMaze.Data.Model
{
    public class Show
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)] 
        public int ShowId { get; set; }
        
        [Required, MaxLength(255)]
        public string Name { get; set; }

        [Required]
        public string Url { get; set; }

        [Required]
        public uint LastUpdated { get; set; }
    }
}

