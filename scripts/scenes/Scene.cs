public abstract class Scene
{
    // Manually called by GameApp when the scene is to run
    public abstract void Init();


    // By default scenes wait for user input
    public virtual void GetInput()
    {
        GameApp.Instance.Input = Console.ReadKey();
    }


    protected abstract void ProcessInput();


    public abstract void Update();
}