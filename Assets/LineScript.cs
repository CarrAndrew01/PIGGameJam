using Unity.VisualScripting;
using UnityEngine;

public class LineScript : MonoBehaviour
{
    public AnimationCurve curve;
    public Transform lureStartingPos;
    public Rigidbody2D rb;

    public float time = 0;
    public float maxTime = 3;

    float xSpeed = 5f;
    float ySpeed = 3f;

    bool droppingLure = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (lureStartingPos != null)
        {
            transform.position = lureStartingPos.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (droppingLure)
        {
            transform.position = new Vector3(transform.position.x, curve.Evaluate(time / maxTime) * 10, transform.position.z);
            time += Time.deltaTime;
            if (time > maxTime)
            {
                droppingLure = false;

            }
        }
        else
        {


            if (Input.GetKey(KeyCode.Space))
            {
                // need to make velocity
                rb.AddForce(new Vector2(xSpeed, ySpeed));
                if (rb.linearVelocityY > 5)
                {
                    rb.linearVelocityY = 5;
                }
                if (rb.linearVelocityX > 5)
                {
                    rb.linearVelocityX = 5;
                }
                // transform.position = new Vector3(transform.position.x + (xSpeed * Time.deltaTime), transform.position.y + (ySpeed * Time.deltaTime), transform.position.z);
            }
            else
            {
                rb.linearVelocityX = 1f;
                rb.linearVelocityY = 0.2f;
            }
        }


        //         followObject.transform.position = Vector3.Lerp(initialPosition, endPosition, time);
        // float offset = curve.Evaluate(time / lerpDuration);
        // followObject.transform.position += offset * followObject.transform.right;
    }
}
