namespace TvMaze.Api.Types;

public class Character
{
    public int Id { get; set; }

    public string? Url { get; set; }

    public string? Name { get; set; }

    public Image? Image { get; set; }
}