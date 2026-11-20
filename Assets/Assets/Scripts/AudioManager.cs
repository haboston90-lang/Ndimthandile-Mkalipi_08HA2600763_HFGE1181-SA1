using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Clips - drag from your asset pack")]
    public AudioClip eatClip;
    public AudioClip hitClip;
    public AudioClip clickClip;
    public AudioClip growClip;
    public AudioClip backgroundMusic;

    [Header("Sources")]
    public AudioSource sfxSource;
    public AudioSource musicSource;

    void Awake()
    {
        if (sfxSource == null) sfxSource = gameObject.AddComponent<AudioSource>();
        if (musicSource == null) musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        DontDestroyOnLoad(gameObject);
    }

    public void PlayEat() { if (eatClip) sfxSource.PlayOneShot(eatClip); }
    public void PlayHit() { if (hitClip) sfxSource.PlayOneShot(hitClip); }
    public void PlayClick() { if (clickClip) sfxSource.PlayOneShot(clickClip); }
    public void PlayGrow() { if (growClip) sfxSource.PlayOneShot(growClip); }
    public void PlayMusic()
    {
        if (backgroundMusic && !musicSource.isPlaying)
        {
            musicSource.clip = backgroundMusic;
            musicSource.Play();
        }
    }
    public void StopMusic() { if (musicSource) musicSource.Stop(); }
}