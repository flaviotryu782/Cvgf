using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource ambienceSource;
    [SerializeField] private AudioSource effectsSource;
    [SerializeField] private AudioSource commentarySource;

    [Header("Music")]
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip matchMusic;
    [SerializeField, Range(0f, 1f)] private float musicVolume = .35f;

    [Header("Effects")]
    [SerializeField] private AudioClip kickSound;
    [SerializeField] private AudioClip passSound;
    [SerializeField] private AudioClip goalSound;
    [SerializeField] private AudioClip whistleSound;
    [SerializeField] private AudioClip saveSound;
    [SerializeField] private AudioClip celebrationSound;
    [SerializeField] private AudioClip crowdLoop;
    [SerializeField] private AudioClip[] commentaryClips;
    [SerializeField, Range(0f, 1f)] private float effectsVolume = .8f;
    [SerializeField, Range(0f, 1f)] private float crowdVolume = .35f;
    [SerializeField] private bool vibrationEnabled = true;

    public bool VibrationEnabled { get => vibrationEnabled; set => vibrationEnabled = value; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        if (musicSource != null) musicSource.loop = true;
        if (ambienceSource != null) ambienceSource.loop = true;
    }

    public void PlayMenuMusic() => PlayMusic(menuMusic);
    public void PlayMatchMusic() => PlayMusic(matchMusic);

    public void PlayMusic(AudioClip clip)
    {
        if (musicSource == null || clip == null) return;
        if (musicSource.clip == clip && musicSource.isPlaying) return;
        musicSource.clip = clip;
        musicSource.volume = musicVolume;
        musicSource.Play();
    }

    public void StartCrowd()
    {
        if (ambienceSource == null || crowdLoop == null) return;
        ambienceSource.clip = crowdLoop;
        ambienceSource.volume = crowdVolume;
        if (!ambienceSource.isPlaying) ambienceSource.Play();
    }

    public void PlayKick() => PlayEffect(kickSound);
    public void PlayPass() => PlayEffect(passSound);
    public void PlayGoal() { PlayEffect(goalSound); Vibrate(.25f); }
    public void PlayWhistle() => PlayEffect(whistleSound);
    public void PlaySave() { PlayEffect(saveSound); Vibrate(.12f); }
    public void PlayCelebration() => PlayEffect(celebrationSound);

    public void PlayCommentary()
    {
        if (commentarySource == null || commentaryClips == null || commentaryClips.Length == 0) return;
        AudioClip clip = commentaryClips[Random.Range(0, commentaryClips.Length)];
        commentarySource.PlayOneShot(clip, effectsVolume);
    }

    public void PlayEffect(AudioClip clip)
    {
        if (effectsSource != null && clip != null) effectsSource.PlayOneShot(clip, effectsVolume);
    }

    public void Vibrate(float duration = .08f)
    {
        if (vibrationEnabled && Application.isMobilePlatform) Handheld.Vibrate();
    }
}
