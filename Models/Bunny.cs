namespace wolf_island.Models;

public sealed class Bunny : Animal
{
    public override Vector2 OFFSET => new(-Cell.SIZE / 5, -Cell.SIZE / 8);
}