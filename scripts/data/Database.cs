// Handles storing and retrieval of game data to database (in this case, the user's storage)

static class Database
{

    public static GameData _GameData { get; }

    static Database()
    {
        _GameData = new GameData();
    }


    public static void SaveGameData()
    {

    }
}