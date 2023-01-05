namespace wolf_island.Models;

#nullable enable

public sealed class Animals
{
    public Bunny? Bunny { get; set; } = null;
    public Wolfie? Wolfie { get; set; } = null;
    public Wolf? Wolf { get; set; } = null;

    public bool IsEmpty => Bunny is null && Wolfie is null && Wolf is null;
}
