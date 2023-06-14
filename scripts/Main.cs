// The width of the window MUST be odd - refer to #1 in the documentation for more information
Console.SetWindowSize(65,35);
Console.SetBufferSize(Console.WindowWidth, Console.WindowHeight);
Console.Title = "Retro Snake";

Console.CursorVisible = false;
Console.BackgroundColor = ConsoleColor.Black;

GameApp.Instance.Run();