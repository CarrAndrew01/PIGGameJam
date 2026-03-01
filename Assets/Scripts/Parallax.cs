using UnityEngine;
using UnityEngine.Serialization;

public class Parallax : MonoBehaviour
{

    private float startpos;
    private GameObject cam;
    [FormerlySerializedAs("parllax")]
    public float parallax;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startpos = transform.position.x;
        cam = Camera.main.gameObject;

        // if parallax is 1, then the background should just move with the camera so I'm parenting it and disabling this script
        if (parallax >= 1)
        {
            transform.parent = cam.transform;
            enabled = false;
        }

        // Test low fps
        // QualitySettings.vSyncCount = 0;
        // Application.targetFrameRate = 60;
    }

    // Update after camera has moved so parallax stays in sync with rendering
    void LateUpdate()
    {
        if (parallax <= 0 || parallax >= 1)
            return;

        if (cam == null)
            cam = Camera.main != null ? Camera.main.gameObject : null;
        if (cam == null) return;

        float camX = cam.transform.position.x;
        float distance = camX * parallax;
        float relativePos = distance - camX + startpos;
        transform.position = new Vector3(relativePos + camX, transform.position.y, transform.position.z);
    }
}
