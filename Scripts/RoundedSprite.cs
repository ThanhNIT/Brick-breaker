using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(SpriteRenderer))]
public class RoundedSprite : MonoBehaviour
{
    [Range(0f, 0.5f)]
    public float radius = 0.3f;

    private static Dictionary<float, Sprite> spriteCache = new Dictionary<float, Sprite>();

    void Awake()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color currentColor = sr.color;
        sr.sprite = GetOrGenerate(radius);
        sr.color = currentColor;
    }

    Sprite GetOrGenerate(float r)
    {
        if (spriteCache.TryGetValue(r, out Sprite cached))
            return cached;

        Sprite generated = Generate(r);
        spriteCache[r] = generated;
        return generated;
    }

    Sprite Generate(float radiusNorm)
    {
        int texW = 128, texH = 128;
        float r = radiusNorm * texH;

        Texture2D tex = new Texture2D(texW, texH, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        Color[] pixels = new Color[texW * texH];

        for (int y = 0; y < texH; y++)
            for (int x = 0; x < texW; x++)
                pixels[y * texW + x] = Inside(x, y, texW, texH, r) ? Color.white : Color.clear;

        tex.SetPixels(pixels);
        tex.Apply();

        return Sprite.Create(tex, new Rect(0, 0, texW, texH), new Vector2(0.5f, 0.5f), 100f);
    }

    bool Inside(int x, int y, int w, int h, float r)
    {
        float cx = Mathf.Clamp(x, r, w - r - 1);
        float cy = Mathf.Clamp(y, r, h - r - 1);
        float dx = x - cx, dy = y - cy;
        return dx * dx + dy * dy <= r * r;
    }
}
