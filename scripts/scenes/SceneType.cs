// Denotes the type of scene

// These values are deliberately assigned in such a manner to favour the MainScene. The values represent the row in the main scene
// where the coresponding options will be placed
public enum SceneType
{
    MainScene = -1, // Not in the main scene, since it is the main scene itself
    NewGameScene = 11,
    ScoreScene = 14,
    OptionsScene = 17,
    HelpScene = 20,
    AboutScene = 23,
    Quit = 26
}