public class Snake
{
    // The basic build-up of the snake's body
    const char body = 'o';

    
    const ConsoleColor bodyColor = ConsoleColor.Yellow;

    // Vertical or horizontal path
    public enum Trajectory
    {
        Horizontal = 0,
        Vertical = 1
    }

    // Positive or negative direction towards the chosen trajectory
    public enum Direction
    {
        Positive = 1,
        Negative = -1
    }


    public Trajectory _Trajectory;
    public Direction _Direction;

    public Vector2 VerticalBounds { get; set; }
    public Vector2 HorizontalBounds { get; set; }

    // Where each body part of the snake is located on the grid
    public LinkedList<Vector2> BodyCoordinates {get; private set; }

    public Vector2 Head
    {
        get
        {
            return BodyCoordinates.Last();
        }
    }

    public Vector2 Tail
    {
        get
        {
            return BodyCoordinates.First();
        }
    }


    public Snake()
    {
        BodyCoordinates = new LinkedList<Vector2>();
        // Initinial positioning and length
        Init();
    }

    private void Init()
    {
        var random = GameApp.Instance.Randomizer;

        // Determine how the snake initailly appears at in the grid
        _Trajectory = (Trajectory)(random.Next() % 2);

        int x;
        int y;
        
        _Trajectory = Trajectory.Vertical;

        // The snake appears vertically
        if (_Trajectory == Trajectory.Vertical)
        {
            x = GameApp.Instance.GridPosition.X + (random.Next() % GameApp.Instance.GridSize.X) + 1;
            // Documentation - 
            if (x % 2 == 0) x = x + 1;

            y = GameApp.Instance.GridPosition.Y;

            BodyCoordinates.AddLast(new Vector2(x, y + 1));
            BodyCoordinates.AddLast(new Vector2(x, y + 2));
            BodyCoordinates.AddLast(new Vector2(x, y + 3));
        }
        // The snake appears horizontally
        else
        {
            x = GameApp.Instance.GridPosition.X;
            y = GameApp.Instance.GridPosition.Y + (random.Next() % GameApp.Instance.GridSize.Y) + 1;

            BodyCoordinates.AddLast(new Vector2(x + 1, y));
            BodyCoordinates.AddLast(new Vector2(x + 3, y));
            BodyCoordinates.AddLast(new Vector2(x + 5, y));
        }

        // Snake will always start at a positive direction towards a chosen trajectory
        _Direction = Direction.Positive;

        // Display the snake
        foreach (var coord in BodyCoordinates)
            GameApp.Instance.Print(body, coord.X, coord.Y, bodyColor);
    }


    public void Move(bool eraseTail = true)
    {
        Vector2 tail = new(this.Tail);
        Vector2 head = new(this.Head);

        // Remove the old tail (By default it is removed)
        if (eraseTail) 
        {
            GameApp.Instance.Print(" ", tail.X, tail.Y);
            BodyCoordinates.RemoveFirst();
        }

        if (!CollidesWithBounds())
        {
            if (_Trajectory == Trajectory.Vertical)
                head.Y += (int)_Direction;
            else
                head.X += (int)_Direction * 2;
        }
        else
        {
            if (_Trajectory == Trajectory.Vertical)
                head.Y = (_Direction == Direction.Positive)? VerticalBounds.X : VerticalBounds.Y;
            else
                head.X = (_Direction == Direction.Positive)? HorizontalBounds.X : HorizontalBounds.Y;
        }

        // Add the new head
        BodyCoordinates.AddLast(head);
        GameApp.Instance.Print(body, head.X, head.Y, bodyColor);
        Console.WriteLine(BodyCoordinates.Count);
    }


    // Basically adds a new tail behind the old tail
    public void Grow()
    {
        Vector2 preTail = BodyCoordinates.ElementAt(1);
        Vector2 newTail = new(Tail.X, Tail.Y);

        // Handle positioning for normal conditions
        int sign = preTail.X > Tail.X? -1 : (preTail.X < Tail.X? 1 : 0);
        newTail.X += 2*sign;

        sign = preTail.Y > Tail.Y? -1 : (preTail.Y < Tail.Y? 1 : 0);
        newTail.Y += sign;

        // Now check if this isn't a normal condition so as to negate the sign 
        if ((Math.Abs(preTail.X - Tail.X) > 2) || (Math.Abs(preTail.Y - Tail.Y) > 1))
            sign = -sign;

        // If the new tail will span over the bounds. Note that only the x or y (not both at the same time) might span over the bounds.
        // This is because the new tail will only modify either the x or y position but never both
        if (newTail.X <= HorizontalBounds.X)
            newTail.X = HorizontalBounds.Y - 1;
        else if (newTail.X >= HorizontalBounds.Y)
            newTail.X = HorizontalBounds.X + 1;
        else if (newTail.Y <= VerticalBounds.X)
            newTail.Y = VerticalBounds.Y - 1;
        else if (newTail.Y >= VerticalBounds.Y)
            newTail.Y = VerticalBounds.X + 1;

        BodyCoordinates.AddFirst(newTail);
        GameApp.Instance.Print(body, newTail.X, newTail.Y, bodyColor);
    }


    public bool BiteSelf()
    {
        int index = 0;

        // If the head's coordinate is found somewhere else other than the last position in the queue (Last because the head is the most recent element)
        foreach (var coord in BodyCoordinates)
            if (index++ < BodyCoordinates.Count-1 && coord.Equals(Head))
                return true;

        return false;
    }


    private bool CollidesWithBounds()
    {
        return (_Trajectory == Trajectory.Vertical)? 
            _Direction == Direction.Negative && Head.Y == VerticalBounds.X || (_Direction == Direction.Positive && Head.Y == VerticalBounds.Y) : 
            _Direction == Direction.Negative && Head.X == HorizontalBounds.X || (_Direction == Direction.Positive && Head.X == HorizontalBounds.Y);
    }
}