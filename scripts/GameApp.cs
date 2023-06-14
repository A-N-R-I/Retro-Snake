// Singleton
public class GameApp
{
    public static GameApp Instance { get; private set; }

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

    // Helps in switching between scenes
    public SceneType RequestedSceneType { get; set; }
    public SceneType CurrentSceneType { get; private set; }

    public bool QuitApplication { get; set; }


    public Random Randomizer {get; private set; }
    public int DeltaTime { get; private set; }


    private GameApp()
    {
        defaultCursorPosition = new Vector2(0, 0);

        scenes = new Stack<Scene>();
        scenesBuffer = new Dictionary<SceneType, Scene>();

        // Sort of pre-loading the scenes
        scenesBuffer.Add(SceneType.MainScene, new MainScene());
        scenesBuffer.Add(SceneType.NewGameScene, new NewGameScene());
        scenesBuffer.Add(SceneType.ScoreScene, new ScoreScene());
        scenesBuffer.Add(SceneType.OptionsScene, new OptionsScene());
        scenesBuffer.Add(SceneType.HelpScene, new HelpScene());
        scenesBuffer.Add(SceneType.AboutScene, new AboutScene());

        ExitCurrentScene = QuitApplication = false;

        Randomizer = new Random();
        
        DeltaTime = Convert.ToInt32(Database._GameData.Level);
    }


    static GameApp() => Instance = new GameApp();


    public void Run()
    {
        scenes.Push(scenesBuffer[SceneType.MainScene]);
        scenes.Peek().Init();

        // Game loop
        while (!QuitApplication)
        {
            if (exitCurrentScene)
                LoadPrevScene();

            else if (RequestedSceneType != CurrentSceneType)
                LoadRequestedScene();

            // Processing for the current running scene
            scenes.Peek().GetInput();
            scenes.Peek().Update();
        }
    }


    public void LoadPrevScene()
    {
        scenes.Pop();
        ClearWindow();

        // So the new scene is ready and shown
        scenes.Peek().Init();

        exitCurrentScene = false;
        CurrentSceneType = RequestedSceneType = scenes.Peek()._SceneType;
    }


    public void LoadRequestedScene()
    {
        scenes.Push(scenesBuffer[RequestedSceneType]);
        ClearWindow();

        // So the new scene is ready and shown
        scenes.Peek().Init();

        CurrentSceneType = RequestedSceneType;
    }

    // Display to a particular position within the grid
    public void Display(object obj, int x, int y)
    {
        Console.SetCursorPosition(x, y);
        Console.Write(obj);
        Console.SetCursorPosition(defaultCursorPosition.X, defaultCursorPosition.Y);
    }

    // Display to a particular position with a particular color
    public void Display(object obj, int x, int y, ConsoleColor textColor)
    {
        var fgc = Console.ForegroundColor;

        Console.SetCursorPosition(x, y);
        Console.ForegroundColor = textColor;
        Console.Write(obj);
        Console.SetCursorPosition(defaultCursorPosition.X, defaultCursorPosition.Y+1);

        Console.ForegroundColor = fgc;
    }
    

    public void ClearWindow()
    {
        for (int i = 0; i < Console.WindowHeight; ++i)
            Display(new string(' ', Console.WindowWidth), 0, i);
    }


    // Returns the X coordinate to start Displaying the content string from, at which it will be horizontally centered
    public int CenterHorizontally(string content)
    {
        return Console.WindowWidth/2 - content.Length/2 + 1;
    }

    // Returns the Y coordinate to Display the content string, at which it will be vertically centered
    public int CenterVertically(string content)
    {
        return Console.WindowHeight/2;
    }
}