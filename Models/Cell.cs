namespace wolf_island.Models;

public sealed class Cell
{
    public static readonly float SCALE = 0.8f;
    public static int SIZE => (int)(64 * SCALE);

    public Vector2 Position { get; set; }
    public int PixelX => (int)Position.X;
    public int PixelY => (int)Position.Y;

    public Animal[] Animals { get; } = new Animal[3];
    // 0 - Кролик, 1 - Волчица, 2 - Волк
}