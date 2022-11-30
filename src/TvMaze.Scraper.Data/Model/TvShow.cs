using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TvMaze.Scraper.Data.Model
{
    public class TvShow
    {
        public int TvShowId { get; set; }
        
        public string Name { get; set; }

        public string Url { get; set; }
        
        public uint LastUpdated { get; set; }
        
        public ICollection<Person> CastMembers { get; set; }
    }
}

