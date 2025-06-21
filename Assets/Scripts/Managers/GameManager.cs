using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.InputSystem.LowLevel;

public class GameManager : MonoBehaviour
{
    [Header("Game State")]
    public GameState currentState = GameState.MainMenu;

    [Header("UI Elements")]
    public GameObject mainMenuUI;
    public GameObject gameUI;
    public GameObject gameOverUI;
    public GameObject settingsUI;
    public Text scoreText;
    public Text finalScoreText;
    public Text highScoreText;
    public Text logoText;
    public Text pressSpaceText;

    [Header("Game Objects")]
    public PlayerController player;
    public EnemyController enemy;
    public PaperPlaneSpawner planeSpawner;

    [Header("Audio")]
    public AudioSource backgroundMusic;

    private int currentScore = 0;
    private int highScore = 0;
    private float scoreTimer = 0f;

    public static GameManager Instance;

    public int CurrentScore => currentScore;
    public int HighScore => highScore;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadHighScore();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetGameState(GameState.MainMenu);
        Time.timeScale = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();

        if (currentState == GameState.Playing)
        {
            UpdateScore();
        }
    }
    private void HandleInput()
    {
        switch (currentState)
        {
            case GameState.MainMenu:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    StartGame();
                }
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    OpenSettings();
                }
                break;

            case GameState.GameOver:
                if (Input.GetMouseButtonDown(0))
                {
                    RestartGame();
                }
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    OpenSettings();
                }
                break;

            case GameState.Settings:
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    CloseSettings();
                }
                break;
        }
    }
    private void UpdateScore()
    {
        scoreTimer += Time.deltaTime;
        if (scoreTimer >= 1f)
        {
            currentScore++;
            scoreText.text = currentScore.ToString();
            scoreTimer = 0f;

            planeSpawner.UpdateDifficulty(currentScore);
        }
    }
    public void StartGame()
    {
        currentScore = 0;
        scoreTimer = 0f;
        SetGameState(GameState.Playing);

        StartCoroutine(HideMenuElements());

        player.ResetPosition();
        enemy.StartChasing();
        planeSpawner.StartSpawning();
    }
    private IEnumerator HideMenuElements()
    {
        float fadeTime = 0.5f;
        float elapsedTime = 0f;

        Color logoColor = logoText.color;
        Color pressSpaceColor = pressSpaceText.color;

        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeTime);

            logoText.color = new Color(logoColor.r, logoColor.g, logoColor.b, alpha);
            pressSpaceText.color = new Color(pressSpaceColor.r, pressSpaceColor.g, pressSpaceColor.b, alpha);

            yield return null;
        }

        logoText.gameObject.SetActive(false);
        pressSpaceText.gameObject.SetActive(false);
        scoreText.gameObject.SetActive(true);
    }
    public void GameOver()
    {
        SetGameState(GameState.GameOver);

        if (currentScore > highScore)
        {
            highScore = currentScore;
            SaveHighScore();
        }
        finalScoreText.text = "Score: " + currentScore;
        highScoreText.text = "High Score: " + highScore;

        planeSpawner.StopSpawning();

        StartCoroutine(GameOverTransition());
    }
    private IEnumerator GameOverTransition()
    {
        yield return new WaitForSeconds(1f);
        gameOverUI.SetActive(true);
    }
    public void RestartGame()
    {
        logoText.gameObject.SetActive(true);
        pressSpaceText.gameObject.SetActive(true);
        logoText.color = Color.white;
        pressSpaceText.color = Color.white;
        scoreText.gameObject.SetActive(false);

        planeSpawner.ClearAllPlanes();

        SetGameState(GameState.MainMenu);
    }
    public void OpenSettings()
    {
        settingsUI.SetActive(true);
        SetGameState(GameState.Settings);
    }
    public void CloseSettings()
    {
        settingsUI.SetActive(false);
        SetGameState(currentState == GameState.Settings ? GameState.MainMenu : currentState);
    }
    private void SetGameState(GameState newState)
    {
        currentState = newState;

        mainMenuUI.SetActive(newState == GameState.MainMenu);
        gameUI.SetActive(newState == GameState.Playing);
        gameOverUI.SetActive(newState == GameState.GameOver);
    }
    private void SaveHighScore()
    {
        PlayerPrefs.SetInt("HighScore", highScore);
        PlayerPrefs.Save();
    }
    private void LoadHighScore()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
    }
    public enum GameState
    {
        MainMenu,
        Playing,
        GameOver,
        Settings
    }
}
