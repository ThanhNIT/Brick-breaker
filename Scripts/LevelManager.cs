using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Levels")]
    public LevelData[] levels;
    public int currentLevel = 0;

    [Header("Prefabs")]
    public GameObject brickPrefab;
    public GameObject ballPrefab;

    [Header("Layout")]
    public float paddingX = 0.12f;
    public float paddingY = 0.15f;
    public Vector2 startPosition = new Vector2(0f, 4f);

    void Start()
    {
        SpawnLevel();
    }

    void SpawnLevel()
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        LevelData data = levels[currentLevel];

        // Tính khoảng trống thực tế giữa 2 tường
        float camHalfWidth = Camera.main.orthographicSize * Camera.main.aspect;
        float wallThickness = 0.3f; // khớp với độ dày tường
        float availableWidth = (camHalfWidth - wallThickness) * 2f;

        // Tự tính brickWidth vừa khít, có padding 2 bên
        float sidePadding = 0.2f;
        float usableWidth = availableWidth - sidePadding * 2f;
        float brickW = (usableWidth - paddingX * (data.columns - 1)) / data.columns;

        float totalWidth = data.columns * (brickW + paddingX) - paddingX;
        float startX = -totalWidth / 2f + brickW / 2f;
        float startY = startPosition.y;

        int brickCount = 0;
        for (int row = 0; row < data.rows; row++)
        {
            for (int col = 0; col < data.columns; col++)
            {
                float x = startX + col * (brickW + paddingX);
                float y = startY - row * (data.brickHeight + paddingY);

                GameObject brick = Instantiate(brickPrefab, new Vector2(x, y), Quaternion.identity);
                brick.transform.parent = transform;

                // Scale brick theo brickW tính được
                brick.transform.localScale = new Vector3(brickW, data.brickHeight, 1f);

                BrickController bc = brick.GetComponent<BrickController>();
                if (bc != null && row < data.hpPerRow.Length)
                {
                    bc.maxHP = data.hpPerRow[row];
                    bc.Init();
                }

                brickCount++;
            }
        }

        if (GameManager.Instance != null)
            GameManager.Instance.ResetForNewLevel(brickCount);

        SpawnBalls(data.ballCount);
    }

    public void SpawnBalls(int count)
    {
        foreach (var b in FindObjectsByType<BallController>(FindObjectsSortMode.None))
            Destroy(b.gameObject);

        for (int i = 0; i < count; i++)
        {
            float offsetX = (i - (count - 1) / 2f) * 1.2f;
            Vector2 pos = new Vector2(offsetX, -6.5f);
            Instantiate(ballPrefab, pos, Quaternion.identity);
        }

        // Báo GameManager số bóng hiện tại
        if (GameManager.Instance != null)
            GameManager.Instance.SetActiveBalls(count);
    }

    public void LoadNextLevel()
    {
        currentLevel = (currentLevel + 1) % levels.Length;
        SpawnLevel();
    }
}