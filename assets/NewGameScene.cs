public class NewGameScene : Scene
{
    Snake snake;
    

    // Fields relating to the food
    const char foodNormal = 'o';
    const char foodBig = 'O';

    int foodEatCount = 0;

    ConsoleColor foodColor = ConsoleColor.Cyan;

    Vector2 foodPosition = new(0, 0);

    // Game data
    uint score = 0;
    bool foodIsBig = false;


    public NewGameScene()
    {
        snake = new Snake();

        // Where the snake is not allowed to go beyond or at the point where it spawns at the opposite direction
        snake.VerticalBounds =  new Vector2(GameApp.Instance.GridPosition.Y+1, GameApp.Instance.GridPosition.Y + GameApp.Instance.GridSize.Y);
        snake.HorizontalBounds = new Vector2(GameApp.Instance.GridPosition.X+1, GameApp.Instance.GridPosition.X + GameApp.Instance.GridSize.X);

        GenerateFood();
        UpdateScore();
        ResetFoodBar();
    }


    public override void GetInput()
    {
        if (Console.KeyAvailable)
            GameApp.Instance.Input = Console.ReadKey();
    }

    // Input processing, Score and other stuff
    public override void Update()
    {
        ConsoleKeyInfo input = GameApp.Instance.Input;

        // changing the Snakes's overall direction

        // Move the snake left
        if (input.Key == ConsoleKey.LeftArrow)
        {
            if (snake._Trajectory == Snake.Trajectory.Vertical)
            {
                snake._Trajectory = Snake.Trajectory.Horizontal;
                snake._Direction = Snake.Direction.Negative;
            }
        }

        // Move the snake right
        else if (input.Key == ConsoleKey.RightArrow)
        {
            if (snake._Trajectory == Snake.Trajectory.Vertical)
            {
                snake._Trajectory = Snake.Trajectory.Horizontal;
                snake._Direction = Snake.Direction.Positive;
            }
        }

        // Move the snake up
        else if (input.Key == ConsoleKey.UpArrow)
        {
            if (snake._Trajectory == Snake.Trajectory.Horizontal)
            {
                snake._Trajectory = Snake.Trajectory.Vertical;
                snake._Direction = Snake.Direction.Negative;
            }
        }

        // Move the snake down
        else if (input.Key == ConsoleKey.DownArrow)
        {
            if (snake._Trajectory == Snake.Trajectory.Horizontal)
            {
                snake._Trajectory = Snake.Trajectory.Vertical;
                snake._Direction = Snake.Direction.Positive;
            }
        }
        
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
                score += 10;
                ResetFoodBar();
            }

           
            snake.Grow();
            UpdateScore();
            GenerateFood();
        }

        
        if (snake.BiteSelf())
        {
            GameOver();
        }

        // Update the time bar that shows when the big food will disappear
        if (foodIsBig) DecreaseFoodBar();


        Thread.Sleep((int)SettingsScene.Level);
    }

    private void GenerateFood()
    {
        var random = GameApp.Instance.Randomizer;
        foodIsBig = !foodIsBig? foodEatCount > 0 && (foodEatCount % 5 == 0) : false;

        // Keep generating a coordinate until one is found which doesn't collide with the snake
        do
        {
            foodPosition = new Vector2(GameApp.Instance.GridPosition.X + GameApp.Instance.GridSize.X, GameApp.Instance.GridPosition.Y + 10);
            foodPosition.X = GameApp.Instance.GridPosition.X + (random.Next() % GameApp.Instance.GridSize.X) + 1;
           
            if (foodPosition.X % 2 == 0) foodPosition.X = foodPosition.X + 1;

            foodPosition.Y = GameApp.Instance.GridPosition.Y + (random.Next() % GameApp.Instance.GridSize.Y) + 1;
        } 
        while (snake.BodyCoordinates.Contains(foodPosition));

        // Print new food
        GameApp.Instance.Print(!foodIsBig? foodNormal : foodBig, foodPosition.X, foodPosition.Y, !foodIsBig? foodColor : ConsoleColor.Red);
    }


    void UpdateScore()
    {
        GameApp.Instance.Print(score, GameApp.Instance.ScoreGridPosition.X + GameApp.Instance.ScoreGridSize.X/2 - $"{score}".Length/2, GameApp.Instance.ScoreGridPosition.Y + 3, ConsoleColor.Green);
    }

    
    void IncreaseFoodBar()
    {
        if (foodEatCount % 5 != 0)
            GameApp.Instance.Print("__", GameApp.Instance.FoodBarGridPosition.X + 2*(foodEatCount % 5) - 1 , GameApp.Instance.FoodBarGridPosition.Y + 3, ConsoleColor.Red);
    }

    void DecreaseFoodBar()
    {
        
    }

    void ResetFoodBar()
    {
        GameApp.Instance.Print("________", GameApp.Instance.FoodBarGridPosition.X + 1, GameApp.Instance.FoodBarGridPosition.Y + 3, ConsoleColor.DarkGray);
    }


    void GameOver()
    {
        // Show scores then write them to a file
        // Reset the snake
        // Go to main Screen
        Console.Read();
    }
}