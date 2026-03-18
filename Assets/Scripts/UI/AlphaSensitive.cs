using UnityEngine;
using UnityEngine.UI;

public class AlphaSensitiveButton : MonoBehaviour
{
    [SerializeField, Range(0.01f, 1f)]
    private float alphaThreshold = 0.1f;   // Adjust as needed (0.01 = very permissive, 1.0 = only fully opaque)

    private void Awake()
    {
        var image = GetComponent<Image>();
        if (image != null)
        {
            image.alphaHitTestMinimumThreshold = alphaThreshold;
        }
    }
}