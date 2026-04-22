using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

public class BallController : MonoBehaviour
{
    public float launchSpeed = 8f;
    public GameObject tapToStartUI;

    private Rigidbody2D rb;
    private bool isLaunched = false;
    private float stuckTimer = 0f;

    void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (tapToStartUI != null) tapToStartUI.SetActive(true);
    }

    void Update()
    {
        if (isLaunched) return;

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == UnityEngine.TouchPhase.Began)
        {
            Launch();
            return;
        }

        if (Input.GetMouseButtonDown(0))
            Launch();
    }

    void Launch()
    {
        isLaunched = true;
        rb.linearVelocity = new Vector2(launchSpeed * 0.6f, launchSpeed);
        if (GameManager.Instance != null)
            GameManager.Instance.HideTapToStart();
    }

    void FixedUpdate()
    {
        if (!isLaunched) return;

        // Maintain constant speed to counteract physics drift
        rb.linearVelocity = rb.linearVelocity.normalized * launchSpeed;

        // Push ball down if stuck near ceiling for too long
        if (transform.position.y > 0f)
        {
            stuckTimer += Time.fixedDeltaTime;
            if (stuckTimer > 3f)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, -Mathf.Abs(rb.linearVelocity.y));
                stuckTimer = 0f;
            }
        }
        else
        {
            stuckTimer = 0f;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isLaunched) return;

        // Enforce minimum vertical angle only on bounce, not every frame
        Vector2 vel = rb.linearVelocity;
        float minVertical = launchSpeed * 0.3f;
        if (Mathf.Abs(vel.y) < minVertical)
        {
            vel.y = vel.y >= 0 ? minVertical : -minVertical;
            vel.x = Mathf.Sqrt(launchSpeed * launchSpeed - vel.y * vel.y) * Mathf.Sign(vel.x);
            rb.linearVelocity = vel;
        }
    }

    public void ResetBall(Vector2 startPos)
    {
        isLaunched = false;
        rb.linearVelocity = Vector2.zero;
        transform.position = startPos;
        stuckTimer = 0f;
        if (GameManager.Instance != null)
            GameManager.Instance.ShowTapToStart();
    }
}
