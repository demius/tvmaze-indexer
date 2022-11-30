namespace TvMaze.Api.Types;

public class Person
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Url { get; set; }

    public DateTime? Birthday { get; set; }

    public uint Updated { get; set; }
}