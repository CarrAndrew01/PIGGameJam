using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting;

/// <summary>
/// Class representing a single item in a list, such as an upgrade or inventory item.
/// </summary>
public class ListItem : MonoBehaviour
{
    // Components
    [Header("Data")]
    public string description;
    public string mechanicalDescription;
    public int listIndex; // Index of this item in the list, set by the Menu when creating the item

    public bool IsStamped { get; private set; } = false;

    [Header("Stamp Settings")]
    public float stampedGrowDuration = 0.4f;
    public float stampedShakeDuration = 0.2f;
    public float stampedShakeMagnitude = 10f;
    public float stampedShakeRotation = 10f;

    [Header("Components")]
    public TextMeshProUGUI nameField;
    public TextMeshProUGUI subtextField;
    public TextMeshProUGUI subtextField2;
    public Image icon;
    public Image selectHightlight;
    public Image stampIcon;
    private Menu parentMenuBase;

    public void Init(Menu parent, string name, Sprite iconSprite = null, string subtext = "", string subtext2 = "", string description = "", string mechanicalDescription = "", int index = -1)
    {
        parentMenuBase = parent;
        nameField.text = name;
        listIndex = index;
        this.description = description;
        this.mechanicalDescription = mechanicalDescription;
        SetupComponents(iconSprite, subtext, subtext2);
    }

    public void SetupComponents(Sprite iconSprite = null, string subtext = "", string subtext2 = "")
    {
        // Subtext
        subtextField.text = subtext;
        subtextField2.text = subtext2;

        // Set icon visibility based on whether an icon was provided
        icon.sprite = iconSprite;
        icon.transform.parent.gameObject.SetActive(iconSprite != null);

        // Set stamp visibility based on whether the item is stamped
        stampIcon.gameObject.SetActive(IsStamped);

        // Set the subtexts visibility based on whether they were provided
        subtextField.gameObject.SetActive(!string.IsNullOrEmpty(subtext));
        subtextField2.gameObject.SetActive(!string.IsNullOrEmpty(subtext2));

        // Disable the select object by default, it will be enabled when selected
        if (selectHightlight != null) selectHightlight.gameObject.SetActive(false);
    }

    public void SetDescriptionFields()
    {
        parentMenuBase.descriptionField.text = description;
        parentMenuBase.mechanicalDescriptionField.text = mechanicalDescription;
    }

    public void OnItemClicked()
    {
        if (parentMenuBase != null)
        {
            SetDescriptionFields();
            if (listIndex != -1)
                parentMenuBase.OnListItemSelected(listIndex);
        }

        UpdateSelectionHighlight();
    }

    public void UpdateSelectionHighlight()
    {
        Transform parent = transform.parent;
        if (parent != null)
        {
            foreach (Transform child in parent)
            {
                ListItem li = child.GetComponent<ListItem>();
                if (li != null)
                    li.SetSelected(li == this);
            }
        }
    }

    public void SetSelected(bool selected)
    {
        selectHightlight.gameObject.SetActive(selected);
    }

    public void Stamp()
    {
        IsStamped = true;
        stampIcon.gameObject.SetActive(true);
        StartCoroutine(StampAnimation());
    }

    public void Unstamp()
    {
        IsStamped = false;
        stampIcon.gameObject.SetActive(false);
        StartCoroutine(UnstampAnimation());
    }

    // Coroutine to make the stamp expand from 0 to 1 scale, as well as shake left and right a little bit
    private IEnumerator StampAnimation()
    {
        float elapsed = 0f;
        Vector3 targetScale = Vector3.one; // Final scale of the stamp
        Vector3 originalPosition = stampIcon.transform.localPosition;

        while (elapsed < stampedGrowDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / stampedGrowDuration;

            // Scale the stamp from 0 to 1
            stampIcon.transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, t);

            yield return null;
        }

        while (elapsed < stampedGrowDuration + stampedShakeDuration)
        {
            elapsed += Time.deltaTime;
            float t = (elapsed - stampedGrowDuration) / stampedShakeDuration;

            // Shake left and right using a sine wave
            float shakeOffset = originalPosition.x + Mathf.Sin(t * Mathf.PI * 4) * stampedShakeMagnitude; // 4 full shakes
            float rotationOffset = Mathf.Sin(t * Mathf.PI * 4) * stampedShakeRotation;

            stampIcon.transform.localPosition = new Vector3(shakeOffset, originalPosition.y, originalPosition.z);
            stampIcon.transform.localRotation = Quaternion.Euler(0, 0, rotationOffset);

            yield return null;
        }

        // Ensure final state is correct
        stampIcon.transform.localScale = targetScale;
        stampIcon.transform.localPosition = new Vector3(originalPosition.x, originalPosition.y, originalPosition.z);
    }

    private IEnumerator UnstampAnimation()
    {
        float elapsed = 0f;
        Vector3 originalScale = stampIcon.transform.localScale;
        Vector3 targetScale = Vector3.zero; // Final scale of the stamp

        while (elapsed < stampedGrowDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / stampedGrowDuration;

            // Scale the stamp from 1 to 0
            stampIcon.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);

            yield return null;
        }

        // Ensure final state is correct
        stampIcon.transform.localScale = targetScale;
    }
}
