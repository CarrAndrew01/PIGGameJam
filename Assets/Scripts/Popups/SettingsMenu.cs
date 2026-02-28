using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [Header("Components")]
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;


    private void Start()
    {
        // Initialize sliders with current volume levels from AudioManager
        musicVolumeSlider.value = AudioManager.instance.GetMusicVolume();
        sfxVolumeSlider.value = AudioManager.instance.GetSFXVolume();
    }

    // Methods
    public void SetMusicVolume(float volume)
    {
        AudioManager.instance.SetMusicVolume(volume);
    }

    public void SetSFXVolume(float volume)
    {
        AudioManager.instance.SetSFXVolume(volume);
    }

    void OnDestroy()
    {
        // Save settings to PlayerPrefs when the menu is closed/destroyed
        float musicVolume = AudioManager.instance.GetMusicVolume();
        float sfxVolume = AudioManager.instance.GetSFXVolume();

        PlayerPrefs.SetFloat("Settings_MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("Settings_SFXVolume", sfxVolume);
    }

    // Methods
    public void CloseMenu()
    {
        // Close
        Menus.Instance.CloseCurrentMenu();
    }
}
