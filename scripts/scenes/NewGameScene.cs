public class NewGameScene : Scene
{
    Snake snake;

    // Fields relating to the food
    const char foodNormal = 'o';
    ConsoleColor foodNormalColor = ConsoleColor.Cyan;

    const char foodBig = '@';
    ConsoleColor foodBigColor = ConsoleColor.Red;

    int foodEatCount = 0;

    int bigFoodTimerLimit = 5000;
    int bigFoodTimerCountdown; // milliseconds
    int bigFoodPoints = 20;

    bool foodIsBig = false;

    Vector2 foodPosition = new(0, 0);

    uint score = 0;


    public NewGameScene() {}


    public override void Init()
    {
        snake = new Snake();

        // Where the snake is not allowed to go beyond or at the point where it spawns at the opposite direction
        snake.VerticalBounds =  new Vector2(0, Console.WindowHeight - 3);
        snake.HorizontalBounds = new Vector2(0, Console.WindowWidth - 1);

        ResetFoodBar();
        GenerateFood();
        UpdateScore();
    }


    // This scene can't wait for user inputs, as the snake has to move nontheless, hence this override 
    public override void GetInput()
    {
        if (Console.KeyAvailable)
            GameApp.Instance.Input = Console.ReadKey();
    }

    // Input processing, Score and other stuff
    public override void Update()
    {
        ProcessInput();
        snake.Move();

        // Check if the snake eats the food
        if (snake.Head.Equals(foodPosition))
        {
            // Snake has eaten a food. Only incrementing for normal food eaten
            if (!foodIsBig)
            {
                 ++foodEatCount;
                 ++score;
                 IncreaseFoodBar();
            } 
            else
            {
                score += Convert.ToUInt32(bigFoodPoints * bigFoodTimerCountdown/bigFoodTimerLimit);
                ResetFoodBar();
            }

            snake.Grow();
            UpdateScore();
            GenerateFood();
        }

        
        if (foodIsBig) CountdownFoodBarTimer();

        // End the game if the snake bits itsel. Else, continue
        if (snake.BiteSelf()) 
            GameOver();
        else 
            Thread.Sleep(GameApp.Instance.DeltaTime);
    }


    protected override void ProcessInput()
    {
        ConsoleKeyInfo input = GameApp.Instance.Input;

        // changing the Snakes's overall direction

        // Move the snake left
        if (input.Key == ConsoleKey.LeftArrow)
        {
            if (snake._Path == Snake.Path.Vertical)
            {
                snake._Path = Snake.Path.Horizontal;
                snake._Direction = Snake.Direction.Negative;
            }
        }

        // Move the snake right
        else if (input.Key == ConsoleKey.RightArrow)
        {
            if (snake._Path == Snake.Path.Vertical)
            {
                snake._Path = Snake.Path.Horizontal;
                snake._Direction = Snake.Direction.Positive;
            }
        }

        // Move the snake up
        else if (input.Key == ConsoleKey.UpArrow)
        {
            if (snake._Path == Snake.Path.Horizontal)
            {
                snake._Path = Snake.Path.Vertical;
                snake._Direction = Snake.Direction.Negative;

            }
        }

        // Move the snake down
        else if (input.Key == ConsoleKey.DownArrow)
        {
            if (snake._Path == Snake.Path.Horizontal)
            {
                snake._Path = Snake.Path.Vertical;
                snake._Direction = Snake.Direction.Positive;
            }
        }
    }
    

    private void GenerateFood()
    {
        var random = GameApp.Instance.Randomizer;
        foodIsBig = !foodIsBig? foodEatCount > 0 && (foodEatCount % 5 == 0) : false;

        // Keep generating a coordinate until one is found which doesn't collide with the snake
        do
        {
            foodPosition = new Vector2(0, 0);
            foodPosition.X = (random.Next() % Console.WindowWidth); // Range is between 0 and Console.WindowWidth - 1;
           
            if (foodPosition.X % 2 != 0) foodPosition.X = foodPosition.X + 1;

            foodPosition.Y = (random.Next() % (Console.WindowHeight - 2));    // -2, So that the range will be between 0 and Console.WindowHeight - 3
        } 
        while (snake.BodyCoordinates.Contains(foodPosition));
        
        // Display new food
        GameApp.Instance.Display(!foodIsBig? foodNormal : foodBig, foodPosition.X, foodPosition.Y, !foodIsBig? foodNormalColor : foodBigColor);
    }


    void UpdateScore()
    {

    }

    
    void IncreaseFoodBar()
    {
        // if (foodEatCount % 5 != 0)
        //     GameApp.Instance.Display("__", GameApp.Instance.FoodBarGridPosition.X + 2*(foodEatCount % 5) - 1 , GameApp.Instance.FoodBarGridPosition.Y + 3, ConsoleColor.Red);
    }


    void CountdownFoodBarTimer()
    {
        // // Display the elapsed time
        // GameApp.Instance.Display($" {(bigFoodTimerCountdown/1000.0):0.00}sec ", GameApp.Instance.FoodBarGridPosition.X + 1, GameApp.Instance.FoodBarGridPosition.Y + 3, bigFoodTimerCountdown >= 2000? ConsoleColor.Green : ConsoleColor.Red);
        // if (bigFoodTimerCountdown <= 0)
        // {
        //     bigFoodTimerCountdown = bigFoodTimerLimit;

        //     // Erase the big food and generate a new, normal food
        //     GameApp.Instance.Display(" ",foodPosition.X, foodPosition.Y);
        //     GenerateFood();
        //     ResetFoodBar();
        // }

        // // Add elapsed time after, so it is reflected in the next check
        // bigFoodTimerCountdown -= GameApp.Instance.DeltaTime;
    }

    void ResetFoodBar()
    {
        // bigFoodTimerCountdown = 5000;
        // GameApp.Instance.Display("________", GameApp.Instance.FoodBarGridPosition.X + 1, GameApp.Instance.FoodBarGridPosition.Y + 3, ConsoleColor.DarkGray);
    }


    void GameOver()
    {
        var gameData = Database._GameData;

        Thread.Sleep(1000);

        GameApp.Instance.ClearWindow();
        string gameover = " G A M E   O V E R! ";
        GameApp.Instance.Display(gameover, GameApp.Instance.CenterHorizontally(gameover), GameApp.Instance.CenterVertically(gameover), ConsoleColor.Green);
        
        if (score > gameData.HighScore) gameData.HighScore = score;
        
        // saves the current state of the game data to the database
        Database.SaveGameData();
        snake.Reset();

        Thread.Sleep(2000);

        ResetFoodBar();
        score = 0;
        UpdateScore();

        GameApp.Instance.ExitCurrentScene = true;
    }
}