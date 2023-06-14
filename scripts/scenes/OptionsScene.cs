public class OptionsScene : Scene
{
    public OptionsScene() 
    {
        _SceneType = SceneType.OptionsScene;
    }
    

    public override void Init()
    {
        
    }
    

    protected override void ProcessInput()
    {
        base.ProcessInput();
    }


    public override void Update()
    {
        ProcessInput();
    }
}