// Handles highlighting menu items and helps with selection

class MenuOptionsSelector
{
    // The various scene types and the menu options which lead to them
    KeyValuePair<SceneType, string>[] options;
    
    public KeyValuePair<SceneType, string> CurrentSelectedOption { get; private set; }

    ConsoleColor highlightorColor;
    ConsoleColor foregroundColor;

    ConsoleColor currentForegroundColor;
    ConsoleColor currentBackgroundColor;

    int size;
    int index = 0;


    public MenuOptionsSelector(KeyValuePair<SceneType, string>[] options, ConsoleColor currentForegroundColor, ConsoleColor currentBackgroundColor, int size = 6)
    {
        this.options = options;
        CurrentSelectedOption = this.options[0];

        highlightorColor = ConsoleColor.Red;
        foregroundColor = ConsoleColor.White;

        this.currentForegroundColor = currentForegroundColor;
        this.currentBackgroundColor = currentBackgroundColor;

        this.size = size;
    }


    public void HighlightCurrentSelectedOption()
    {
        string highlighText = new string(' ', size) + CurrentSelectedOption.Value + new string(' ', size);

        Console.BackgroundColor = highlightorColor;
        GameApp.Instance.Display(highlighText, GameApp.Instance.CenterHorizontally(highlighText), (int)CurrentSelectedOption.Key,foregroundColor);
        
        Console.BackgroundColor = currentBackgroundColor;
    }


    public void HighlightNextOption()
    {
        if (++index < options.Length)
        {
            DeHighlightSelectedOption();
            CurrentSelectedOption = options[index];
            HighlightCurrentSelectedOption();
        }
        else --index;
    }

    
    public void HighlightPrevOption()
    {
        if (--index >= 0)
        {
            DeHighlightSelectedOption();
            CurrentSelectedOption = options[index];
            HighlightCurrentSelectedOption();
        }
        else ++index;
    }


    private void DeHighlightSelectedOption()
    {
        string highlighText = " " + new string(' ', size) + CurrentSelectedOption.Value + new string(' ', size) + " ";
        GameApp.Instance.Display(highlighText, GameApp.Instance.CenterHorizontally(highlighText), (int)CurrentSelectedOption.Key, currentForegroundColor);
    }
}