using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource sfxSource;
    public AudioSource bgmSource;

    [Header("SFX Clips")]
    public AudioClip handGrabClip;
    public AudioClip fruitPickupClip;
    public AudioClip checkpointClip;
    public AudioClip deathClip;
    public AudioClip respawnClip;

    [Header("BGM")]
    public AudioClip bgmClip;
    public bool playBgmOnStart = true;

    [Header("Volume")]
    [Range(0f, 1f)] public float sfxVolume = 1f;
    [Range(0f, 1f)] public float bgmVolume = 0.4f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        if (playBgmOnStart)
        {
            PlayBGM();
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    public void PlayBGM()
    {
        if (bgmClip == null || bgmSource == null) return;

        bgmSource.clip = bgmClip;
        bgmSource.loop = true;
        bgmSource.volume = bgmVolume;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        if (bgmSource == null) return;
        bgmSource.Stop();
    }

    public void PlayHandGrab()
    {
        PlaySFX(handGrabClip);
    }

    public void PlayFruitPickup()
    {
        PlaySFX(fruitPickupClip);
    }

    public void PlayCheckpoint()
    {
        PlaySFX(checkpointClip);
    }

    public void PlayDeath()
    {
        PlaySFX(deathClip);
    }

    public void PlayRespawn()
    {
        PlaySFX(respawnClip);
    }
}