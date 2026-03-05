using UnityEngine;

public class CamResolution : MonoBehaviour
{
    private Camera cam;
    private int lastScreenWidth;
    private int lastScreenHeight;

    public Vector2 targetResolution = new Vector2(1600, 1200); // Set your desired resolution here

    void Start()
    {
        cam = GetComponent<Camera>();
        ApplyResolution();
    }

    void OnEnable()
    {
        ResolutionUtils.OnResolutionChanged += ApplyResolution;
        ResolutionUtils.OnFullscreenToggled += HandleFullscreenToggled;
    }

    void OnDisable()
    {
        ResolutionUtils.OnResolutionChanged -= ApplyResolution;
        ResolutionUtils.OnFullscreenToggled -= HandleFullscreenToggled;
    }

    void ApplyResolution(Resolution newResolution = default)
    {
        lastScreenWidth = newResolution.width != 0 ? newResolution.width : Screen.width;
        lastScreenHeight = newResolution.height != 0 ? newResolution.height : Screen.height;

        float targetAspect = targetResolution.x / targetResolution.y;

        // Convert the target resolution to viewport coordinates
        float windowAspect = (float)Screen.width / (float)Screen.height;
        float scaleHeight = windowAspect / targetAspect;
        if (scaleHeight < 1.0f)
        {
            // Letterbox: bars on top and bottom
            cam.rect = new Rect(0, (1.0f - scaleHeight) / 2.0f, 1, scaleHeight);
        }
        else
        {
            // Pillarbox: bars on left and right
            float scaleWidth = 1.0f / scaleHeight;
            cam.rect = new Rect((1.0f - scaleWidth) / 2.0f, 0, scaleWidth, 1);
        }
    }

    void HandleFullscreenToggled(bool isFullscreen)
    {
        // If entering windowed mode, set the window size to the target resolution
        if (!isFullscreen)
        {
            Screen.SetResolution((int)targetResolution.x, (int)targetResolution.y, false);
        }
        ApplyResolution();
    }
}
