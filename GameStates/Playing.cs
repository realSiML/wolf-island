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
    private Animals[][] Animals { get; set; }
    // private int BunnyStartCount { get; set; }
    // private int WolfStartCount { get; set; }
    // private int WolfieStartCount { get; set; }

    private Random Random { get; } = new();
    private readonly int GRID_MARGIN_X;
    private readonly int GRID_MARGIN_Y;

    private const int GRID_DIM = 20;
    private const float SPRITE_SCALE = 0.8f;
    private const float BUNNY_CHANCE = 0.2f;
    private const int BUNNY_START_COUNT = 10;
    private const int WOLFIE_START_COUNT = 10;
    private const int WOLF_START_COUNT = 10;
    private const double DELAY = 500;

    private bool IsRunning { get; set; }
    private double DeltaTime { get; set; } = DELAY;

    public Playing(GraphicsManager graphics, GameStateManager gsm, KeyboardManager keyboard, MouseManager mouse, CellFactory cellFactory, BunnyFactory bunnyFactory, WolfFactory wolfFactory, WolfieFactory wolfieFactory)
    {
        Graphics = graphics;

        GSM = gsm;

        Keyboard = keyboard;
        Mouse = mouse;
        Mouse.UseSystemCursor();

        CellFactory = cellFactory;
        BunnyFactory = bunnyFactory;
        WolfFactory = wolfFactory;
        WolfieFactory = wolfieFactory;

        Cells = Array.Empty<Cell[]>();
        Animals = Array.Empty<Animals[]>();

        GRID_MARGIN_X = (Graphics.Width - GRID_DIM * (int)(Cell.SIZE * SPRITE_SCALE)) / 2;
        if (GRID_MARGIN_X < 0) GRID_MARGIN_X = 0;

        GRID_MARGIN_Y = (Graphics.Height - GRID_DIM * (int)(Cell.SIZE * SPRITE_SCALE)) / 2 + 25;
        if (GRID_MARGIN_Y < 0) GRID_MARGIN_Y = 0;

        Restart();
    }

    public override void ActiveInput(GameTime gameTime)
    {
        if (Keyboard.PressedKey(Keys.Escape))
        {
            GSM.Exit();
        }
        else if (Keyboard.PressedKey(Keys.R))
        {
            Restart();
        }

        if (Mouse.LeftClicked)
        {
            IsRunning = IsRunning == false;
        }
    }

    public override void ActiveUpdate(GameTime gameTime)
    {
        // TODO: update game objects based on user input, AI logic, etc
    }

    public override void AlwaysUpdate(GameTime gameTime)
    {
        if (!IsRunning) return;

        if (DeltaTime <= 0)
        {
            Tick();
            DeltaTime = DELAY;
        }
        else
        {
            DeltaTime -= gameTime.ElapsedGameTime.TotalMilliseconds;
        }


        // System.Console.WriteLine(gameTime.ElapsedGameTime.TotalMilliseconds);
    }

    public override void ActiveDraw(GameTime gameTime)
    {
        Mouse.ActiveDraw(gameTime);
    }

    public override void AlwaysDraw(GameTime gameTime)
    {
        int bunnies = 0;
        int wolfies = 0;
        int wolfs = 0;

        Graphics.Clear(Color.LightGray);

        for (int i = 0; i < GRID_DIM; i++)
        {
            for (int j = 0; j < GRID_DIM; j++)
            {
                var cell = Cells[i][j];
                DrawCell(cell);

                var animals = Animals[i][j];

                if (animals.Bunny is not null)
                {
                    DrawBunny(animals.Bunny);
                    bunnies++;
                }

                if (animals.Wolfie is not null)
                {
                    DrawWolfie(animals.Wolfie);
                    Graphics.DrawText("Font",
                                      Cells[i][j].PixelX - (int)(Cell.SIZE * SPRITE_SCALE / 2),
                                      Cells[i][j].PixelY - (int)(Cell.SIZE * SPRITE_SCALE / 2),
                                      animals.Wolfie.Points.ToString(),
                                      Color.Black);
                    wolfies++;
                }

                if (animals.Wolf is not null)
                {
                    DrawWolf(animals.Wolf);
                    Graphics.DrawText("Font",
                                      Cells[i][j].PixelX - (int)(Cell.SIZE * SPRITE_SCALE / 2),
                                      Cells[i][j].PixelY,
                                      animals.Wolf.Points.ToString(),
                                      Color.Black);
                    wolfs++;
                }
            }
        }

        var ch = Graphics.Fonts["Font"].CharacterHeight;
        var margin = 20;
        var offset = Graphics.Width - (Graphics.Width - (int)(GRID_DIM * Cell.SIZE * SPRITE_SCALE)) / 2;
        Graphics.DrawText("Font", offset, margin, $"Bunnies:{bunnies}", Color.Black);
        Graphics.DrawText("Font", offset, margin + ch, $"Wolfies:{wolfies}", Color.Black);
        Graphics.DrawText("Font", offset, margin + ch * 2, $"Wolfs:  {wolfs}", Color.Black);
    }

    // *** МЕТОДЫ ***
    public void Restart()
    {
        IsRunning = false;

        Cells = new Cell[GRID_DIM][];
        Animals = new Animals[GRID_DIM][];
        for (int i = 0; i < GRID_DIM; i++)
        {
            Cells[i] = new Cell[GRID_DIM];
            Animals[i] = new Animals[GRID_DIM];
            for (int j = 0; j < GRID_DIM; j++)
            {
                Cells[i][j] = CellFactory.CreateCell(new Vector2(GRID_MARGIN_X + j * Cell.SIZE * SPRITE_SCALE, GRID_MARGIN_Y + i * Cell.SIZE * SPRITE_SCALE));
                Animals[i][j] = new();
            }
        }

        for (int i = 0; i < BUNNY_START_COUNT; i++)
        {
            while (true)
            {
                var x = Random.Next(GRID_DIM);
                var y = Random.Next(GRID_DIM);

                var animals = Animals[y][x];
                if (animals.Bunny is null)
                {
                    animals.Bunny = BunnyFactory.CreateBunny(Cells[y][x].Position);
                    break;
                }
            }
        }

        for (int i = 0; i < WOLFIE_START_COUNT; i++)
        {
            while (true)
            {
                var x = Random.Next(GRID_DIM);
                var y = Random.Next(GRID_DIM);

                var animals = Animals[y][x];
                if (animals.Bunny is null && animals.Wolfie is null)
                {
                    animals.Wolfie = WolfieFactory.CreateWolfie(Cells[y][x].Position);
                    break;
                }
            }
        }

        for (int i = 0; i < WOLF_START_COUNT; i++)
        {
            while (true)
            {
                var x = Random.Next(GRID_DIM);
                var y = Random.Next(GRID_DIM);

                var animals = Animals[y][x];
                if (animals.Bunny is null && animals.Wolfie is null && animals.Wolf is null)
                {
                    animals.Wolf = WolfFactory.CreateWolf(Cells[y][x].Position);
                    break;
                }
            }
        }
    }

    private void Tick()
    {
        var tempAnimals = new Animals[GRID_DIM][];
        for (int i = 0; i < GRID_DIM; i++)
        {
            tempAnimals[i] = new Animals[GRID_DIM];
            for (int j = 0; j < GRID_DIM; j++)
            {
                tempAnimals[i][j] = new();
            }
        }

        for (int i = 0; i < GRID_DIM; i++)
        {
            for (int j = 0; j < GRID_DIM; j++)
            {
                var animals = Animals[i][j];

                if (animals.IsEmpty)
                {
                    continue;
                }

                if (animals.Bunny is not null) BunnyTick((i, j), ref tempAnimals);
                if (animals.Wolfie is not null) WolfieTick((i, j), ref tempAnimals);
                if (animals.Wolf is not null) WolfTick((i, j), ref tempAnimals);
            }
        }

        Animals = tempAnimals;

        for (int i = 0; i < GRID_DIM; i++)
        {
            for (int j = 0; j < GRID_DIM; j++)
            {
                if (!Animals[i][j].IsEmpty) EndTick((i, j));
            }
        }

        // * Пробегаем по оригиналу
        // * Встречаем животного -> что-то с ним делаем
        // *    Не встречаем -> пропускаем
        // * Действие отобразили на копии (если нужные клетки не заняты)
        // *    Если клетки заняты, делаем по другому
        // *    Если не можем по другому -> пропускаем
    }

    public void EndTick((int, int) index)
    {
        var (i, j) = index;

        // Кролик и кто-то
        if (Animals[i][j].Bunny is not null && (Animals[i][j].Wolfie is not null || Animals[i][j].Wolf is not null))
        {
            if (Animals[i][j].Wolfie is not null && Animals[i][j].Wolf is not null)
            {
                if (Random.Next(2) < 1)
                {
                    Animals[i][j].Wolfie.Points += 10;
                }
                else
                {
                    Animals[i][j].Wolf.Points += 10;
                }
            }
            else if (Animals[i][j].Wolfie is not null)
            {
                Animals[i][j].Wolfie.Points += 10;
            }
            else if (Animals[i][j].Wolf is not null)
            {
                Animals[i][j].Wolf.Points += 10;
            }
            Animals[i][j].Bunny = null;
        }
        // 2 или 1 волка
        else if (Animals[i][j].Bunny is null && (Animals[i][j].Wolfie is not null || Animals[i][j].Wolf is not null))
        {
            if (Animals[i][j].Wolfie is not null)
            {
                Animals[i][j].Wolfie.Points -= 1;
                if (Animals[i][j].Wolfie.Points <= 0) Animals[i][j].Wolfie = null;
            }
            if (Animals[i][j].Wolf is not null)
            {
                Animals[i][j].Wolf.Points -= 1;
                if (Animals[i][j].Wolf.Points <= 0) Animals[i][j].Wolf = null;
            }
        }
        // В конце Волк и Волчица -> потомство
        if (Animals[i][j].Wolf is null || Animals[i][j].Wolfie is null) return;

        var isEmptyNeighbor = false;
        // Проверка: есть ли свободные соседние клетки
        for (int ii = -1; ii <= 1; ii++)
        {
            for (int jj = -1; jj <= 1; jj++)
            {
                var row = i + ii;
                var col = j + jj;
                if (row < 0 || row >= GRID_DIM || col < 0 || col >= GRID_DIM) continue;

                if (Animals[row][col].Wolfie is null || Animals[row][col].Wolf is null)
                {
                    isEmptyNeighbor = true;
                    break;
                }
            }
        }

        if (!isEmptyNeighbor) return;

        while (true)
        {
            var rand = Random.Next(9);
            var ii = (rand / 3) - 1; // -1, 0, 1
            var jj = (rand % 3) - 1; // -1, 0, 1
            var (row, col) = (i + ii, j + jj);

            if (row < 0 || row >= GRID_DIM || col < 0 || col >= GRID_DIM) continue;

            if (Animals[row][col].Wolf is not null && Animals[row][col].Wolfie is not null) continue;

            // Создаем нового
            do
            {
                if (Random.Next(2) < 1)
                {
                    if (Animals[row][col].Wolf is null)
                    {
                        Animals[row][col].Wolf = WolfFactory.CreateWolf(Cells[row][col].Position);
                    }
                }
                else
                {
                    if (Animals[row][col].Wolfie is null)
                    {
                        Animals[row][col].Wolfie = WolfieFactory.CreateWolfie(Cells[row][col].Position);
                    }
                }
            } while (Animals[row][col].Wolf is null && Animals[row][col].Wolfie is null);
            break;
        }
    }

    public void BunnyTick((int, int) index, ref Animals[][] tempAnimals)
    {
        var (i, j) = index;
        var isEmptyNeighbor = false;

        // Проверка: есть ли свободные соседние клетки
        for (int ii = -1; ii <= 1; ii++)
        {
            for (int jj = -1; jj <= 1; jj++)
            {
                var row = i + ii;
                var col = j + jj;
                if (row < 0 || row >= GRID_DIM || col < 0 || col >= GRID_DIM) continue;

                if (Animals[row][col].Bunny is null)
                {
                    isEmptyNeighbor = true;
                    break;
                }
            }
        }

        if (!isEmptyNeighbor)
        {
            tempAnimals[i][j].Bunny = Animals[i][j].Bunny;
            return;
        }

        // Передвижение
        while (true)
        {
            var rand = Random.Next(9);
            var ii = (rand / 3) - 1; // -1, 0, 1
            var jj = (rand % 3) - 1; // -1, 0, 1
            var (row, col) = (i + ii, j + jj);

            if (row < 0 || row >= GRID_DIM || col < 0 || col >= GRID_DIM) continue;

            if (row == i && col == j)
            {
                if (tempAnimals[row][col].Bunny is null)
                {
                    tempAnimals[row][col].Bunny = Animals[i][j].Bunny;
                    break;
                }

                continue;
            }

            if (Animals[row][col].Bunny is null && tempAnimals[row][col].Bunny is null)
            {
                // Передвигаем
                tempAnimals[row][col].Bunny = Animals[i][j].Bunny;
                tempAnimals[row][col].Bunny.Position = Cells[row][col].Position;
                // Позиция поменялась
                (i, j) = (row, col);
                break;
            }
        }

        // Проверка: есть ли свободные соседние клетки
        for (int ii = -1; ii <= 1; ii++)
        {
            for (int jj = -1; jj <= 1; jj++)
            {
                var row = i + ii;
                var col = j + jj;
                if (row < 0 || row >= GRID_DIM || col < 0 || col >= GRID_DIM) continue;

                var targetAnimals = Animals[row][col];
                if (targetAnimals.Bunny is null)
                {
                    isEmptyNeighbor = true;
                    break;
                }
            }
        }

        if (!isEmptyNeighbor)
        {
            tempAnimals[i][j].Bunny = Animals[i][j].Bunny;
            return;
        }

        // Размножение
        if (Random.NextSingle() <= BUNNY_CHANCE)
        {
            while (true)
            {
                var rand = Random.Next(9);
                var ii = (rand / 3) - 1; // -1, 0, 1
                var jj = (rand % 3) - 1; // -1, 0, 1
                var (row, col) = (i + ii, j + jj);

                if (row < 0 || row >= GRID_DIM || col < 0 || col >= GRID_DIM) continue;

                if (Animals[row][col].Bunny is null || tempAnimals[row][col] is null)
                {

                    // Создаем нового
                    tempAnimals[row][col].Bunny = BunnyFactory.CreateBunny(Cells[row][col].Position);
                    break;
                }
            }
        }
    }

    public void WolfieTick((int, int) index, ref Animals[][] tempAnimals)
    {
        var (i, j) = index;
        var isEmptyNeighbor = false;
        var hasMoved = false;

        // Проверка: есть ли свободные соседние клетки
        for (int ii = -1; ii <= 1; ii++)
        {
            if (hasMoved) break;
            for (int jj = -1; jj <= 1; jj++)
            {
                var row = i + ii;
                var col = j + jj;
                if (row < 0 || row >= GRID_DIM || col < 0 || col >= GRID_DIM) continue;

                if (Animals[row][col].Wolfie is null)
                {
                    isEmptyNeighbor = true;
                    // Поиск кролика
                    if (Animals[row][col].Bunny is not null)
                    {
                        // Передвигаем
                        tempAnimals[row][col].Wolfie = Animals[i][j].Wolfie;
                        tempAnimals[row][col].Wolfie.Position = Cells[row][col].Position;
                        hasMoved = true;
                        break;
                    }
                }
            }
        }

        if (hasMoved) return;

        if (!isEmptyNeighbor)
        {
            tempAnimals[i][j].Wolfie = Animals[i][j].Wolfie;
            return;
        }

        // Случайное передвижение
        while (true)
        {
            var rand = Random.Next(9);
            var ii = (rand / 3) - 1; // -1, 0, 1
            var jj = (rand % 3) - 1; // -1, 0, 1
            var (row, col) = (i + ii, j + jj);

            if (row < 0 || row >= GRID_DIM || col < 0 || col >= GRID_DIM) continue;

            if (row == i && col == j)
            {
                if (tempAnimals[row][col].Wolfie is null)
                {
                    tempAnimals[row][col].Wolfie = Animals[i][j].Wolfie;
                    break;
                }

                continue;
            }

            if (Animals[row][col].Wolfie is null && tempAnimals[row][col].Wolfie is null)
            {
                // Передвигаем
                tempAnimals[row][col].Wolfie = Animals[i][j].Wolfie;
                tempAnimals[row][col].Wolfie.Position = Cells[row][col].Position;
                break;
            }
        }
    }

    public void WolfTick((int, int) index, ref Animals[][] tempAnimals)
    {
        var (i, j) = index;
        var isEmptyNeighbor = false;
        var hasMoved = false;

        // Проверка: есть ли свободные соседние клетки
        for (int ii = -1; ii <= 1; ii++)
        {
            if (hasMoved) break;
            for (int jj = -1; jj <= 1; jj++)
            {
                var row = i + ii;
                var col = j + jj;
                if (row < 0 || row >= GRID_DIM || col < 0 || col >= GRID_DIM) continue;

                if (Animals[row][col].Wolf is null)
                {
                    isEmptyNeighbor = true;
                    // Поиск кролика
                    if (Animals[row][col].Bunny is not null)
                    {
                        // Передвигаем
                        tempAnimals[row][col].Wolf = Animals[i][j].Wolf;
                        tempAnimals[row][col].Wolf.Position = Cells[row][col].Position;
                        hasMoved = true;
                        break;
                    }
                }
            }
        }

        if (hasMoved) return;

        if (!isEmptyNeighbor)
        {
            tempAnimals[i][j].Wolf = Animals[i][j].Wolf;
            return;
        }

        // Ищем волчицу
        for (int ii = -1; ii <= 1; ii++)
        {
            if (hasMoved) break;
            for (int jj = -1; jj <= 1; jj++)
            {
                var row = i + ii;
                var col = j + jj;
                if (row < 0 || row >= GRID_DIM || col < 0 || col >= GRID_DIM) continue;

                if (Animals[row][col].Wolf is null && tempAnimals[row][col].Wolf is null)
                {
                    if (Animals[row][col].Wolfie is not null)
                    {
                        // Передвигаем
                        tempAnimals[row][col].Wolf = Animals[i][j].Wolf;
                        tempAnimals[row][col].Wolf.Position = Cells[row][col].Position;
                        hasMoved = true;
                        break;
                    }
                }
            }
        }

        if (hasMoved) return;

        // Случайное передвижение
        while (true)
        {
            var rand = Random.Next(9);
            var ii = (rand / 3) - 1; // -1, 0, 1
            var jj = (rand % 3) - 1; // -1, 0, 1
            var (row, col) = (i + ii, j + jj);

            if (row < 0 || row >= GRID_DIM || col < 0 || col >= GRID_DIM) continue;

            if (row == i && col == j)
            {
                if (tempAnimals[row][col].Wolf is null)
                {
                    tempAnimals[row][col].Wolf = Animals[i][j].Wolf;
                    break;
                }

                continue;
            }

            if (Animals[row][col].Wolf is null && tempAnimals[row][col].Wolf is null)
            {
                // Передвигаем
                tempAnimals[row][col].Wolf = Animals[i][j].Wolf;
                tempAnimals[row][col].Wolf.Position = Cells[row][col].Position;
                break;
            }
        }
    }

    // * Draw Методы *
    public void DrawCell(Cell cell)
    {
        Graphics.DrawSpriteRotatedAndScaled("Cell", cell.PixelX, cell.PixelY, 0, 0, SPRITE_SCALE, Color.White);
    }

    public void DrawBunny(Bunny bunny)
    {
        Graphics.DrawSpriteRotatedAndScaled("Bunny", bunny.PixelX, bunny.PixelY, 0, 0, SPRITE_SCALE, Color.White);
    }

    public void DrawWolf(Wolf wolf)
    {
        Graphics.DrawSpriteRotatedAndScaled("Wolf", wolf.PixelX, wolf.PixelY, 0, 0, SPRITE_SCALE, Color.White);
    }

    public void DrawWolfie(Wolfie wolfie)
    {
        Graphics.DrawSpriteRotatedAndScaled("Wolf", wolfie.PixelX, wolfie.PixelY, 1, 0, SPRITE_SCALE, Color.White);
    }

}