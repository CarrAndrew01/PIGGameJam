using UnityEngine;
using UnityEngine.Audio;
using System;
public class AudioManager : MonoBehaviour
{
    [Header("Sound Clips")]
    public Sound[] sounds;

    [Header("Audio Mixers")]
    public AudioMixerGroup musicMixerGroup;
    public AudioMixerGroup sfxMixerGroup;

    public float effectiveMuteThreshold = -40f; // Threshold in decibels below which sound is considered effectively muted

    public const float DECIBEL_MIN = -80f; // Minimum decibel level for silence
    public const float DECIBEL_MAX = 0f;   // Maximum decibel level for full volume

    public const string MASTER_VOLUME_PARAM = "masterVol";
    public const string MUSIC_VOLUME_PARAM = "musicVol";
    public const string SFX_VOLUME_PARAM = "sfxVol";
    public const string AMBIENT_VOLUME_PARAM = "ambientVol";

    public delegate void PlaySound(string soundName);
    public static PlaySound playSound;
    public delegate void StopSound(string soundName);
    public static StopSound stopSound;

    public static AudioManager instance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.outputAudioMixerGroup = s.audioMixer;
        }

        // Load saved audio settings from PlayerPrefs
        float musicVolume, sfxVolume;
        if (PlayerPrefs.HasKey("Settings_MusicVolume"))
        {
            musicVolume = PlayerPrefs.GetFloat("Settings_MusicVolume");
            musicMixerGroup.audioMixer.SetFloat(MUSIC_VOLUME_PARAM, musicVolume);
        }
        if (PlayerPrefs.HasKey("Settings_SFXVolume"))
        {
            sfxVolume = PlayerPrefs.GetFloat("Settings_SFXVolume");
            sfxMixerGroup.audioMixer.SetFloat(SFX_VOLUME_PARAM, sfxVolume);
        }
    }

    private void OnEnable()
    {
        playSound += Play;
        stopSound += Stop;
    }
    private void OnDisable()
    {
        playSound -= Play;
        stopSound -= Stop;
    }

    // Mixer groups
    public void SetMusicVolume(float volume, bool effectivelyMute = true)
    {
        // Convert linear slider value (0.0 to 1.0) to decibel range
        float dBVolume;
        if (!effectivelyMute)
            dBVolume = Mathf.Lerp(DECIBEL_MIN, DECIBEL_MAX, volume);
        else
        {
            // If effectively muting, set volume to the effective mute threshold
            dBVolume = Mathf.Lerp(effectiveMuteThreshold, DECIBEL_MAX, volume);
            if (dBVolume <= effectiveMuteThreshold)
                dBVolume = DECIBEL_MIN; // Set to absolute minimum if below effective mute threshold
        }
        musicMixerGroup.audioMixer.SetFloat(MUSIC_VOLUME_PARAM, dBVolume);
    }

    public float GetMusicVolume(bool effectivelyMute = true)
    {
        musicMixerGroup.audioMixer.GetFloat(MUSIC_VOLUME_PARAM, out float volume);
        // Convert decibel value back to linear range (0.0 to 1.0)
        float volumeLinear;
        if (!effectivelyMute) volumeLinear = Mathf.InverseLerp(DECIBEL_MIN, DECIBEL_MAX, volume);
        else
        {
            if (volume <= effectiveMuteThreshold)
                volumeLinear = 0f; // Consider effectively muted if below threshold
            else
                volumeLinear = Mathf.InverseLerp(effectiveMuteThreshold, DECIBEL_MAX, volume);
        }
        return volumeLinear;
    }

    public void SetSFXVolume(float volume, bool effectivelyMute = true)
    {
        // Convert linear slider value (0.0 to 1.0) to decibel range
        float dBVolume;
        if (!effectivelyMute)
            dBVolume = Mathf.Lerp(DECIBEL_MIN, DECIBEL_MAX, volume);
        else
        {
            // If effectively muting, set volume to the effective mute threshold
            dBVolume = Mathf.Lerp(effectiveMuteThreshold, DECIBEL_MAX, volume);
            if (dBVolume <= effectiveMuteThreshold)
                dBVolume = DECIBEL_MIN; // Set to absolute minimum if below effective mute threshold
        }
        sfxMixerGroup.audioMixer.SetFloat(SFX_VOLUME_PARAM, dBVolume);
    }

    public float GetSFXVolume(bool effectivelyMute = true)
    {
        sfxMixerGroup.audioMixer.GetFloat(SFX_VOLUME_PARAM, out float volume);
        // Convert decibel value back to linear range (0.0 to 1.0)
        float volumeLinear;
        if (!effectivelyMute) volumeLinear = Mathf.InverseLerp(DECIBEL_MIN, DECIBEL_MAX, volume);
        else
        {
            if (volume <= effectiveMuteThreshold)
                volumeLinear = 0f; // Consider effectively muted if below threshold
            else
                volumeLinear = Mathf.InverseLerp(effectiveMuteThreshold, DECIBEL_MAX, volume);
        }
        return volumeLinear;
    }

    public void Play(string name)
    {

        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found.");
            return;
        }
        s.source.Play();
    }
    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found.");
            return;
        }
        s.source.Stop();
    }
}
