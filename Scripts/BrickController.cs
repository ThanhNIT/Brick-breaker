using UnityEngine;

public class BrickController : MonoBehaviour
{
    public int maxHP = 1;
    private int currentHP;

    // Màu theo HP — tuỳ chỉnh trong Inspector
    public Color[] hpColors = {
        new Color(0.36f, 0.80f, 0.65f), // HP 1 — xanh teal
        new Color(0.94f, 0.62f, 0.16f), // HP 2 — vàng cam
        new Color(0.85f, 0.35f, 0.19f), // HP 3 — đỏ cam
    };

    private SpriteRenderer sr;

    [Header("Effects")]
    public GameObject particlePrefab;

    void SpawnParticle()
    {
        if (particlePrefab != null)
        {
            GameObject p = Instantiate(particlePrefab, transform.position, Quaternion.identity);
            // Tự xoá sau 1 giây
            Destroy(p, 1f);
        }
    }

    void Start()
    {
        Init();
    }

    public void Init()
    {
        currentHP = maxHP;
        sr = GetComponent<SpriteRenderer>();
        UpdateColor();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            TakeHit();
        }
    }

    void TakeHit()
    {
        currentHP--;

        if (currentHP <= 0)
        {
            if (GameManager.Instance != null)
                GameManager.Instance.AddScore(10);

            // Âm thanh vỡ
            if (AudioManager.Instance != null)
                AudioManager.Instance.Play(AudioManager.Instance.brickBreakClip);

            SpawnParticle();
            Destroy(gameObject);
        }
        else
        {
            // Âm thanh bị đánh nhưng chưa vỡ
            if (AudioManager.Instance != null)
                AudioManager.Instance.Play(AudioManager.Instance.brickHitClip);

            UpdateColor();
        }
    }

    void UpdateColor()
    {
        int colorIndex = Mathf.Clamp(currentHP - 1, 0, hpColors.Length - 1);
        sr.color = hpColors[colorIndex];
    }
}