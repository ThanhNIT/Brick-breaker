using UnityEngine;
using UnityEngine.InputSystem;

public class PaddleController : MonoBehaviour
{
    public float speed = 10f;
    public Transform wallLeft;
    public Transform wallRight;

    private Rigidbody2D rb;
    private Camera cam;
    private float minX;
    private float maxX;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;

        float camHalfWidth = cam.orthographicSize * cam.aspect;
        float halfPaddle = GetComponent<SpriteRenderer>().bounds.size.x / 2f;

        minX = -camHalfWidth + halfPaddle;
        maxX = camHalfWidth - halfPaddle;
    }

    void FixedUpdate()
    {
        float newX = GetTargetX();
        newX = Mathf.Clamp(newX, minX, maxX);
        rb.MovePosition(new Vector2(newX, rb.position.y));
    }

float GetTargetX()
{
    if (Input.touchCount > 0)
    {
        Vector2 touchPos = Input.GetTouch(0).position;

        // Validate tọa độ hợp lệ trước khi dùng
        if (float.IsInfinity(touchPos.x) || float.IsInfinity(touchPos.y) ||
            float.IsNaN(touchPos.x) || float.IsNaN(touchPos.y))
        {
            return rb.position.x;
        }

        // Validate nằm trong màn hình
        if (touchPos.x < 0 || touchPos.x > Screen.width ||
            touchPos.y < 0 || touchPos.y > Screen.height)
        {
            return rb.position.x;
        }

        Vector3 tp = cam.ScreenToWorldPoint(
            new Vector3(touchPos.x, touchPos.y, 10f));
        return tp.x;
    }

    #if UNITY_EDITOR
    Vector2 mousePos = Input.mousePosition;
    if (mousePos.x >= 0 && mousePos.x <= Screen.width &&
        mousePos.y >= 0 && mousePos.y <= Screen.height)
    {
        Vector3 mp = cam.ScreenToWorldPoint(
            new Vector3(mousePos.x, mousePos.y, 10f));
        return mp.x;
    }
    #endif

    return rb.position.x;
}

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.Play(AudioManager.Instance.paddleHitClip);

            Rigidbody2D ballRb = collision.gameObject.GetComponent<Rigidbody2D>();
            float offset = (collision.transform.position.x - transform.position.x)
                           / (GetComponent<SpriteRenderer>().bounds.size.x / 2f);
            float spd = ballRb.linearVelocity.magnitude;
            ballRb.linearVelocity = new Vector2(offset * spd * 0.8f, Mathf.Abs(ballRb.linearVelocity.y));
        }
    }
}