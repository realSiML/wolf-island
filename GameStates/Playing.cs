using BenMakesGames.PlayPlayMini;
using BenMakesGames.PlayPlayMini.Services;

using Microsoft.Xna.Framework.Input;

using wolf_island.Models;
using wolf_island.Services;

namespace wolf_island.GameStates;

// sealed classes execute faster than non-sealed, so always seal your game states!
public sealed class Playing : GameState
{
    private GraphicsManager Graphics { get; }
    private KeyboardManager Keyboard { get; }
    private MouseManager Mouse { get; }
    private GameStateManager GSM { get; }
    private CellFactory CellFactory { get; }
    private BunnyFactory BunnyFactory { get; }
    private WolfFactory WolfFactory { get; }
    private WolfieFactory WolfieFactory { get; }

    private Cell[][] Cells { get; set; }
    private const int GRID_DIM = 20;
    private readonly int GRID_MARGIN_X;
    private readonly int GRID_MARGIN_Y;

    private int BunnyStartCount { get; set; }
    private int WolfStartCount { get; set; }
    private int WolfieStartCount { get; set; }

    private Random Random { get; } = new();

    public Playing(GraphicsManager graphics, GameStateManager gsm, KeyboardManager keyboard, MouseManager mouse, CellFactory cellFactory, BunnyFactory bunnyFactory, WolfFactory wolfFactory, WolfieFactory wolfieFactory)
    {
        Graphics = graphics;
        Graphics.SetFullscreen(true);

        GSM = gsm;
        Keyboard = keyboard;
        Mouse = mouse;
        Mouse.UseSystemCursor();

        CellFactory = cellFactory;
        BunnyFactory = bunnyFactory;
        WolfFactory = wolfFactory;
        WolfieFactory = wolfieFactory;

        Cells = Array.Empty<Cell[]>();

        GRID_MARGIN_X = (Graphics.Width - GRID_DIM * Cell.SIZE) / 2;
        if (GRID_MARGIN_X < 0) GRID_MARGIN_X = 0;

        GRID_MARGIN_Y = (Graphics.Height - GRID_DIM * Cell.SIZE) / 2;
        if (GRID_MARGIN_Y < 0) GRID_MARGIN_Y = 0;

        BunnyStartCount = 100;
        WolfStartCount = 100;
        WolfieStartCount = 100;

        Restart();
    }

    public override void ActiveInput(GameTime gameTime)
    {
        if (Keyboard.PressedKey(Keys.Escape))
        {
            GSM.Exit();
        }
    }

    public override void ActiveUpdate(GameTime gameTime)
    {
        // TODO: update game objects based on user input, AI logic, etc
    }

    public override void AlwaysUpdate(GameTime gameTime)
    {
        var cellsCopy = Cells.ToArray();

        for (int i = 0; i < GRID_DIM; i++)
        {
            for (int j = 0; j < GRID_DIM; j++)
            {
                var animals = Cells[i][j].Animals;

                for (int ii = -1; ii <= 1; ii++)
                {
                    for (int jj = -1; jj < 1; jj++)
                    {
                        var row = i + ii;
                        var col = j + jj;

                        if (row < 0 || row >= GRID_DIM || col < 0 || col >= GRID_DIM) continue;

                        // Bunny Move

                    }
                }
            }
        }
    }

    public override void ActiveDraw(GameTime gameTime)
    {
        Mouse.ActiveDraw(gameTime);
    }

    public override void AlwaysDraw(GameTime gameTime)
    {
        Graphics.Clear(Color.LightGray);

        foreach (var cells in Cells)
        {
            foreach (var cell in cells)
            {
                DrawCell(cell);

                if (cell.Animals[0] is not null) DrawBunny((Bunny)cell.Animals[0]);
                if (cell.Animals[1] is not null) DrawWolfie((Wolfie)cell.Animals[1]);
                if (cell.Animals[2] is not null) DrawWolf((Wolf)cell.Animals[2]);
            }
        }
    }

    // *** МЕТОДЫ ***
    public void Restart()
    {
        Cells = new Cell[GRID_DIM][];
        for (int i = 0; i < GRID_DIM; i++)
        {
            Cells[i] = new Cell[GRID_DIM];
            for (int j = 0; j < GRID_DIM; j++)
            {
                Cells[i][j] = CellFactory.CreateCell(new Vector2(GRID_MARGIN_X + j * Cell.SIZE, GRID_MARGIN_Y + i * Cell.SIZE));
            }
        }

        for (int i = 0; i < BunnyStartCount; i++)
        {
            while (true)
            {
                var x = Random.Next(GRID_DIM);
                var y = Random.Next(GRID_DIM);

                var animals = Cells[y][x].Animals;
                if (!ArrayHas(animals, typeof(Bunny)))
                {
                    var position = Cells[y][x].Position;
                    ArrayAdd(animals, BunnyFactory.CreateBunny(position));
                    break;
                }
            }
        }

        for (int i = 0; i < WolfieStartCount; i++)
        {
            while (true)
            {
                var x = Random.Next(GRID_DIM);
                var y = Random.Next(GRID_DIM);

                var animals = Cells[y][x].Animals;
                if (!ArrayHas(animals, typeof(Bunny)) && !ArrayHas(animals, typeof(Wolfie)))
                {
                    var position = Cells[y][x].Position;
                    ArrayAdd(animals, WolfieFactory.CreateWolfie(position));
                    break;
                }
            }
        }

        for (int i = 0; i < WolfStartCount; i++)
        {
            while (true)
            {
                var x = Random.Next(GRID_DIM);
                var y = Random.Next(GRID_DIM);

                var animals = Cells[y][x].Animals;
                if (!ArrayHas(animals, typeof(Bunny)) && !ArrayHas(animals, typeof(Wolfie)) && !ArrayHas(animals, typeof(Wolf)))
                {
                    var position = Cells[y][x].Position;
                    ArrayAdd(animals, WolfFactory.CreateWolf(position));
                    break;
                }
            }
        }
    }

    public static void ArrayAdd<T>(Animal[] animals, T animal) where T : Animal
    {
        switch (animal)
        {
            case Bunny:
                animals[0] = animal;
                break;
            case Wolfie:
                animals[1] = animal;
                break;
            case Wolf:
                animals[2] = animal;
                break;
        }
    }

    public static bool ArrayHas(Animal[] animals, Type type)
    {
        if (type == typeof(Bunny))
        {
            return animals[0] is not null;
        }
        else if (type == typeof(Wolfie))
        {
            return animals[1] is not null;
        }
        else if (type == typeof(Wolf))
        {
            return animals[2] is not null;
        }

        return false;
    }

    // * Draw Методы *
    public void DrawCell(Cell cell)
    {
        Graphics.DrawSpriteRotatedAndScaled("Cell", cell.PixelX, cell.PixelY, 0, 0, Cell.SCALE, Color.White);
    }

    public void DrawBunny(Bunny bunny)
    {
        Graphics.DrawSpriteRotatedAndScaled("Bunny", bunny.PixelX, bunny.PixelY, 0, 0, bunny.SCALE, Color.White);
    }

    public void DrawWolf(Wolf wolf)
    {
        Graphics.DrawSpriteRotatedAndScaled("Wolf", wolf.PixelX, wolf.PixelY, 0, 0, wolf.SCALE, Color.White);
    }

    public void DrawWolfie(Wolfie wolfie)
    {
        Graphics.DrawSpriteRotatedAndScaled("Wolf", wolfie.PixelX, wolfie.PixelY, 1, 0, wolfie.SCALE, Color.White);
    }

}