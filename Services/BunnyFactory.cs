using BenMakesGames.PlayPlayMini.Attributes.DI;

using wolf_island.Models;

namespace wolf_island.Services;

[AutoRegister(Lifetime.Singleton)]
public sealed class BunnyFactory
{
    public Bunny CreateBunny(Vector2 position)
    {
        return new Bunny()
        {
            Position = new Vector2(position.X, position.Y),
        };
    }
}