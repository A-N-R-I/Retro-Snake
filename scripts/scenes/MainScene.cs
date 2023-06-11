class MainScene : Scene
{

    KeyValuePair<SceneType, string>[] menuOptions;
    // string[] playOptions = {"New Game", "Continue"};
    ConsoleColor menuOptionsColor = ConsoleColor.Cyan;


    // To manage highlighting options
    MenuOptionsSelector selector;


    public MainScene() 
    {
        menuOptions = new KeyValuePair<SceneType, string>[5];

        // The keys are the type of Scene which they lead to
        menuOptions[0] = new (SceneType.NewGameScene, "Play");
        menuOptions[1] = new (SceneType.ScoreScene, "Score");
        menuOptions[2] = new (SceneType.OptionsScene, "Options");
        menuOptions[3] = new (SceneType.HelpScene, "Help");
        menuOptions[4] = new (SceneType.AboutScene, "About");

        selector = new MenuOptionsSelector(menuOptions, menuOptionsColor, Console.BackgroundColor);
    }


    public override void Init()
    {
        DisplayMenuOptions();
    }


    public override void Update()
    {
        ProcessInput();
        // Sleep() not needed because the console waits for input in each frame
    }


    protected override void ProcessInput()
    {
        ConsoleKeyInfo input = GameApp.Instance.Input;

        // Move the selector up and down
        if (input.Key == ConsoleKey.DownArrow)
            selector.HighlightNextOption();
        else if (input.Key == ConsoleKey.UpArrow)
            selector.HighlightPrevOption();

        // Player selects the currrent option, tell the game app that a new scene is needed at the next frame update
        else if (input.Key == ConsoleKey.Spacebar)
        {
            // The bug is that RequestedSceneType and CurrentSceneType are the same after the game is over and the 
            // ui goes back to menu. As a result trying to load the NewGameScene using the method below, won't work.

            GameApp.Instance.RequestedSceneType = selector.CurrentSelectedOption.Key;
        }

    }


    private void DisplayMenuOptions()
    {
        var text = " R E T R O   S N A K E ";
        GameApp.Instance.Display(text, GameApp.Instance.CenterHorizontally(text), 3, ConsoleColor.Green);

        foreach (var pair in menuOptions)
        {
            GameApp.Instance.Display(pair.Value, GameApp.Instance.CenterHorizontally(pair.Value),(int)pair.Key, menuOptionsColor);
        }

        text = "Press space to select";
        GameApp.Instance.Display(text, GameApp.Instance.CenterHorizontally(text), (int)menuOptions.Last().Key + 4, ConsoleColor.DarkGray);

        // Highlight a default option
        selector.HighlightCurrentSelectedOption();
    }
}