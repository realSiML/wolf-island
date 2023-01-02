using BenMakesGames.PlayPlayMini.Attributes.DI;

using wolf_island.Models;

namespace wolf_island.Services;

[AutoRegister(Lifetime.Singleton)]
public sealed class WolfieFactory
{
    public Wolfie CreateWolfie(Vector2 position)
    {
        return new Wolfie()
        {
            Position = new Vector2(position.X, position.Y),
        };
    }
}