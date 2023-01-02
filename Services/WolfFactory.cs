using BenMakesGames.PlayPlayMini.Attributes.DI;

using wolf_island.Models;

namespace wolf_island.Services;

[AutoRegister(Lifetime.Singleton)]
public sealed class WolfFactory
{
    public Wolf CreateWolf(Vector2 position)
    {
        return new Wolf()
        {
            Position = new Vector2(position.X, position.Y),
        };
    }
}