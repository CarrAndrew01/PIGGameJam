using UnityEngine;
using UnityEngine.InputSystem;


/// <summary>
/// Allows the player to move the ship left and right on the water.
/// </summary>
public class ShipMovement : MonoBehaviour
{
    // State
    public float currentVelocity = 0f; // Current speed of the ship, positive is right, negative is left
    private Vector2 inputDirection; // Direction of input, x is left/right, y is unused

    // Variables
    [Header("Movement Settings")]
    public float acceleration = 5f;
    public float deceleration = 5f; // When no input
    public float maxVelocity = 10f;
    [Range(0f, 1f)]
    public float bounceMultiplier = 0.5f; // How much velocity is kept when bouncing

    [Header("Bobbing Settings")]
    public float bobbingAmplitude = 0.1f;
    public float bobbingFrequency = 2f;

    [Header("Input Actions")]
    public InputActionReference moveAction; // expects Vector2, only the x component is used for left/right movement

    [Header("Components")]
    public Transform spriteTransform; // Child object that bobs up and down

    [Header("Shadow")]
    public Transform spriteShadow; // Shadow
    public float shadowMin, shadowMax; // Min and max scale for the shadow based on bobbing

    // Components
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();

        ApplyBobbing();
    }

    void FixedUpdate()
    {
        HandleMovement();
        ApplyVelocity();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Get the average contact normal
        Vector2 normal = collision.GetContact(0).normal;

        // Only bounce if we're moving into the surface (velocity opposes the normal)
        // normal.x > 0 means wall is to our left, normal.x < 0 means wall is to our right
        bool movingIntoWall = (currentVelocity > 0 && normal.x < 0) || (currentVelocity < 0 && normal.x > 0);

        if (movingIntoWall)
        {
            currentVelocity = -currentVelocity * bounceMultiplier;
        }
    }

    // Methods
    private void GetInput()
    {
        inputDirection = moveAction.action.ReadValue<Vector2>();
    }

    private void HandleMovement()
    {
        // Accelerate based on input
        if (inputDirection.x != 0)
            currentVelocity += inputDirection.x * acceleration * Time.fixedDeltaTime;
        // If no input, decelerate towards 0
        else
            currentVelocity = Mathf.MoveTowards(currentVelocity, 0, deceleration * Time.fixedDeltaTime);

        // Clamp velocity to max speed
        currentVelocity = Mathf.Clamp(currentVelocity, -maxVelocity, maxVelocity);
    }

    private void ApplyVelocity()
    {
        // Let the physics system move the ship horizontally — collisions are handled automatically
        rb.linearVelocity = new Vector2(currentVelocity, 0f);
    }

    private void ApplyBobbing()
    {
        // Bobbing effect on the child so it doesn't interfere with physics
        float bobbingY = Mathf.Sin(Time.time * bobbingFrequency) * bobbingAmplitude;

        // Scale shadow based on bobbing
        float shadowScale = Mathf.Lerp(shadowMax, shadowMin, (bobbingY + bobbingAmplitude) / (2 * bobbingAmplitude));
        spriteShadow.localScale = new Vector3(shadowScale, .5f, 1f);

        spriteTransform.localPosition = new Vector3(0f, bobbingY, 0f);
    }
}
