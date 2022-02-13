public class GameStateManager
{
    private static GameStateManager _instance;
    public static GameStateManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameStateManager();
            }
            return _instance;
        }
    }

    public GameState CurrentGameState { get; private set; }

    public delegate void GameStatechangeHandler(GameState newGameState);
    public event GameStatechangeHandler OnGameStateChanged;

    private GameStateManager()
    {

    }

    public void SetState(GameState newGameState)
    {
        if (newGameState == CurrentGameState)
        {
            return;
        }

        CurrentGameState = newGameState;
        OnGameStateChanged?.Invoke(newGameState);
    }
}
