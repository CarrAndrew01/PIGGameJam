using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using TMPro;

public class BlackjackButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    TextMeshProUGUI text;

    Color32 textDefaultColor = new Color32(247, 150, 23, 166);
    Color32 textHoverColor = new Color32(255, 255, 255, 166);

    public BlackjackScript blackjackScript;
    // true = hit, false = stand. THIS IS STUPID CODE THAT NEEDS TO BE FIXED
    public bool buttonType;


    private void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        blackjackScript = GetComponentInParent<BlackjackScript>();
    }
    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (text != null) text.color = textHoverColor;
    }
    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if (text != null) text.color = textDefaultColor;
    }
    // onpointerclick is used here, somewhere else i will add button presses
    public virtual void OnPointerClick(PointerEventData eventData)
    {
        // do stuff
        if (blackjackScript == null) return;
        if (buttonType)
        {
            blackjackScript.Hit();
        }
        else
        {
            blackjackScript.Stand();
        }
    }
}
