
// The width of the window MUST be odd - refer to #1 in the documentation for more information
Console.SetWindowSize(61,32);

Console.CursorVisible = false;
Console.BackgroundColor = ConsoleColor.Black;

GameApp.Instance.ClearWindow();
GameApp.Instance.Run();