public struct Vector2
{
    public int X;
    public int Y;

    public Vector2(int x, int y)
    {
        X = x;
        Y = y;
    }

    public Vector2(Vector2 vector)
    {
        X = vector.X;
        Y = vector.Y;
    }

    public override string ToString() => $"(x: {X}, y: {Y})";

    public bool Equals(Vector2 other) => this.X == other.X && this.Y == other.Y;

}