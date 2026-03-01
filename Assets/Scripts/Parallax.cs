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
    }

    // Update after camera has moved so parallax stays in sync with rendering
    void LateUpdate()
    {
        if (parallax <= 0 || parallax >= 1)
            return;

        if (cam == null)
            cam = Camera.main != null ? Camera.main.gameObject : null;
        if (cam == null) return;

        float distance = (cam.transform.position.x * parallax);
        transform.position = new Vector3(startpos + distance, transform.position.y, transform.position.z);
    }
}
