namespace wolf_island.Models;

public sealed class Bunny : Animal
{
    public override float SCALE => 0.8f;
    public override Vector2 OFFSET => new(-Cell.SIZE / 4, -Cell.SIZE / 8);
}