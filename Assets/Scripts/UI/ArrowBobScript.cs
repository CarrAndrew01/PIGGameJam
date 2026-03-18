using UnityEngine;

public class ArrowBobScript : MonoBehaviour
{
    public float bobbingAmount = 10f; // Speed of the bobbing motion
    public float bobbingSpeed = 1f; // Amplitude of the bobbing motion
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
                startYValue + (Mathf.Sin(Time.time * bobbingSpeed) * bobbingAmount));
    }
}
