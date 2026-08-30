using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BioLight : MonoBehaviour
{
    public Light2D globalLight;
    public Light2D bioLight;

    public void SetLighting(bool isActive)
    {
        if (globalLight != null)
            globalLight.enabled = !isActive;

        if (bioLight != null)
            bioLight.enabled = isActive;
    }
}
