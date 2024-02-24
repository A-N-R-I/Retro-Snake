using System;
using System.Threading;


public class NewGameScene : Scene
{
    Snake snake;


    Vector2 boundsX;
    Vector2 boundsY;

    Vector2 foodPosition;

    // Fields relating to the food
    const char food = 'o';
    const char bigFood = '@';

    string bigFoodBar = "__________";

    ConsoleColor foodColor = ConsoleColor.Cyan;
    ConsoleColor bigFoodColor = ConsoleColor.Yellow;

    int foodEatCount = 0;

    int bigFoodTimerLimit = 5000;
    int bigFoodTimerCountdown; // milliseconds
    int bigFoodPoints = 20;

    bool foodIsBig = false;

    uint score = 0;


    public NewGameScene()
    {
        _SceneType = SceneType.NewGameScene;

        boundsX = new Vector2(0, Console.WindowWidth - 1);
        boundsY = new Vector2(0, Console.WindowHeight - 4);

        snake = new Snake();

        // Where the snake is not allowed to go beyond or at the point where it spawns at the opposite direction
        snake.HorizontalBounds = boundsX;
        snake.VerticalBounds = boundsY;
    }


    public override void Init()
    {
        snake.Init();

        DisplayUI();

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


    protected override void ProcessInput()
    {
        base.ProcessInput();

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


    // Input processing, Score and other stuff
    public override void Update()
    {
        ProcessInput();
        snake.Move();

        // End the game if the snake bits itself
        if (snake._BiteSelf)
            GameOver();
        else
        {
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
                    score += Convert.ToUInt32(bigFoodPoints * bigFoodTimerCountdown / bigFoodTimerLimit);
                    ResetFoodBar();
                }
                snake.Grow();
                UpdateScore();
                GenerateFood();
            }

            if (foodIsBig) DecreaseFoodBar();

            Thread.Sleep(GameApp.Instance.DeltaTime);
        }
    }


    void DisplayUI()
    {
        GameApp.Instance.Display("Score", boundsX.X + 3, boundsY.Y + 1, ConsoleColor.Cyan);
        GameApp.Instance.Display("@", boundsX.X + boundsX.Y - 20, boundsY.Y + 1, ConsoleColor.Yellow);
    }


    void GenerateFood()
    {
        var random = GameApp.Instance.Randomizer;
        foodIsBig = !foodIsBig ? foodEatCount > 0 && (foodEatCount % 5 == 0) : false;

        // Keep generating a coordinate until one is found which doesn't collide with the snake
        do
        {
            foodPosition = new Vector2(0, 0);
            foodPosition.X = (random.Next() % (boundsX.Y - boundsX.X + 1)) + boundsX.X; // Range is between boundsX.X and boundsX.Y

            if (foodPosition.X % 2 != 0) foodPosition.X = foodPosition.X + 1;

            foodPosition.Y = (random.Next() % (boundsY.Y - boundsY.X + 1)) + boundsY.X; // Range is between boundsY.X and boundsY.Y
        }
        while (snake.BodyCoordinates.Contains(foodPosition));

        // Display new food
        GameApp.Instance.Display(!foodIsBig ? food : bigFood, foodPosition.X, foodPosition.Y, !foodIsBig ? foodColor : bigFoodColor);
    }


    void UpdateScore()
    {
        GameApp.Instance.Display($"  {score}", boundsX.X + 8, boundsY.Y + 1, ConsoleColor.Red);
    }


    void IncreaseFoodBar()
    {
        var rem = foodEatCount % 5;
        var count = rem == 0 ? bigFoodBar.Length : 2 * rem;

        GameApp.Instance.Display(new string(bigFoodBar[0], count), boundsX.Y - 18, boundsY.Y + 1, ConsoleColor.Cyan);
    }


    void DecreaseFoodBar()
    {
        // Display the elapsed time
        GameApp.Instance.Display($"{(bigFoodTimerCountdown / 1000.0):0.00}sec    ", boundsX.Y - 18, boundsY.Y + 1, bigFoodTimerCountdown >= 2000 ? ConsoleColor.Cyan : ConsoleColor.Red);
        if (bigFoodTimerCountdown <= 0)
        {
            bigFoodTimerCountdown = bigFoodTimerLimit;

            // Erase the big food and generate a new, normal food
            GameApp.Instance.Display(" ", foodPosition.X, foodPosition.Y);
            GenerateFood();
            ResetFoodBar();
        }

        // Add elapsed time after, so it is reflected in the next check
        bigFoodTimerCountdown -= GameApp.Instance.DeltaTime;
    }

    void ResetFoodBar()
    {
        bigFoodTimerCountdown = 5000;
        GameApp.Instance.Display("__________", boundsX.Y - 18, boundsY.Y + 1, ConsoleColor.DarkGray);
    }


    void GameOver()
    {
        var gameData = Database._GameData;

        Thread.Sleep(1000);

        GameApp.Instance.ClearWindow();


        // https://patorjk.com/software/taag/#p=display&v=1&f=Cybersmall&t=game%20over!

        string game = @"
                    ____ ____ _  _ ____  
                    |__, |--| |\/| |===  ";
        string over = @"
                    ____ _  _ ____ ____ /
                    [__]  \/  |=== |--<. ";


        GameApp.Instance.Display(game, GameApp.Instance.CenterHorizontally("gggg aaaa mmmm eeee"), GameApp.Instance.CenterVertically(game) - 5, ConsoleColor.Cyan);
        GameApp.Instance.Display(over, GameApp.Instance.CenterHorizontally("oooo vvvv eeee rrrr"), GameApp.Instance.CenterVertically(over) - 2, ConsoleColor.Red);

        if (score > gameData.HighScore) gameData.HighScore = score;

        // saves the current state of the game data to the database
        Database.SaveGameData();
        snake.Reset();

        Thread.Sleep(2000);

        // Reset gameplay data
        ResetFoodBar();
        foodEatCount = 0;
        score = 0;

        GameApp.Instance.ExitCurrentScene = true;
    }
}