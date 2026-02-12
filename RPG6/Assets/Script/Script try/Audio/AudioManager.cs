using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-100)]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Music Clips")]
    public AudioClip menuMusic;
    public AudioClip gameMusic;
    public AudioClip bossMusic;

    [Header("SFX Clips")]
    public AudioClip hitSfx;

    [Header("Sources")]
    public AudioSource musicSourceA;
    public AudioSource musicSourceB;
    public AudioSource sfxSource;

    [Header("Settings")]
    public float musicFadeDuration = 1f;
    [Range(0f, 1f)] public float musicVolume = 0.5f;
    [Range(0f, 1f)] public float sfxVolume = 0.6f;

    private AudioSource activeMusic;
    private AudioSource incomingMusic;
    private Coroutine crossfadeCo;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // ensure sources exist
        if (musicSourceA == null) musicSourceA = gameObject.AddComponent<AudioSource>();
        if (musicSourceB == null) musicSourceB = gameObject.AddComponent<AudioSource>();
        if (sfxSource == null) sfxSource = gameObject.AddComponent<AudioSource>();

        musicSourceA.loop = true;
        musicSourceB.loop = true;
        musicSourceA.volume = musicVolume;
        musicSourceB.volume = musicVolume;
        sfxSource.volume = sfxVolume;

        activeMusic = musicSourceA;
        incomingMusic = musicSourceB;

        SceneManager.sceneLoaded += OnSceneLoaded;

        // auto-play menu music if assigned (useful for start/menu scene)
        if (menuMusic != null && (gameMusic == null || musicSourceA.clip == null && musicSourceB.clip == null))
        {
            PlayMusic(menuMusic, true, 0.5f);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Simple policy: when a scene loads, switch to gameMusic if assigned
        if (!string.IsNullOrEmpty(scene.name))
        {
            // if menuMusic assigned and scene name contains "Menu" or "Start", play menu
            string lower = scene.name.ToLowerInvariant();
            if ((lower.Contains("menu") || lower.Contains("start")) && menuMusic != null)
            {
                PlayMusic(menuMusic, true);
                return;
            }
        }

        if (gameMusic != null)
        {
            PlayMusic(gameMusic, true);
        }
    }

    public void PlayMusic(AudioClip clip, bool loop = true, float fade = -1f)
    {
        if (clip == null) return;
        if (fade <= 0f) fade = musicFadeDuration;
        if (activeMusic.clip == clip) return;

        if (crossfadeCo != null) StopCoroutine(crossfadeCo);
        crossfadeCo = StartCoroutine(CrossfadeTo(clip, loop, fade));
    }

    private IEnumerator CrossfadeTo(AudioClip clip, bool loop, float duration)
    {
        var incoming = activeMusic == musicSourceA ? musicSourceB : musicSourceA;
        incoming.clip = clip;
        incoming.loop = loop;
        incoming.volume = 0f;
        incoming.Play();

        float t = 0f;
        float startVolActive = activeMusic.volume;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float p = Mathf.Clamp01(t / duration);
            incoming.volume = Mathf.Lerp(0f, musicVolume, p);
            activeMusic.volume = Mathf.Lerp(startVolActive, 0f, p);
            yield return null;
        }

        activeMusic.Stop();
        activeMusic.volume = musicVolume;
        activeMusic = incoming;
        crossfadeCo = null;
    }

    public void PlayBossMusic(bool show)
    {
        if (show)
        {
            if (bossMusic != null)
                PlayMusic(bossMusic, true);
        }
        else
        {
            // fall back to game music
            if (gameMusic != null)
                PlayMusic(gameMusic, true);
        }
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip, volume * sfxVolume);
    }

    public void PlayHitSFX()
    {
        if (hitSfx == null) return;
        PlaySFX(hitSfx);
    }
}
