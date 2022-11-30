using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TvMaze.Scraper.Data.Model;

public class Person
{
    public int PersonId { get; set; }
    
    public string Url { get; set; }
    
    public string Name { get; set; }
    
    public DateTime Birthday { get; set; }

    public uint LastUpdated { get; set; }
    
    public ICollection<TvShow> TvShows { get; set; }
}