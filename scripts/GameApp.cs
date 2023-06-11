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

    private Stack<Scene> scenes;    // currently running scenes 
    private Dictionary<SceneType, Scene> scenesBuffer;  // All the scenes of the game, preloaded (The actual scene objects)
    
    private bool exitCurrentScene;
    public bool ExitCurrentScene 
    { 
        get { return exitCurrentScene; }
        set
        {
            // If it is requested to exit a scene, we must make sure there is at least one scene running (i.e the main scene)
            exitCurrentScene = (scenes.Count > 1)? value : false;
        }
    }

    public SceneType RequestedSceneType { get; set; }
    public SceneType CurrentSceneType { get; private set; }


    public Random Randomizer {get; private set; }
    public int DeltaTime { get; private set; }


    private GameApp()
    {
        GridSize = new Vector2(55, 27);

        Vector2 position = new Vector2(Console.WindowWidth / 2 - GridSize.X / 2, Console.WindowHeight / 2 - GridSize.Y / 2);
        
        // Documentation
        GridPosition = new Vector2(position.X % 2 == 0? position.X : position.X + 1, position.Y);

        ScoreGridSize = new Vector2(10, 5);
        ScoreGridPosition = new Vector2(GridSize.X + 2, 5);

        FoodBarGridSize = ScoreGridSize;
        FoodBarGridPosition = new Vector2(ScoreGridPosition.X, ScoreGridPosition.Y + ScoreGridSize.Y + 2);

        defaultCursorPosition = new Vector2(0, 0);

        scenes = new Stack<Scene>();

        scenesBuffer = new Dictionary<SceneType, Scene>();

        // Sort of pre-loading the scenes
        scenesBuffer.Add(SceneType.MainScene, new MainScene());
        scenesBuffer.Add(SceneType.NewGameScene, new NewGameScene());
        scenesBuffer.Add(SceneType.OptionsScene, new OptionsScene());

        ExitCurrentScene = false;

        Randomizer = new Random();
        
        DeltaTime = Convert.ToInt32(Database._GameData.Level);
    }


    static GameApp() => Instance = new GameApp();


    public void Run()
    {
        DrawGrid();

        scenes.Push(scenesBuffer[SceneType.MainScene]);
        scenes.Peek().Init();

        // Game loop
        while (true)
        {
            if (exitCurrentScene)
                LoadPrevScene();

            else if (RequestedSceneType != CurrentSceneType)
                LoadNewScene();

            scenes.Peek().GetInput();
            scenes.Peek().Update();
        }
    }


    public void LoadPrevScene()
    {
        scenes.Pop();
        ClearGrid();

        // So the new scene is ready and shown
        scenes.Peek().Init();

        exitCurrentScene = false;
        // Whether to update current scene and requested scene (but they are already the same, just not referring to the actual scene type)
    }


    public void LoadNewScene()
    {
        scenes.Push(scenesBuffer[RequestedSceneType]);
        ClearGrid();

        // So the new scene is ready and shown
        scenes.Peek().Init();

        CurrentSceneType = RequestedSceneType;
    }


    public void DrawGrid()
    {
        int i;

        // Draw the main grid
        Display(' ' + new string('_', GridSize.X), 0, 0);

        for (i = 1; i <= GridSize.Y; ++i)
            Display('|' + new string(' ', GridSize.X) + '|', 0, i);

        Display(' ' + new string('"', GridSize.X), 0, i);


        DrawSideGrid("score", ScoreGridSize, ScoreGridPosition);
        DrawSideGrid("food bar", FoodBarGridSize, FoodBarGridPosition);
    }

    public void DrawSideGrid(string name, Vector2 size, Vector2 position, ConsoleColor color = ConsoleColor.DarkCyan)
    {
        int i = 0;
         // Draw the score grid
        Display(new string('_', size.X), position.X, position.Y, color);

        for (i = 1; i <= ScoreGridSize.Y; ++i)
            Display(new string(' ', size.X) + '|', position.X, position.Y + i, color);

        Display(name, position.X + size.X/2 - name.Length/2 , position.Y + 1, ConsoleColor.Green);
        Display(new string('"', size.X), position.X, position.Y + 2, color);
        Display(new string('"', size.X), position.X, position.Y + i, color);

    }


    // Display to a particular position within the grid
    public void Display(object obj, int x, int y)
    {
        Console.SetCursorPosition(GridPosition.X + x, GridPosition.Y + y);
        Console.Write(obj);
        Console.SetCursorPosition(defaultCursorPosition.X, defaultCursorPosition.Y);
    }

    // Display to a particular position  within the grid, with a particular color
    public void Display(object obj, int x, int y, ConsoleColor textColor)
    {
        var fgc = Console.ForegroundColor;

        Console.SetCursorPosition(GridPosition.X  + x, GridPosition.Y + y);
        Console.ForegroundColor = textColor;
        Console.Write(obj);
        Console.SetCursorPosition(defaultCursorPosition.X, defaultCursorPosition.Y);

        Console.ForegroundColor = fgc;
    }
    

    public void ClearGrid()
    {
        for (int i = 1; i <= GridSize.Y; ++i)
            Display(new string(' ', GridSize.X),1, i);
    }


    // Returns the X coordinate to start Displaying the content string from, at which it will be horizontally centered
    public int CenterHorizontally(string content)
    {
        return GameApp.Instance.GridSize.X/2 - content.Length/2 + 1;
    }

    // Returns the Y coordinate to Display the content string, at which it will be vertically centered
    public int CenterVertically(string content)
    {
        return GameApp.Instance.GridSize.Y/2;
    }

    public Vector2 Center(string content)
    {
        return new Vector2(CenterHorizontally(content), CenterVertically(content));
    }
}