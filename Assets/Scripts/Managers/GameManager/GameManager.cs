using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private LevelManager levelManager;

    //debug
    public bool startGame = false;

    MenuManager UIController;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public enum GameStates
    {
        Menu,
        GamePaused,
        GameInProgress,
        GameFinished,
    }

    private GameStates gameState;
    public GameStates GameState
    {
        get { return gameState; }
    }

    public void Start()
    {
        gameState = GameStates.Menu;
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void OnLevelWasLoaded(int level)
    {
        string sceneName = SceneUtility.GetScenePathByBuildIndex(level);
        sceneName = System.IO.Path.GetFileNameWithoutExtension(sceneName);

        if (sceneName.Contains("Level"))
        {
            levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
            gameState = GameStates.GamePaused;

            UIController = GameObject.Find("GameUI").GetComponent<MenuManager>();
        }
        else
        {
            gameState = GameStates.Menu;
        }
    }

    public void StartLevel()
    {
        if (levelManager == null)
        {
            if (!GameObject.Find("LevelManager").TryGetComponent<LevelManager>(out levelManager))
            {
                return;
            }
        }

        if (UIController == null)
        {
            if (!GameObject.Find("GameUI").TryGetComponent<MenuManager>(out UIController))
            {
                Debug.LogWarning("No \"GameUI\" present");
            }
        }

        levelManager.StartGame();
        gameState = GameStates.GameInProgress;
        
    }

    public void GameFinished(TeamInfo.TeamId teamId)
    {
        levelManager = null;
        gameState = GameStates.GameFinished;

        //Code for Ui here yippeyayyy game won 
        Debug.Log("Game Won by " +  teamId);

        if (teamId == TeamInfo.TeamId.PlayerTeam)
        {
            UIController.GoToPage("PlayerWin");
        }
        else
        {
            UIController.GoToPage("PlayerLost");
        }
    }

    //Debug ignore
    private void Update()
    {
        if (startGame)
        {
            StartLevel();
        }
    }
}
