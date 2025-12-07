using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }
    
    [Header("Music Settings")]
    public AudioClip backgroundMusic; // Assign your .wav file here
    [Range(0f, 1f)] public float volume = 0.5f;
    public bool playOnStart = true;
    
    private AudioSource audioSource;
    
    void Awake()
    {
        // Singleton pattern - only one MusicManager
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SetupAudioSource();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        if (playOnStart && backgroundMusic != null)
        {
            PlayMusic();
        }
    }
    
    void SetupAudioSource()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = backgroundMusic;
        audioSource.volume = volume;
        audioSource.loop = true; // ← THIS is where you set loop to true!
        audioSource.playOnAwake = false;
        
        // Optional: Set spatial blend to 2D for background music
        audioSource.spatialBlend = 0f; // 0 = fully 2D, 1 = fully 3D
    }
    
    public void PlayMusic()
    {
        if (audioSource != null && backgroundMusic != null && !audioSource.isPlaying)
        {
            audioSource.Play();
            Debug.Log($"Playing music: {backgroundMusic.name}");
        }
    }
    
    public void StopMusic()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
    
    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume);
        if (audioSource != null)
        {
            audioSource.volume = volume;
        }
    }
    
    public bool IsPlaying()
    {
        return audioSource != null && audioSource.isPlaying;
    }
}