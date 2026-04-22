using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI livesText;
    public GameObject winPanel;
    public GameObject losePanel;

    [Header("Game Settings")]
    public int startingLives = 3;

    public GameObject tapToStartUI;

    private int score = 0;
    private int lives;
    private int totalBricks = 0;
    private int bricksDestroyed = 0;
    private bool gameOver = false;
    private int activeBalls = 0;

    public TextMeshProUGUI levelText;

    private LevelManager levelManager;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        levelManager = FindFirstObjectByType<LevelManager>();
    }

    public void HideTapToStart()
    {
        if (tapToStartUI != null) tapToStartUI.SetActive(false);
    }

    public void ShowTapToStart()
    {
        if (tapToStartUI != null) tapToStartUI.SetActive(true);
    }

    void Start()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
        lives = startingLives;
        levelManager = FindFirstObjectByType<LevelManager>();
        activeBalls = FindObjectsByType<BallController>(FindObjectsSortMode.None).Length;
        UpdateUI();
    }

    public void AddScore(int points)
    {
        if (gameOver) return;
        score += points;
        bricksDestroyed++;
        UpdateUI();

        if (totalBricks - bricksDestroyed <= 0)
            TriggerWin();
    }

    public void ResetForNewLevel(int brickCount)
    {
        bricksDestroyed = 0;
        totalBricks = brickCount;
        UpdateUI();
    }

    public void LoseLife()
    {
        if (gameOver) return;
        lives--;
        UpdateUI();

        if (lives <= 0)
            TriggerLose();
        else
            RespawnBalls();
    }

    public void SetActiveBalls(int count)
    {
        activeBalls = count;
    }

    public void BallFell()
    {
        activeBalls--;
        if (activeBalls > 0) return;

        lives--;
        UpdateUI();

        if (lives <= 0)
            TriggerLose();
        else
            RespawnBalls();
    }

    void RespawnBalls()
    {
        if (levelManager != null)
            levelManager.SpawnBalls(levelManager.levels[levelManager.currentLevel].ballCount);
        else
        {
            BallController ball = FindFirstObjectByType<BallController>();
            if (ball != null) ball.ResetBall(new Vector2(0, -6.5f));
        }

        ShowTapToStart();
    }

    void TriggerLose()
    {
        gameOver = true;
        if (losePanel != null) losePanel.SetActive(true);
    }

    void TriggerWin()
    {
        int nextLevel = levelManager.currentLevel + 1;

        if (nextLevel >= levelManager.levels.Length)
        {
            gameOver = true;
            AdsManager.Instance?.ShowInterstitial();
            if (winPanel != null) winPanel.SetActive(true);
        }
        else
        {
            if (nextLevel % 2 == 0)
                AdsManager.Instance?.ShowInterstitial();

            bricksDestroyed = 0;
            levelManager.LoadNextLevel();
        }
    }

    public void OnWatchAdButton()
    {
        AdsManager.Instance?.ShowRewarded(() =>
        {
            lives = 1;
            gameOver = false;
            if (losePanel != null) losePanel.SetActive(false);
            RespawnBalls();
            UpdateUI();
        });
    }

    IEnumerator ShowLevelTransition(int levelNumber)
    {
        if (tapToStartUI != null)
            tapToStartUI.SetActive(true);
        yield return new WaitForSeconds(1.5f);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void UpdateUI()
    {
        if (scoreText != null) scoreText.text = "Score: " + score;
        if (livesText != null) livesText.text = "Lives: " + lives;
        if (levelText != null && levelManager != null)
            levelText.text = "Level " + (levelManager.currentLevel + 1);
    }
}
