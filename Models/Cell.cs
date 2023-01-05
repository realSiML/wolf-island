namespace wolf_island.Models;

public sealed class Cell
{
    public static int SIZE => 64;

    public Vector2 Position { get; set; }
    public int PixelX => (int)Position.X;
    public int PixelY => (int)Position.Y;

}