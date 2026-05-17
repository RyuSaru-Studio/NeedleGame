using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Sources")]
    public AudioSource sfxSource; // one-shot sounds
    public AudioSource loopSource; // for looping sounds (needle swing)

    [Header("Clips")]
    public AudioClip[] uiClicks; // e.g., sfx_input_click1..8
    public AudioClip timingStart;
    public AudioClip timingSwingLoop;
    public AudioClip levelComplete;
    public AudioClip levelFailed;
    public AudioClip hitPerfect;
    public AudioClip hitGood;
    public AudioClip hitMiss;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (sfxSource == null)
            sfxSource = gameObject.AddComponent<AudioSource>();
        if (loopSource == null)
        {
            loopSource = gameObject.AddComponent<AudioSource>();
            loopSource.loop = true;
        }
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip, volume);
    }

    public void PlayClick()
    {
        if (uiClicks != null && uiClicks.Length > 0)
        {
            var clip = uiClicks[Random.Range(0, uiClicks.Length)];
            PlaySFX(clip);
        }
    }

    public void PlayTimingStart()
    {
        PlaySFX(timingStart);
    }

    public void PlayHitResult(TimingResult result)
    {
        switch (result)
        {
            case TimingResult.Perfect:
                PlaySFX(hitPerfect);
                break;
            case TimingResult.Good:
                PlaySFX(hitGood);
                break;
            case TimingResult.Miss:
                PlaySFX(hitMiss);
                break;
        }
    }

    public void PlayLoop(AudioClip clip)
    {
        if (clip == null || loopSource == null) return;
        if (loopSource.clip == clip && loopSource.isPlaying) return;
        loopSource.clip = clip;
        loopSource.Play();
    }

    public void StopLoop()
    {
        if (loopSource == null) return;
        loopSource.Stop();
        loopSource.clip = null;
    }

    public void SetMasterVolume(float v)
    {
        if (sfxSource != null) sfxSource.volume = v;
        if (loopSource != null) loopSource.volume = v;
    }
}

// TimingResult enum is defined in TimingEvaluator.cs; do not redefine here to avoid compile conflicts.
