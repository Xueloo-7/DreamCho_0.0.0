using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] bool randomSong;
    [SerializeField] SoundData soundData;
    [SerializeField] AudioSource musicSource, soundSource, fxSource;

    private Dictionary<string, Sound> musicSoundDict;
    private Dictionary<string, Sound> fxSoundDict;

    // 音量
    private static float mainV;
    private static float musicV;
    private static float soundV;
    private static float fxV;

    protected override void Awake()
    {
        base.Awake();

        // 初始化字典
        musicSoundDict = new Dictionary<string, Sound>();
        fxSoundDict = new Dictionary<string, Sound>();

        foreach (var sound in soundData.musicSounds)
        {
            if (!musicSoundDict.ContainsKey(sound.name))
            {
                musicSoundDict[sound.name] = sound;
            }
        }

        foreach (var sound in soundData.fxSounds)
        {
            if (!fxSoundDict.ContainsKey(sound.name))
            {
                fxSoundDict[sound.name] = sound;
            }
        }
    }


    private void Start()
    {
        //PlayMusic(null);
    }

    public static void StopMusic()
    {
        if (Instance != null) Instance.musicSource.Stop();
    }
    public static void PlayMusic(string musicName = null)
    {
        if (Instance != null) Instance._PlayMusic(musicName);
    }
    public static void PlayMusic(AudioClip clip)
    {
        if (Instance != null)
        {
            if (Instance.musicSource.clip != clip)
            {
                Instance.musicSource.clip = clip;
                Instance.musicSource.Play();
            }
        }
    }
    private void _PlayMusic(string musicName = null)
    {
        if (randomSong && musicName == null)
        {
            int r = UnityEngine.Random.Range(0, soundData.musicSounds.Length);
            UpdateMusic(soundData.musicSounds[r]);
        }
        else
        {
            if (musicSoundDict.TryGetValue(musicName, out Sound sound))
            {
                UpdateMusic(sound);
            }
            else
            {
                Debug.LogWarning($"Music '{musicName}' Not Found");
            }
        }
    }

    private void UpdateMusic(Sound sound)
    {
        if (musicSource.clip != sound.audioClip)
        {
            musicSource.clip = sound.audioClip;
            musicSource.volume = sound.volume;
            musicSource.Play();

            StartCoroutine(UpdateRandomMusic(sound.name));
        }
    }

    private IEnumerator UpdateRandomMusic(string musicName)
    {
        yield return new WaitForSeconds(musicSource.clip.length);

        if (randomSong)
        {
            _PlayMusic(null);
        }
        else
        {
            musicSource.clip = null;
            _PlayMusic(musicName);
        }
    }

    public static void PlayFx(AudioClip clip, float volume = 1)
    {
        if (Instance != null) Instance._PlayFx(clip, volume);
    }
    public static void PlayFx(string fxName)
    {
        if (Instance != null) Instance._PlayFx(fxName);
    }
    private void _PlayFx(AudioClip clip, float volume)
    {
        fxSource.PlayOneShot(clip, volume);
    }
    private void _PlayFx(string fxName)
    {
        if (fxSoundDict.TryGetValue(fxName, out Sound sound))
        {
            fxSource.PlayOneShot(sound.audioClip, sound.volume);
            fxSource.volume = sound.volume;
        }
        else
        {
            Debug.LogWarning($"FX '{fxName}' Not Found");
        }
    }

    private void ApplyVolume()
    {
        if (musicSource != null) musicSource.volume = musicV * mainV;
        if (soundSource != null) soundSource.volume = soundV * mainV;
        if (fxSource != null) fxSource.volume = fxV * mainV;
    }
}
