using UnityEngine;

public class Parallax : MonoBehaviour
{

    private float startpos;
    private GameObject cam;
    public float parllax;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startpos = transform.position.x;
        cam = Camera.main.gameObject;

        // if parallax is 1, then the background should just move with the camera so I'm parenting it and disabling this script
        if (parllax >= 1)
        {
            transform.parent = cam.transform;
            enabled = false;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (parllax <= 0 || parllax >= 1)
            return;

        float temp = (cam.transform.position.x * (1 - parllax));
        float distance = (cam.transform.position.x * parllax);

        transform.position = new Vector3(startpos + distance, transform.position.y, transform.position.z);
    }
}
