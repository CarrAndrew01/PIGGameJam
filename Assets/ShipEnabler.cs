using UnityEngine;


// This script controls whether the ship can be controlled or moved
public class ShipEnabler : MonoBehaviour
{
    [SerializeField]
    ShipMovement shipMovementScript;
    [SerializeField]
    Fishing fishingScript;
    [SerializeField]
    Animator shipAnimator;
    [SerializeField]
    bool enabledAtStart = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Automatically disables player controls while entrance animation is playing
        ToggleControls(enabledAtStart);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            shipAnimator.SetTrigger("ExitScene");
        }
    }
    // disables all player control and activates animator
    public void ToggleControls(bool enabled)
    {
        if (shipMovementScript == null || fishingScript == null || shipAnimator == null) return;
        shipMovementScript.enabled = enabled;
        fishingScript.enabled = enabled;
    }
    // allows the player to move when the landing animation is finished
    void AnimationComplete()
    {
        ToggleControls(true);

    }
    void LeaveStart()
    {
        ToggleControls(false);
    }
}
