namespace wolf_island.Models;

public sealed class Wolf : Animal
{
    public override float SCALE => 0.8f;
    public override Vector2 OFFSET => new(Cell.SIZE / 7, Cell.SIZE / 4);
}