using UnityEngine;

public class PlayerCamera : MonoBehaviour
{

    [Header("Target")]
    public Transform target;

    [Header("Offset")]
    public float offsetX = 0f;

    [Header("Deadzone")]
    public bool useDeadzone = true;
    public float deadzoneHalfWidth = 1f;

    private float _currentX;
    private float _anchorX;


    private void Start()
    {
        if (target == null)
        {
            // Grab player by tag if not set in inspector
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }

        _anchorX = target.position.x + offsetX;
        _currentX = _anchorX;
        SetCameraX(_currentX);
    }

    private void LateUpdate()
    {
        if (target == null) return;

        float desiredX = target.position.x + offsetX;

        if (useDeadzone)
        {
            float delta = desiredX - _anchorX;
            if (Mathf.Abs(delta) > deadzoneHalfWidth)
            {
                // Shift anchor just enough to keep target at the deadzone edge.
                _anchorX = desiredX - Mathf.Sign(delta) * deadzoneHalfWidth;
            }
        }
        else
        {
            _anchorX = desiredX;
        }
       
            _currentX = _anchorX;
        

        SetCameraX(_currentX);
    }


    private void SetCameraX(float x)
    {
        Vector3 pos = transform.position;
        pos.x = x;
        transform.position = pos;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (!useDeadzone) return;

        float drawX = Application.isPlaying ? _anchorX : transform.position.x;

        Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.25f);
        Vector3 center = new Vector3(drawX, transform.position.y, transform.position.z);
        Vector3 size = new Vector3(deadzoneHalfWidth * 2f, 20f, 0.05f);
        Gizmos.DrawCube(center, size);

        Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.85f);
        Gizmos.DrawWireCube(center, size);
    }
#endif
}