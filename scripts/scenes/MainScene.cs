using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

class MainScene : Scene
{

    KeyValuePair<SceneType, string>[] menuOptions;
    // string[] playOptions = {"New Game", "Continue"};
    ConsoleColor menuOptionsColor = ConsoleColor.Cyan;


    // To manage highlighting options
    MenuOptionsSelector selector;


    string retro = @"
               ____ ____ ___ ____ ____
               |--< |===  |  |--< [__]";
    string snake = @" 
                        ____ __ _ ____ _  _ ____
                        ==== | \| |--| |-:_ |===";


    public MainScene()
    {
        _SceneType = SceneType.MainScene;

        menuOptions = new KeyValuePair<SceneType, string>[6];

        // The keys are the type of Scene which they lead to
        menuOptions[0] = new(SceneType.NewGameScene, "Play");
        menuOptions[1] = new(SceneType.ScoreScene, "Score");
        menuOptions[2] = new(SceneType.OptionsScene, "Options");
        menuOptions[3] = new(SceneType.HelpScene, "Help");
        menuOptions[4] = new(SceneType.AboutScene, "About");
        menuOptions[5] = new(SceneType.Quit, "Quit");

        selector = new MenuOptionsSelector(menuOptions, menuOptionsColor, Console.BackgroundColor);
    }


    public override void Init()
    {
        DisplayUI();
    }


    protected override void ProcessInput()
    {
        base.ProcessInput();

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

            if (selector.CurrentSelectedOption.Key != SceneType.Quit)
                GameApp.Instance.RequestedSceneType = selector.CurrentSelectedOption.Key;
            else
                GameApp.Instance.QuitApplication = true;
        }

    }


    public override void Update()
    {
        ProcessInput();
        // Sleep() not needed because the console waits for input in each frame
    }


    void DisplayUI()
    {
        GameApp.Instance.Display(retro, GameApp.Instance.CenterHorizontally("rrrr eeee tttt rrrr oooo") + 5, 1, ConsoleColor.Cyan);
        GameApp.Instance.Display(snake, GameApp.Instance.CenterHorizontally("ssss nnnn aaaa kkkk eeee"), 4, ConsoleColor.Red);

        foreach (var pair in menuOptions)
        {
            GameApp.Instance.Display(pair.Value, GameApp.Instance.CenterHorizontally(pair.Value), (int)pair.Key, menuOptionsColor);
        }

        var text = "Press space to select";
        GameApp.Instance.Display(text, GameApp.Instance.CenterHorizontally(text), (int)menuOptions.Last().Key + 5, ConsoleColor.Red);

        // Highlight a default option
        selector.HighlightCurrentSelectedOption();
    }
}