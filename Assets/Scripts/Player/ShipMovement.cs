using System;
using System.Collections;
using Sirenix.Utilities;
using Unity.Mathematics;
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

    public int LastDirection { get; private set; } = 1; // Either 1 (right) or -1 (left)
    private Vector3 originalScale; // Original scale of the ship for flipping
    private Coroutine spinCoroutine; // Reference to the current spin coroutine

    // Variables
    [Header("Movement Settings")]
    public float acceleration = 5f;
    public float deceleration = 5f; // When no input
    public float maxVelocity = 10f;
    [Range(0f, 1f)]
    public float bounceMultiplier = 0.5f; // How much velocity is kept when bouncing
    public float spinDuration = 0.5f; // Duration of the spin animation when turning around
    public float speedAffectSpinFactor = 0.02f; // (higher means faster spins at higher speeds)

    public bool CanMove => !MenuManager.IsAnyMenuOpen && (!Fishing.IsMinigameActive || Fishing.Instance.IsCharging) && (!Fishing.IsFishing || Fishing.IsReelingIn);

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

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void OnEnable()
    {
        AudioManager.playSound?.Invoke("Ship_Hover");
    }
    private void OnDisable()
    {
        AudioManager.stopSound?.Invoke("Ship_Hover");
    }

    void Start()
    {
        originalScale = spriteTransform.localScale;
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
        if (CanMove == false)
        {
            // In menus, input becomes zero so the ship slows to a stop
            inputDirection = Vector2.zero;
            return;
        }

        inputDirection = moveAction.action.ReadValue<Vector2>();

        // If input direction changes from left to right or vice versa, start the spin animation
        if (inputDirection.x > 0 && LastDirection == -1)
            BeginSpin(1);
        else if (inputDirection.x < 0 && LastDirection == 1)
            BeginSpin(-1);



    }

    private void HandleMovement()
    {
        // Accelerate based on input
        if (inputDirection.x != 0)
        {
            currentVelocity += inputDirection.x * acceleration * Time.fixedDeltaTime;
        }
        // If no input, decelerate towards 0
        else
        {
            currentVelocity = Mathf.MoveTowards(currentVelocity, 0, deceleration * Time.fixedDeltaTime);
        }


        // Clamp velocity to max speed
        currentVelocity = Mathf.Clamp(currentVelocity, -maxVelocity, maxVelocity);
        #region yMovementStuffIgnore
        /*
        float scaleAdjuster = 0.005f;
        float yMovement = 0.03f;

        float scaleAim = 0.78f;


        //just testing out some Y axis movement into the background, dont know if we'll like that though
        if(inputDirection.y > 0 && spriteTransform.localScale.y > scaleAim)
        {
            //move player up while scaling the sprite down slightly
            spriteTransform.localScale = new Vector3(Mathf.MoveTowards(spriteTransform.localScale.x, scaleAim, scaleAdjuster), 
            Mathf.MoveTowards(spriteTransform.localScale.y, scaleAim, scaleAdjuster), originalScale.z); // Scale down slightly

            transform.position += new Vector3(0, yMovement, 0); // Move up slightlyq

            
        }else if(inputDirection.y < 0 && spriteTransform.localScale.y < 1f)
        {
            //cant work out a better way to do this rn

            //move player down while scaling the sprite up slightly
            spriteTransform.localScale = new Vector3(Mathf.MoveTowards(spriteTransform.localScale.x, 
            spriteTransform.localScale.x > 0 ? 1f : -1f, 
            scaleAdjuster), 
            Mathf.MoveTowards(spriteTransform.localScale.y, 1f, scaleAdjuster), originalScale.z); // Scale down slightly

            transform.position -= new Vector3(0, yMovement, 0); // Move down slightly
        }
        */
        #endregion
    }

    private void ApplyVelocity()
    {
        // Let the physics system move the ship horizontally — collisions are handled automatically
        rb.linearVelocity = new Vector2(currentVelocity, 0f);
    }

    private void ApplyBobbing()
    {

        //TODO: adjust bobbing height by a % of the scale so it looks right

        // Bobbing effect on the child so it doesn't interfere with physics
        // float bobbingY = Mathf.Sin(Time.time * bobbingFrequency) * bobbingAmplitude;

        // // Scale shadow based on bobbing
        // float shadowScale = Mathf.Lerp(shadowMax, shadowMin, (bobbingY + bobbingAmplitude) / (2 * bobbingAmplitude));
        // float shipScaleX = spriteTransform.localScale.x / originalScale.x; // Get the current horizontal scale factor (1 or -1)
        // spriteShadow.localScale = new Vector3(shadowScale * shipScaleX, .5f, 1f);

        // spriteTransform.localPosition = new Vector3(0f, bobbingY, 0f);
    }

    private void BeginSpin(int direction)
    {
        AudioManager.playSound?.Invoke("Ship_Spin");
        LastDirection = direction;
        if (spinCoroutine != null) StopCoroutine(spinCoroutine);
        spinCoroutine = StartCoroutine(SpinShip());
    }

    // Coroutine to spin the ship around by scaling the x-axis to either -1 or 1
    public IEnumerator SpinShip()
    {
        float affectedSpinDuration = spinDuration - (Mathf.Abs(currentVelocity) * speedAffectSpinFactor);
        affectedSpinDuration = Mathf.Max(0f, affectedSpinDuration); // Ensure spin duration is not negative

        float elapsed = 0f;
        float startScaleX = spriteTransform.localScale.x;
        
        float targetScaleX = math.abs(spriteTransform.localScale.x) * LastDirection; // Flip the scale to spin
 
        //just a conveniance variable
        //true if we're facing right, false if we're facing left
        bool directionFacingOnTurn = spriteTransform.localScale.x > 0;

        //ANDREW: I've changed this stuff so its possible to move up and down with the ship getting smaller. Just an experiment/idea but it works as normal 
        //now as the input stuff for y axis is commented out
        //so anything with the y scale can basically be ignored here
        while (elapsed < affectedSpinDuration)
        {
            elapsed += Time.deltaTime;

            float newScaleX = Mathf.Lerp(startScaleX, targetScaleX, elapsed / affectedSpinDuration);
            spriteTransform.localScale = new Vector3(newScaleX, spriteTransform.localScale.y, spriteTransform.localScale.z);

            if (directionFacingOnTurn)
            {
                //we are turning left because we're already facing right
                if(spriteTransform.localScale.x <= -spriteTransform.localScale.y)
                {
                    spriteTransform.localScale = new Vector3(-spriteTransform.localScale.y, spriteTransform.localScale.y, 1);
                    yield break;
                }
            }
            else
            {
                //we are turning right because we're already facing left
                if(spriteTransform.localScale.x >= spriteTransform.localScale.y)
                {
                    spriteTransform.localScale = new Vector3(spriteTransform.localScale.y, spriteTransform.localScale.y, 1);
                    yield break;
                }                
            }

            yield return null;
        }

        float xScale = directionFacingOnTurn ? -spriteTransform.localScale.y : spriteTransform.localScale.y;

        // Ensure final scale is set
        spriteTransform.localScale = new Vector3(xScale, 
        spriteTransform.localScale.y, spriteTransform.localScale.z);
    }
}
