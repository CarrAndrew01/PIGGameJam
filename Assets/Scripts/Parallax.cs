using UnityEngine;

public class Parallax : MonoBehaviour
{

    private float length, startpos;
    public GameObject cam;
    public float parllax;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startpos = transform.position.x;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float temp = (cam.transform.position.x * (1 - parllax));
        float distance = (cam.transform.position.x * parllax);

        transform.position = new Vector3(startpos + distance, transform.position.y, transform.position.z);


    }
}
