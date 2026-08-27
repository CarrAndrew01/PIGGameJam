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
    public bool visible = false;
    public float alphaSpeed = 0.1f;
    private Coroutine alphaControl;

    [Header("Temperature")]
    public Zone.Activator target;
    public float normalTemp = 0.5f;
    public float netGain = 0f;
    public float heatGain = 0.001f;
    public float heatLoss = 0.001f;
    public float stabilisation = 0.0005f;

    [Header("Components")]
    public Slider temperatureSlider;
    public CanvasGroup group;
    private Image temperatureFill;
    public List<Zone> coldZones;
    public List<Zone> hotZones;

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
        if (activator != target) return;

        netGain += heatGain;
    }

    void Cool(Zone.Activator activator)
    {
        if (activator != target) return;

        netGain -= heatLoss;
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
        if (netGain != 0)
        {
            temperatureSlider.value += netGain;

            // Appear
            if (!visible)
            {
                alphaControl = StartCoroutine(Appear());
            }
        }
        else
        {
            if (temperatureSlider.value > normalTemp)
                temperatureSlider.value -= stabilisation;
            else if (temperatureSlider.value < normalTemp)
                temperatureSlider.value += stabilisation;

            // Disappear
            if (visible)
            {
                if (temperatureSlider.value >= normalTemp - 0.1f && temperatureSlider.value <= normalTemp + 0.1f)
                {
                    alphaControl = StartCoroutine(Disappear());
                }
            }
        }
        netGain = 0;
    }

    public IEnumerator Disappear()
    {
        if (alphaControl != null)
            StopCoroutine(alphaControl);
        visible = false;

        while (group.alpha > 0)
        {
            group.alpha -= alphaSpeed;
            yield return null;
        }
    }

    public IEnumerator Appear()
    {
        if (alphaControl != null)
            StopCoroutine(alphaControl);
        visible = true;

        while (group.alpha < 1)
        {
            group.alpha += alphaSpeed;
            yield return null;
        }
    }
}
