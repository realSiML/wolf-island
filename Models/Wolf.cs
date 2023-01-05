namespace wolf_island.Models;

public sealed class Wolf : Animal
{
    public int Points { get; set; } = 10;
    public override Vector2 OFFSET => new(Cell.SIZE / 9, Cell.SIZE / 5);
}