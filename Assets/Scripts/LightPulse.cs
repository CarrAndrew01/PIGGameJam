using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

[RequireComponent(typeof(Light2D))]
public class LightPulse : MonoBehaviour
{
    public bool useIntensity = true;
    public bool useColor = true;
    public bool useRandomInterval = true;

    public float intensityA = 1f;
    public float intensityB = 2f;

    public Color colorA = Color.white;
    public Color colorB = Color.red;

    public float pulseDuration = 0.5f;

    public float intervalA = 1f;
    public float intervalB = 2f;

    Light2D light2D;

    void Awake()
    {
        light2D = GetComponent<Light2D>();
        StartCoroutine(PulseRoutine());
    }

    IEnumerator PulseRoutine()
    {
        while (true)
        {
            float t = 0f;

            float startIntensity = useIntensity ? intensityA : light2D.intensity;
            float targetIntensity = useIntensity ? intensityB : light2D.intensity;

            Color startColor = useColor ? colorA : light2D.color;
            Color targetColor = useColor ? colorB : light2D.color;

            while (t < pulseDuration)
            {
                t += Time.deltaTime;
                float lerp = t / pulseDuration;

                if (useIntensity)
                    light2D.intensity = Mathf.Lerp(startIntensity, targetIntensity, lerp);

                if (useColor)
                    light2D.color = Color.Lerp(startColor, targetColor, lerp);

                yield return null;
            }

            t = 0f;

            while (t < pulseDuration)
            {
                t += Time.deltaTime;
                float lerp = t / pulseDuration;

                if (useIntensity)
                    light2D.intensity = Mathf.Lerp(targetIntensity, startIntensity, lerp);

                if (useColor)
                    light2D.color = Color.Lerp(targetColor, startColor, lerp);

                yield return null;
            }

            float waitTime = useRandomInterval ? Random.Range(intervalA, intervalB) : intervalA;
            yield return new WaitForSeconds(waitTime);
        }
    }
}