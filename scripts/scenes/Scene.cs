using System;


public abstract class Scene
{
    public SceneType _SceneType { get; protected set; }

    // Manually called by GameApp when the scene is to run
    public abstract void Init();


    // By default scenes wait for user input
    public virtual void GetInput()
    {
        GameApp.Instance.Input = Console.ReadKey();
    }


    protected virtual void ProcessInput()
    {
        // Pressing the backspace key on any scene will automatically exit the current scene for the previous
        if (GameApp.Instance.Input.Key == ConsoleKey.Backspace)
            GameApp.Instance.ExitCurrentScene = true;
    }


    public abstract void Update();
}