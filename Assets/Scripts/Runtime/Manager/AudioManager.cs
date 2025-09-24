using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


[System.Serializable]
public struct AudioInfo
{
    public string name;
    public AudioClip clip;
}
public class AudioManager : PersistentSingleton<AudioManager>
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Clips Library")]
    public List<AudioInfo> sfxClips; // drag in your clips here
    public List<AudioInfo> musicClips; // drag in your clips here

    [Header("Volumes")]
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    private const string MASTER_KEY = "MasterVolume";
    private const string MUSIC_KEY = "MusicVolume";
    private const string SFX_KEY = "SfxVolume";

    protected override void Awake()
    {
        base.Awake();
        LoadVolume();
        ApplyVolume();
    }

    #region Public API
    public void SetMasterVolume(float value)
    {
        masterVolume = Mathf.Clamp01(value);
        ApplyVolume();
        SaveVolume();
    }

    public void SetMusicVolume(float value)
    {
        musicVolume = Mathf.Clamp01(value);
        ApplyVolume();
        SaveVolume();
    }

    public void SetSfxVolume(float value)
    {
        sfxVolume = Mathf.Clamp01(value);
        ApplyVolume();
        SaveVolume();
    }

    public void PlaySFX(string clipName)
    {
        AudioClip clip = null;
        foreach(var audioInfo in sfxClips)
        {
            if (audioInfo.name == clipName)
            {
                clip = audioInfo.clip;
                break;
            }
        }
        if (clip != null)
        {
            float randomPitch = Random.Range(0.9f, 1.1f);
            sfxSource.pitch = randomPitch;
            sfxSource.PlayOneShot(clip, sfxVolume * masterVolume);
        }
        else
        {
            Debug.LogWarning($"SFX clip '{clipName}' not found in AudioManager list!");
        }
    }

    public void PlayMusic(string clipName, bool loop = true)
    {
        AudioClip clip = null;
        foreach (var audioInfo in musicClips)
        {
            if (audioInfo.name == clipName)
            {
                clip = audioInfo.clip;
                break;
            }
        }

        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
        ApplyVolume();
    }
    public void StopMusic()
    {
        if (musicSource.isPlaying)
            musicSource.Stop();
    }
    #endregion

    #region Volume Handling
    private void ApplyVolume()
    {
        if (musicSource != null)
            musicSource.volume = musicVolume * masterVolume;

        if (sfxSource != null)
            sfxSource.volume = sfxVolume * masterVolume;
    }

    private void SaveVolume()
    {
        PlayerPrefs.SetFloat(MASTER_KEY, masterVolume);
        PlayerPrefs.SetFloat(MUSIC_KEY, musicVolume);
        PlayerPrefs.SetFloat(SFX_KEY, sfxVolume);
        PlayerPrefs.Save();
    }

    private void LoadVolume()
    {
        masterVolume = PlayerPrefs.GetFloat(MASTER_KEY, 1f);
        musicVolume = PlayerPrefs.GetFloat(MUSIC_KEY, 1f);
        sfxVolume = PlayerPrefs.GetFloat(SFX_KEY, 1f);
    }
    #endregion
}
