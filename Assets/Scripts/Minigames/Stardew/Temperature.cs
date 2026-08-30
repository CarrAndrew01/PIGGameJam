using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Temperature : MonoBehaviour
{
    [Header("Colors")]
    public Color coldColor = Color.blue;
    public Color middleColor = Color.white;
    public Color hotColor = Color.red;
    public float alphaSpeed = 0.1f;
    public float iceAlphaSpeed = 0.1f;

    private bool visible = false;
    private Coroutine alphaControl;
    private Coroutine iceAlphaControl;

    [Header("Temperature")]
    public Zone.Activator target;
    public float normalTemp = 0.5f;
    public float heatGain = 0.001f;
    public float heatLoss = 0.001f;
    public float stabilisation = 0.0005f;
    public float heatRecovery = 0.001f;
    public float coldRecovery = 0.001f;

    private float netGain = 0f;
    private bool doesStabilise = true;
    private bool doZonesWork = true;
    private bool frozen = false;
    private bool burnt = false;

    [Header("Components")]
    public Stardew minigame;
    public Slider temperatureSlider;
    public CanvasGroup group;
    public CanvasGroup iceGroup;
    public List<Zone> coldZones;
    public List<Zone> hotZones;

    private Image temperatureFill;

    void Awake()
    {
        temperatureFill = temperatureSlider.fillRect.GetComponent<Image>();
    }

    void OnEnable()
    {
        foreach (Zone zone in coldZones)
        {
            zone.OnZoneStay.AddListener(Cool);
        }
        foreach (Zone zone in hotZones)
        {
            zone.OnZoneStay.AddListener(Heat);
        }
    }

    void OnDisable()
    {
        foreach (Zone zone in coldZones)
        {
            zone.OnZoneStay.RemoveListener(Cool);
        }
        foreach (Zone zone in hotZones)
        {
            zone.OnZoneStay.RemoveListener(Heat);
        }
    }

    void Heat(Zone.Activator activator)
    {
        if (activator != target || !doZonesWork) return;

        netGain += heatGain;
    }

    void Cool(Zone.Activator activator)
    {
        if (activator != target || !doZonesWork) return;

        netGain -= heatLoss;
    }

    void Freeze()
    {
        Debug.Log("Freezing hook.");
        if (minigame && !frozen)
        {
            frozen = true;
            doZonesWork = false;

            minigame.FreezeHook();

            if (iceGroup)
            {
                StartCoroutine(Fade(iceGroup, true, iceAlphaControl));
            }
        }
    }

    void Unfreeze()
    {
        Debug.Log("Unfreezing hook.");
        if (minigame && frozen)
        {
            frozen = false;
            doZonesWork = true;

            minigame.UnfreezeHook();

            if (iceGroup)
            {
                StartCoroutine(Fade(iceGroup, false, iceAlphaControl));
            }
        }
    }

    void Burn()
    {
        Debug.Log("Burning hook.");
        if (minigame && !burnt)
        {
            burnt = true;
            doZonesWork = false;
        }
    }

    void Unburn()
    {
        Debug.Log("Unburning hook.");
        if (minigame && burnt)
        {
            burnt = false;
            doZonesWork = true;
        }
    }

    void Update()
    {
        if (temperatureSlider.value > 0.5f)
        {
            float hotValue = temperatureSlider.value - 0.5f;
            temperatureFill.color = Color.Lerp(middleColor, hotColor, hotValue * 2);
        }
        else
        {
            float coldValue = temperatureSlider.value;
            temperatureFill.color = Color.Lerp(coldColor, middleColor, coldValue * 2);
        }

    }

    void LateUpdate()
    {
        // Handle frozen state
        if (frozen)
        {
            // Do QTE or something I don't know, for now just slowly unfreeze
            temperatureSlider.value += coldRecovery;

            if (temperatureSlider.value >= 0.5f)
                Unfreeze();
        }
        // Handle burnt state
        else if (burnt)
        {
            temperatureSlider.value -= heatRecovery;

            if (temperatureSlider.value <= 0.5f)
                Unburn();
        }
        // In zone state
        else if (netGain != 0)
        {
            temperatureSlider.value += netGain;

            // Check for freeze/burn
            if (temperatureSlider.value == 0f)
                Freeze();
            else if (temperatureSlider.value == 1f)
                Burn();

            // Appear
            if (!visible)
            {
                visible = true;
                alphaControl = StartCoroutine(Fade(group, true, alphaControl));
            }
        }
        // Normal state
        else
        {
            // Stabilisation
            if (doesStabilise)
            {
                if (temperatureSlider.value > normalTemp)
                    temperatureSlider.value -= stabilisation;
                else if (temperatureSlider.value < normalTemp)
                    temperatureSlider.value += stabilisation;
            }

            // Disappear
            if (visible)
            {
                if (temperatureSlider.value >= normalTemp - 0.1f && temperatureSlider.value <= normalTemp + 0.1f)
                {
                    visible = false;
                    alphaControl = StartCoroutine(Fade(group, false, alphaControl));
                }
            }
        }
        netGain = 0;
    }
    IEnumerator Fade(CanvasGroup canvasGroup, bool fadeIn = true, Coroutine control = null)
    {
        if (control != null)
            StopCoroutine(control);

        if (fadeIn)
        {
            while (canvasGroup.alpha < 1)
            {
                canvasGroup.alpha += alphaSpeed;
                yield return null;
            }
        }
        else
        {
            while (canvasGroup.alpha > 0)
            {
                canvasGroup.alpha -= alphaSpeed;
                yield return null;
            }
        }
    }
}
