using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Sound Clips")]
    public AudioClip brickBreakClip;
    public AudioClip brickHitClip;
    public AudioClip paddleHitClip;
    public AudioClip loseLifeClip;
    public AudioClip winClip;

    private AudioSource[] sources;
    private int sourceIndex = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Tạo sẵn 8 AudioSource để tránh delay
        sources = new AudioSource[8];
        for (int i = 0; i < sources.Length; i++)
        {
            sources[i] = gameObject.AddComponent<AudioSource>();
            sources[i].playOnAwake = false;
        }
    }

    public void Play(AudioClip clip)
    {
        if (clip == null) return;

        // Tìm source không đang phát, hoặc dùng xoay vòng
        AudioSource src = sources[sourceIndex];
        sourceIndex = (sourceIndex + 1) % sources.Length;

        src.clip = clip;
        src.Play();
    }
}