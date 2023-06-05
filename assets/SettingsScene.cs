public class SettingsScene : Scene
{

    // This will be used to simulate the speed of the snake via sleeping the main thread
    public enum GameLevel
    {
        Easy = 250,
        Normal = 100,
        Hard = 50
    }

    public static GameLevel Level { get; private set; }


    static SettingsScene()
    {
        Level = GameLevel.Normal;
    }


    public override void GetInput()
    {

    }

    public override void Update()
    {

    }
}