using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Attach to a Slider to give visual feedback on its handle when the slider is
/// selected by controller/keyboard navigation or SetSelectedGameObject.
/// Supports scale and/or color changes.
/// </summary>
[RequireComponent(typeof(Slider))]
public class SliderHandleSelectFeedback : MonoBehaviour
{
    [Header("Scale Feedback")]
    public bool useScaleFeedback = true;
    public Vector3 selectedScale = new Vector3(1.3f, 1.3f, 1.3f);

    private Vector3 normalScale = Vector3.one;

    [Header("Color Feedback")]
    public bool useColorFeedback = true;
    public Color selectedColor = new Color(1f, 0.85f, 0.2f);

    private Color normalColor = Color.white;

    [Header("Components")]
    private Image handleImage;
    private Transform handleTransform;

    private bool wasSelected = false;

    void Awake()
    {
        Slider slider = GetComponent<Slider>();
        if (slider.handleRect != null)
            handleImage = slider.handleRect.GetComponent<Image>();

        if (handleImage != null)
        {
            handleTransform = handleImage.transform;
            normalColor = handleImage.color;
            normalScale = handleTransform.localScale;

            if (useScaleFeedback)
                selectedScale = Vector3.Scale(normalScale, selectedScale);
        }
    }

    void Update()
    {
        bool isSelected = EventSystem.current != null &&
                          EventSystem.current.currentSelectedGameObject == gameObject;

        if (isSelected == wasSelected) return;
        wasSelected = isSelected;

        if (isSelected)
            ApplySelected();
        else
            ApplyNormal();
    }

    private void ApplySelected()
    {
        if (useScaleFeedback && handleTransform != null)
            handleTransform.localScale = selectedScale;

        if (useColorFeedback && handleImage != null)
            handleImage.color = selectedColor;
    }

    private void ApplyNormal()
    {
        if (useScaleFeedback && handleTransform != null)
            handleTransform.localScale = normalScale;

        if (useColorFeedback && handleImage != null)
            handleImage.color = normalColor;
    }
}
