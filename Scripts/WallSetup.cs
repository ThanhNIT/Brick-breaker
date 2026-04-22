using UnityEngine;

public class WallSetup : MonoBehaviour
{
    void Awake()
    {
        Camera cam = Camera.main;
        float h = cam.orthographicSize;
        float w = h * cam.aspect;

        EdgeCollider2D edge = GetComponent<EdgeCollider2D>();

        // Vẽ 3 cạnh: trái → trên → phải (không có đáy)
        edge.points = new Vector2[]
        {
            new Vector2(-w,  -h),   // dưới trái
            new Vector2(-w,   h),   // trên trái
            new Vector2( w,   h),   // trên phải
            new Vector2( w,  -h),   // dưới phải
        };
    }
}