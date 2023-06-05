// Singleton
public class GameApp
{
    public static GameApp Instance { get; private set; }

    public Vector2 GridSize { get; private set; }
    public Vector2 GridPosition { get; private set; }

    public Vector2 ScoreGridSize { get; private set; }
    public Vector2 ScoreGridPosition { get; private set; }

    public Vector2 FoodBarGridSize { get; private set; }
    public Vector2 FoodBarGridPosition { get; private set; }

    // Where the cursor should be taken back to by default
    Vector2 defaultCursorPosition;

    // User inputs
    public ConsoleKeyInfo Input { get; set; }


    // All the scenes of the game
    private Stack<Scene> scenes;


    public Random Randomizer {get; private set; }


    private GameApp()
    {
        GridSize = new Vector2(55, 30);

        Vector2 position = new Vector2(Console.WindowWidth / 2 - GridSize.X / 2, Console.WindowHeight / 2 - GridSize.Y / 2);
        
        // Documentation
        GridPosition = new Vector2(position.X % 2 == 0? position.X : position.X + 1, position.Y);

        ScoreGridSize = new Vector2(10, 5);
        ScoreGridPosition = new Vector2(GridPosition.X + GridSize.X + 2, GridPosition.Y + 5);

        FoodBarGridSize = ScoreGridSize;
        FoodBarGridPosition = new Vector2(ScoreGridPosition.X, ScoreGridPosition.Y + ScoreGridSize.Y + 2);

        defaultCursorPosition = new Vector2(0, 0);

        scenes = new Stack<Scene>();

        Randomizer = new Random();
    }


    static GameApp() => Instance = new GameApp();


    public void Run()
    {
        DrawGrid();

        // Creates a default scene
        scenes.Push(new NewGameScene());

        // Game loop
        while (true)
        {
            // Check if a new scene is being requested
            
            scenes.Peek().GetInput();
            scenes.Peek().Update();
        }
    }

    public void DrawGrid()
    {
        int i;

        // Draw the main grid
        Print(' ' + new string('_', GridSize.X), GridPosition.X, GridPosition.Y);

        for (i = 1; i <= GridSize.Y; ++i)
            Print('|' + new string(' ', GridSize.X) + '|', GridPosition.X, GridPosition.Y + i);

        Print(' ' + new string('"', GridSize.X), GridPosition.X, GridPosition.Y + i);


        DrawSideGrid("score", ScoreGridSize, ScoreGridPosition);
        DrawSideGrid("food bar", FoodBarGridSize, FoodBarGridPosition);
    }

    public void DrawSideGrid(string name, Vector2 size, Vector2 position, ConsoleColor color = ConsoleColor.DarkCyan)
    {
        int i = 0;
         // Draw the score grid
        Print(new string('_', size.X), position.X, position.Y, color);

        for (i = 1; i <= ScoreGridSize.Y; ++i)
            Print(new string(' ', size.X) + '|', position.X, position.Y + i, color);

        Print(name, position.X + size.X/2 - name.Length/2 , position.Y + 1, ConsoleColor.Green);
        Print(new string('"', size.X), position.X, position.Y + 2, color);
        Print(new string('"', size.X), position.X, position.Y + i, color);

    }


    // Print to a particular position
    public void Print(object obj, int x, int y)
    {
        Console.SetCursorPosition(x, y);
        Console.Write(obj);
        Console.SetCursorPosition(defaultCursorPosition.X, defaultCursorPosition.Y);
    }

    // Print to a particular position with a particular color
    public void Print(object obj, int x, int y, ConsoleColor textColor)
    {
        var fgc = Console.ForegroundColor;

        Console.SetCursorPosition(x, y);
        Console.ForegroundColor = textColor;
        Console.Write(obj);
        Console.SetCursorPosition(defaultCursorPosition.X, defaultCursorPosition.Y);

        Console.ForegroundColor = fgc;
    }
}