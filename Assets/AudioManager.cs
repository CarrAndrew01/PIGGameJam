using UnityEngine;
using UnityEngine.Audio;
using System;
public class AudioManager : MonoBehaviour
{
    [Header("Sound Clips")]
    public SoundArrays[] soundsArray;

    [Header("Audio Mixers")]
    public AudioMixer audioMixer;

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

    // Helper: convert linear (0..1) to decibel, honoring effective mute logic
    private float LinearToDecibel(float linear, bool effectivelyMute)
    {
        if (!effectivelyMute)
            return Mathf.Lerp(DECIBEL_MIN, DECIBEL_MAX, linear);

        float dB = Mathf.Lerp(effectiveMuteThreshold, DECIBEL_MAX, linear);
        if (dB <= effectiveMuteThreshold)
            dB = DECIBEL_MIN;
        return dB;
    }

    // Helper: convert decibel to linear (0..1), honoring effective mute logic
    private float DecibelToLinear(float dB, bool effectivelyMute)
    {
        if (!effectivelyMute)
            return Mathf.InverseLerp(DECIBEL_MIN, DECIBEL_MAX, dB);

        if (dB <= effectiveMuteThreshold)
            return 0f;

        return Mathf.InverseLerp(effectiveMuteThreshold, DECIBEL_MAX, dB);
    }

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

        foreach (SoundArrays array in soundsArray)
        {
            foreach (Sound s in array)
            {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;

                s.source.volume = s.volume;
                s.source.pitch = s.pitch;
                s.source.loop = s.loop;
                s.source.outputAudioMixerGroup = s.audioMixer;
            }
        }
    }

    void Start()
    {
        // Load saved audio settings from PlayerPrefs.
        // Accept either previously-saved decibel values or newer linear (0..1) values.
        if (PlayerPrefs.HasKey("Settings_MasterVolume"))
            SetMasterVolume(PlayerPrefs.GetFloat("Settings_MasterVolume"), effectivelyMute: true);

        if (PlayerPrefs.HasKey("Settings_MusicVolume"))
            SetMusicVolume(PlayerPrefs.GetFloat("Settings_MusicVolume"), effectivelyMute: true);

        if (PlayerPrefs.HasKey("Settings_SFXVolume"))
            SetSFXVolume(PlayerPrefs.GetFloat("Settings_SFXVolume"), effectivelyMute: true);

        if (PlayerPrefs.HasKey("Settings_AmbientVolume"))
            SetAmbientVolume(PlayerPrefs.GetFloat("Settings_AmbientVolume"), effectivelyMute: true);

        if (PlayerPrefs.HasKey("Settings_UIVolume"))
            SetUIVolume(PlayerPrefs.GetFloat("Settings_UIVolume"), effectivelyMute: true);
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
    public void SetMasterVolume(float volume, bool effectivelyMute = true)
    {
        audioMixer.SetFloat(MASTER_VOLUME_PARAM, LinearToDecibel(volume, effectivelyMute));
    }

    public float GetMasterVolume(bool effectivelyMute = true)
    {
        audioMixer.GetFloat(MASTER_VOLUME_PARAM, out float dB);
        return DecibelToLinear(dB, effectivelyMute);
    }

    public void SetMusicVolume(float volume, bool effectivelyMute = true)
    {
        audioMixer.SetFloat(MUSIC_VOLUME_PARAM, LinearToDecibel(volume, effectivelyMute));
    }

    public float GetMusicVolume(bool effectivelyMute = true)
    {
        audioMixer.GetFloat(MUSIC_VOLUME_PARAM, out float dB);
        return DecibelToLinear(dB, effectivelyMute);
    }

    public void SetSFXVolume(float volume, bool effectivelyMute = true)
    {
        audioMixer.SetFloat(SFX_VOLUME_PARAM, LinearToDecibel(volume, effectivelyMute));
    }

    public float GetSFXVolume(bool effectivelyMute = true)
    {
        audioMixer.GetFloat(SFX_VOLUME_PARAM, out float dB);
        return DecibelToLinear(dB, effectivelyMute);
    }

    public void SetAmbientVolume(float volume, bool effectivelyMute = true)
    {
        audioMixer.SetFloat(AMBIENT_VOLUME_PARAM, LinearToDecibel(volume, effectivelyMute));
    }

    public float GetAmbientVolume(bool effectivelyMute = true)
    {
        audioMixer.GetFloat(AMBIENT_VOLUME_PARAM, out float dB);
        return DecibelToLinear(dB, effectivelyMute);
    }

    public void SetUIVolume(float volume, bool effectivelyMute = true)
    {
        audioMixer.SetFloat("uiVol", LinearToDecibel(volume, effectivelyMute));
    }

    public float GetUIVolume(bool effectivelyMute = true)
    {
        audioMixer.GetFloat("uiVol", out float dB);
        return DecibelToLinear(dB, effectivelyMute);
    }

    public void Play(string name)
    {
        Sound s = FindSound(name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found.");
            return;
        }
        s.source.Play();
    }
    public void Stop(string name)
    {
        Sound s = FindSound(name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found.");
            return;
        }
        s.source.Stop();
    }
    public Sound FindSound(string name)
    {
        foreach (SoundArrays array in soundsArray)
        {
            foreach (Sound sound in array)
            {
                if (sound.name == name)
                {
                    return sound;
                }
            }
        }
        // if sound isnt found
        return null;
    }
}
