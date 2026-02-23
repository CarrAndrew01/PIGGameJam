using UnityEngine;

public class ArrowBobScript : MonoBehaviour
{
    float startYValue;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startYValue = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector2(transform.position.x,
                startYValue + (Mathf.Sin(Time.time) * 10));
    }
}
