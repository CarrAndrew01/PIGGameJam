using UnityEngine;
using UnityEngine.Serialization;

public class Parallax : MonoBehaviour
{

    private float startx;
    private GameObject cam;
    [FormerlySerializedAs("parllax")]
    public float parallax;

    void Awake()
    {
        startx = transform.position.x;
    }

    void Start()
    {
        cam = Camera.main.gameObject;

        // if parallax is 1, then the background should just move with the camera so I'm parenting it and disabling this script
        if (parallax >= 1)
        {
            transform.parent = cam.transform;
            enabled = false;
        }

        Resynchronize();
    }

    // Update after camera has moved so parallax stays in sync with rendering
    void LateUpdate()
    {
        if (parallax <= 0 || parallax >= 1)
            return;

        if (cam == null) return;

        float camX = cam.transform.position.x;
        float distance = camX * parallax;
        float relativePos = distance - camX + startx;
        transform.position = new Vector3(relativePos + camX, transform.position.y, transform.position.z);
    }

    // Don't ask me why this works 😂
    private void Resynchronize()
    {
        this.enabled = false;
        this.enabled = true;
    }
}
