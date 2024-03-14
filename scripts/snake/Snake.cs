using System;
using System.Collections.Generic;


public class Snake
{
    // The basic build-up of the snake's body
    const char body = 'o';


    const ConsoleColor bodyColor = ConsoleColor.Red;
    public bool _BiteSelf { get; private set; }

    // Vertical or horizontal path
    public enum Path
    {
        Horizontal = 0,
        Vertical = 1
    }

    // Positive or negative direction towards the chosen Path
    public enum Direction
    {
        Positive = 1,
        Negative = -1
    }


    public Path _Path { get; set; }
    public Direction _Direction { get; set; }

    public Vector2 VerticalBounds { get; set; }
    public Vector2 HorizontalBounds { get; set; }

    // Where each body part of the snake is located on the grid
    public LinkedList<Vector2> BodyCoordinates { get; private set; }
    public Vector2 Head
    {
        get { return BodyCoordinates.Last.Value; }
    }
    public Vector2 Tail
    {
        get { return BodyCoordinates.First.Value; }
    }


    public Snake()
    {
        BodyCoordinates = new LinkedList<Vector2>();
    }


    public void Init()
    {
        var random = GameApp.Instance.Randomizer;

        // Determine how the snake initailly appears at in the grid
        _Path = (Path)(random.Next() % 2);
        _Direction = Direction.Positive;

        int x = 0;
        int y = 0;

        // The snake appears vertically
        if (_Path == Path.Vertical)
        {
            x = (random.Next() % (HorizontalBounds.Y - HorizontalBounds.X + 1)) + HorizontalBounds.X; // Range is between HorizontalBounds.X and HorizontalBounds.Y); 

            if (x % 2 != 0) x = x + 1;

            BodyCoordinates.AddLast(new Vector2(x, y + 1));
            BodyCoordinates.AddLast(new Vector2(x, y + 2));
            BodyCoordinates.AddLast(new Vector2(x, y + 3));
        }
        // The snake appears horizontally
        else
        {
            y = (random.Next() % (VerticalBounds.Y - VerticalBounds.X + 1)) + VerticalBounds.X; // Range is between VerticalBounds.X and VerticalBounds.Y;

            BodyCoordinates.AddLast(new Vector2(x + 0, y));
            BodyCoordinates.AddLast(new Vector2(x + 2, y));
            BodyCoordinates.AddLast(new Vector2(x + 4, y));
        }


        // Display the snake
        foreach (var coord in BodyCoordinates)
            GameApp.Instance.Display(body, coord.X, coord.Y, bodyColor);
    }


    public void Move(bool eraseTail = true)
    {
        Vector2 tail = new(this.Tail);
        Vector2 head = new(this.Head);

        if (!CollidesWithBounds())
        {
            if (_Path == Path.Vertical)
                head.Y += (int)_Direction;
            else
                head.X += (int)_Direction * 2;
        }
        else
        {
            if (_Path == Path.Vertical)
                head.Y = (_Direction == Direction.Positive) ? VerticalBounds.X : VerticalBounds.Y;
            else
                head.X = (_Direction == Direction.Positive) ? HorizontalBounds.X : HorizontalBounds.Y;
        }

        // Add the new head
        BodyCoordinates.AddLast(head);

        // A new head will only be shown (and old tail removed) if the snake does not bite itself
        if (!(_BiteSelf = BiteSelf()))
        {
            // Remove the old tail (By default it is removed)
            if (eraseTail)
            {
                GameApp.Instance.Display(" ", tail.X, tail.Y);
                BodyCoordinates.RemoveFirst();
            }
            GameApp.Instance.Display(body, head.X, head.Y, bodyColor);
        }
    }


    // Basically adds a new tail behind the old tail
    public void Grow()
    {
        Vector2 preTail = BodyCoordinates.First.Next.Value;
        Vector2 newTail = new(Tail.X, Tail.Y);

        // Handle positioning for normal conditions
        int sign = preTail.X > Tail.X ? -1 : (preTail.X < Tail.X ? 1 : 0);
        newTail.X += 2 * sign;

        sign = preTail.Y > Tail.Y ? -1 : (preTail.Y < Tail.Y ? 1 : 0);
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
        GameApp.Instance.Display(body, newTail.X, newTail.Y, bodyColor);
    }


    bool BiteSelf()
    {
        int index = 0;

        // If the head's coordinate is found somewhere else other than the last position in the list (Last because the head is the most recent element)
        foreach (var coord in BodyCoordinates)
            if (index++ < BodyCoordinates.Count - 1 && coord.Equals(Head))
                return true;

        return false;
    }


    bool CollidesWithBounds()
    {
        return (_Path == Path.Vertical) ?
            _Direction == Direction.Negative && Head.Y == VerticalBounds.X || (_Direction == Direction.Positive && Head.Y == VerticalBounds.Y) :
            _Direction == Direction.Negative && Head.X == HorizontalBounds.X || (_Direction == Direction.Positive && Head.X == HorizontalBounds.Y);
    }

    public void Reset()
    {
        BodyCoordinates.Clear();
    }
}
