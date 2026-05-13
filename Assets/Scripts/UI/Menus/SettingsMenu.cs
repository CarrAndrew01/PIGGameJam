using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class SettingsMenu : MonoBehaviour
{
    [Header("Variables")]
    public Color sliderStartColor = Color.white;
    public Color sliderEndColor = Color.white;

    [Header("Components")]
    // Sound
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public Slider ambientVolumeSlider;
    public Slider uiVolumeSlider;

    // Gameplay
    public Slider difficultySlider;


    // Components
    private Image masterSliderFill;
    private Image musicSliderFill;
    private Image sfxSliderFill;
    private Image ambientSliderFill;
    private Image uiSliderFill;
    private Image difficultySliderFill;

    // Cached fill values
    private float lastMasterFillValue = -1f;
    private float lastMusicFillValue = -1f;
    private float lastSfxFillValue = -1f;
    private float lastAmbientFillValue = -1f;
    private float lastUiFillValue = -1f;
    private float lastDifficultyFillValue = -1f;
    private void Awake()
    {
        // Get references to the fill images of each slider
        masterSliderFill = masterVolumeSlider.fillRect.GetComponent<Image>();
        musicSliderFill = musicVolumeSlider.fillRect.GetComponent<Image>();
        sfxSliderFill = sfxVolumeSlider.fillRect.GetComponent<Image>();
        ambientSliderFill = ambientVolumeSlider.fillRect.GetComponent<Image>();
        uiSliderFill = uiVolumeSlider.fillRect.GetComponent<Image>();
        difficultySliderFill = difficultySlider.fillRect.GetComponent<Image>();

        // Initialize slider colors
        UpdateSliderColor(masterVolumeSlider, masterSliderFill, ref lastMasterFillValue, overwrite: true);
        UpdateSliderColor(musicVolumeSlider, musicSliderFill, ref lastMusicFillValue, overwrite: true);
        UpdateSliderColor(sfxVolumeSlider, sfxSliderFill, ref lastSfxFillValue, overwrite: true);
        UpdateSliderColor(ambientVolumeSlider, ambientSliderFill, ref lastAmbientFillValue, overwrite: true);
        UpdateSliderColor(uiVolumeSlider, uiSliderFill, ref lastUiFillValue, overwrite: true);
        UpdateSliderColor(difficultySlider, difficultySliderFill, ref lastDifficultyFillValue, overwrite: true);
    }

    private void Start()
    {
        // Initialize sliders with current volume levels from AudioManager
        masterVolumeSlider.value = AudioManager.instance.GetMasterVolume();
        musicVolumeSlider.value = AudioManager.instance.GetMusicVolume();
        sfxVolumeSlider.value = AudioManager.instance.GetSFXVolume();
        ambientVolumeSlider.value = AudioManager.instance.GetAmbientVolume();
        uiVolumeSlider.value = AudioManager.instance.GetUIVolume();

        // Initialize gameplay sliders with current values from GameManager/PlayerInventory
        difficultySlider.value = GameManager.GetDifficultyModifierNormalized(difficultySlider);
    }

    void OnDestroy()
    {
        // Save settings to PlayerPrefs when the menu is closed/destroyed
        float masterVolume = AudioManager.instance.GetMasterVolume();
        float musicVolume = AudioManager.instance.GetMusicVolume();
        float sfxVolume = AudioManager.instance.GetSFXVolume();
        float ambientVolume = AudioManager.instance.GetAmbientVolume();
        float uiVolume = AudioManager.instance.GetUIVolume();

        PlayerPrefs.SetFloat("Settings_MasterVolume", masterVolume);
        PlayerPrefs.SetFloat("Settings_MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("Settings_SFXVolume", sfxVolume);
        PlayerPrefs.SetFloat("Settings_AmbientVolume", ambientVolume);
        PlayerPrefs.SetFloat("Settings_UIVolume", uiVolume);

        PlayerPrefs.Save();
    }

    void OnEnable()
    {
        //Debug player pref
        Debug.Log("Master Volume: " + PlayerPrefs.GetFloat("Settings_MasterVolume", -1f));
        Debug.Log("Music Volume: " + PlayerPrefs.GetFloat("Settings_MusicVolume", -1f));
        Debug.Log("SFX Volume: " + PlayerPrefs.GetFloat("Settings_SFXVolume", -1f));
        Debug.Log("Ambient Volume: " + PlayerPrefs.GetFloat("Settings_AmbientVolume", -1f));
        Debug.Log("UI Volume: " + PlayerPrefs.GetFloat("Settings_UIVolume", -1f));
        Debug.Log("Difficulty Modifier: " + PlayerPrefs.GetFloat("Settings_DifficultyModifier", -1f));
        SetupNavigation();
    }

    void Update()
    {
        // Update slider colors in real-time as the sliders are adjusted
        UpdateSliderColor(masterVolumeSlider, masterSliderFill, ref lastMasterFillValue);
        UpdateSliderColor(musicVolumeSlider, musicSliderFill, ref lastMusicFillValue);
        UpdateSliderColor(sfxVolumeSlider, sfxSliderFill, ref lastSfxFillValue);
        UpdateSliderColor(ambientVolumeSlider, ambientSliderFill, ref lastAmbientFillValue);
        UpdateSliderColor(uiVolumeSlider, uiSliderFill, ref lastUiFillValue);
        UpdateSliderColor(difficultySlider, difficultySliderFill, ref lastDifficultyFillValue);
    }

    // Methods
    public void ClearPrefs()
    {
        GameManager.ClearPlayerData();
    }
    public void SetMasterVolume(float volume)
    {
        AudioManager.instance.SetMasterVolume(volume);
    }
    public void SetMusicVolume(float volume)
    {
        AudioManager.instance.SetMusicVolume(volume);
    }
    public void SetSFXVolume(float volume)
    {
        AudioManager.instance.SetSFXVolume(volume);
    }
    public void SetAmbientVolume(float volume)
    {
        AudioManager.instance.SetAmbientVolume(volume);
    }
    public void SetUIVolume(float volume)
    {
        AudioManager.instance.SetUIVolume(volume);
    }
    public void SetDifficultyModifier(float modifier)
    {
        float normalizedModifier = Mathf.InverseLerp(difficultySlider.minValue, difficultySlider.maxValue, modifier);
        GameManager.SetDifficultyModifier(normalizedModifier);
    }
    public void CloseMenu()
    {
        MenuManager.Instance.CloseCurrentMenu();
    }

    private void UpdateSliderColor(Slider slider, Image fillImage, ref float lastFillValue, bool overwrite = false)
    {
        float fillValue = slider.value;
        if (overwrite || !Mathf.Approximately(fillValue, lastFillValue))
        {
            Color newColor = Color.Lerp(sliderStartColor, sliderEndColor, fillValue);
            fillImage.color = newColor;
            lastFillValue = fillValue;
        }
    }

    private void SetupNavigation()
    {
        if (Gamepad.current == null) return;

        // Set up navigation the first navigation element in the menu (e.g., masterVolumeSlider) to be selected by default
        var navigationObject = GetComponentInChildren<Selectable>();

        EventSystem.current.SetSelectedGameObject(navigationObject.gameObject);
    }
}
