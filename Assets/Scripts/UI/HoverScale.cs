using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Temporarily scales up UI elements on hover. Found on the internet and modified by me to stop masks from cutting off the scaled element.
/// </summary>
public class HoverScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Vector3 originalScale;
    public float scaleMultiplier = 1.1f; // Adjust this in the Inspector
    public float animationDuration = 0.1f; // Duration of the scale animation
    public float disableMaskDuring = 0.5f; // The percantage of the animation that plays before removing mask
    public bool disableMasking = true; // Whether to disable the parent mask during scaling
    private RectTransform rectTransform;

    private RectMask2D parentMask;
    private List<MaskableGraphic> maskableGraphics;

    void Awake()
    {
        originalScale = transform.localScale;
        rectTransform = GetComponent<RectTransform>();
        parentMask = GetComponentInParent<RectMask2D>();
        maskableGraphics = new List<MaskableGraphic>();
        CollectMaskableGraphics();
    }

    private void CollectMaskableGraphics()
    {
        maskableGraphics.Clear();
        MaskableGraphic[] allMaskableGraphics = GetComponentsInChildren<MaskableGraphic>(true);
        foreach (MaskableGraphic graphic in allMaskableGraphics)
        {
            maskableGraphics.Add(graphic);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(SmoothScale(originalScale * scaleMultiplier, animationDuration, disableMaskDuring: disableMaskDuring)); // Smoothly scale up on hover
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (disableMasking && parentMask != null && maskableGraphics.Count > 0)
        {
            // Restore the mask before scaling back down to prevent it from cutting off the object during the animation
            foreach (MaskableGraphic maskableGraphic in maskableGraphics)
            {
                parentMask.AddClippable(maskableGraphic);
            }
        }

        StopAllCoroutines();
        StartCoroutine(SmoothScale(originalScale, animationDuration)); // Smoothly return to original scale on exit and restore mask
    }

    // Coroutine for smooth scaling.
    private IEnumerator SmoothScale(Vector3 targetScale, float duration, float disableMaskDuring = -1f)
    {
        Vector3 initialScale = transform.localScale;
        float disableMask = duration * disableMaskDuring;
        float time = 0f;

        while (time < duration)
        {
            transform.localScale = Vector3.Lerp(initialScale, targetScale, time / duration);
            time += Time.deltaTime;

            if (disableMasking && disableMaskDuring != -1 && time >= disableMask && parentMask != null && maskableGraphics.Count > 0)
            {
                // Disable the mask after a short delay to allow the object to scale up without being cut off
                foreach (MaskableGraphic maskableGraphic in maskableGraphics)
                {
                    parentMask.RemoveClippable(maskableGraphic);
                }
            }
            yield return null;
        }
        transform.localScale = targetScale; // Ensure it ends at the exact target scale
    }
}