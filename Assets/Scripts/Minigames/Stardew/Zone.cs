using System;
using UnityEngine;
using UnityEngine.Events;

public class Zone : MonoBehaviour
{
    public enum Activator
    {
        Hook,
        Fish
    }

    [Header("Settings")]
    public bool canHookActivate;
    public bool canFishActivate;
    public bool debug;

    [Header("Components")]
    public RectTransform hookRect;
    public RectTransform fishRect;
    private RectTransform rectTrans;

    [Header("Events")]
    public UnityEvent<Activator> OnZoneEnter;
    public UnityEvent<Activator> OnZoneStay;
    public UnityEvent<Activator> OnZoneExit;

    // State
    private bool isHookInside;
    private bool isFishInside;

    public void Init(RectTransform hook, RectTransform fish)
    {
        hookRect = hook;
        fishRect = fish;
    }

    void Awake()
    {
        rectTrans = GetComponent<RectTransform>();

        if (!rectTrans)
        {
            Debug.LogWarning("Zone is not attached to a UI item with a RectTransform.");
        }
    }

    void Update()
    {
        if (!rectTrans) return;

        isHookInside = CheckForRect(Activator.Hook, hookRect, isHookInside, !canHookActivate);
        isFishInside = CheckForRect(Activator.Fish, fishRect, isFishInside, !canFishActivate);
    }

    // Methods
    private bool CheckForRect(Activator type, RectTransform rect, bool wasInside, bool ignore = false)
    {
        if (ignore) return false;

        bool inside = rectTrans.RectOverlaps(rect);

        if (inside)
        {
            if (wasInside)
            {
                OnZoneStay.Invoke(type);
                DebugMessage($"{type} stayed in zone.");
            }
            else
            {
                OnZoneEnter.Invoke(type);
                DebugMessage($"{type} entered zone.");
            }
            return true;
        }
        else
        {
            if (wasInside)
            {
                OnZoneExit.Invoke(type);
                DebugMessage($"{type} exited zone.");
            }
            return false;
        }
    }

    private void DebugMessage(string msg)
    {
        if (debug)
            Debug.Log(msg);
    }
}
