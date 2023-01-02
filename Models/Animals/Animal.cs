namespace wolf_island.Models;

public abstract class Animal
{
    public abstract float SCALE { get; }
    public abstract Vector2 OFFSET { get; }


    private Vector2 _position;
    public Vector2 Position
    {
        get => _position; set
        {
            _position = value + OFFSET;
        }
    }

    public int PixelX => (int)Position.X;
    public int PixelY => (int)Position.Y;
}
