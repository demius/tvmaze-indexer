namespace TvMaze.Api.Types;

public class Person
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Url { get; set; }

    public DateTime Birthday { get; set; }

    public DateTime? DeathDay { get; set; }

    public string? Gender { get; set; }

    public Country? Country { get; set; }

    public uint Updated { get; set; }
}