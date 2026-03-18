using UnityEngine;
using UnityEngine.UI;

public class TokenScript : MonoBehaviour
{
    public float force = 50000f;
    Rigidbody2D rb;
    Image image;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        image = GetComponent<Image>();

        // grabs a random value inside the small circle
        float tokenRotation = Random.Range(-30, 30);
        // rotates the token so that when force is applied, it will fly in a random direction
        transform.Rotate(0, 0, tokenRotation);
        // fires the token out in the direction the token is facing. a very high value but it uses high linear damping to slow it
        rb.AddRelativeForce(transform.up * force);

        image.color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1);

        AudioManager.playSound?.Invoke("Poker_Chip");
    }
    private void OnDestroy()
    {
        AudioManager.playSound?.Invoke("Poker_Chip_Remove");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
