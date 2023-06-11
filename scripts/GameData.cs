// Handles 

// This will be used to simulate the speed of the snake via sleeping the main thread
public enum GameLevel
{
    Easy = 250,
    Normal = 100,
    Hard = 50
}

class GameData
{
    public uint HighScore { get; set; }
    public GameLevel Level { get; private set; }


    public GameData()
    {
        Level = GameLevel.Normal;
    }
}