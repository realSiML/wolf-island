using BenMakesGames.PlayPlayMini.Attributes.DI;

using wolf_island.Models;

namespace wolf_island.Services;

[AutoRegister(Lifetime.Singleton)]
public sealed class CellFactory
{
    public Cell CreateCell(Vector2 position)
    {
        return new Cell()
        {
            Position = new Vector2(position.X, position.Y),
        };
    }
}