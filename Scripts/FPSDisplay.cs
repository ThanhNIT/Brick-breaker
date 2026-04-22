using UnityEngine;
using TMPro;

public class FPSDisplay : MonoBehaviour
{
    public TextMeshProUGUI fpsText;
    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 0.5f)
        {
            int fps = Mathf.RoundToInt(1f / Time.deltaTime);
            fpsText.text = "FPS: " + fps;
            timer = 0f;
        }
    }
}