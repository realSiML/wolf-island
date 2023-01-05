using Autofac;

using BenMakesGames.PlayPlayMini;
using BenMakesGames.PlayPlayMini.Model;

using wolf_island.GameStates;

// TODO: any pre-req setup, ex:
/*
 * var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
 * var appDataGameDirectory = @$"{appData}{Path.DirectorySeparatorChar}wolf_island";
 * 
 * Directory.CreateDirectory(appDataGameDirectory);
 */

var gsmBuilder = new GameStateManagerBuilder();

gsmBuilder
    .SetWindowSize(1920, 1080, 1)
    .SetInitialGameState<Startup>().UseFixedTimeStep()

    // TODO: set a better window title
    .SetWindowTitle("Волчий остров")

    // TODO: add any resources needed (refer to PlayPlayMini documentation for more info)
    .AddAssets(new IAsset[]
    {
        new FontMeta("Font","Font",11,23),
        // new PictureMeta(...)
        new SpriteSheetMeta("Wolf","Wolf",48,32),
        new SpriteSheetMeta("Bunny","Bunny",32,32),
        new SpriteSheetMeta("Cell","Cell",64,64),
        // new SongMeta(...)
        // new SoundEffectMeta(...)
    })

    // TODO: any additional service registration (refer to PlayPlayMini and/or Autofac documentation for more info)
    .AddServices(s =>
    {

    })
;

gsmBuilder.Run();