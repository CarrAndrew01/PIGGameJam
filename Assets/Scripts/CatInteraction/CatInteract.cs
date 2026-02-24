using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CatInteract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Image outlineImage;
    public Image pointerImage;
    Color startColour;

    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startColour = outlineImage.color;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        OutlineCat(true);
    }
    public virtual void OnPointerExit(PointerEventData eventData)
    {
        OutlineCat(false);
    }
    // onpointerclick is used here, somewhere else i will add button presses
    public virtual void OnPointerClick(PointerEventData eventData)
    {
        InteractWithCat();
    }
    public virtual void InteractWithCat()
    {
        
    }

    void OutlineCat(bool on)
    {
        // if the bool is true, set the alpha to full, otherwise set to 0
        var alpha = on ? 255 : 0;

        if (outlineImage != null)
        {
            outlineImage.color = new Color(startColour.r, startColour.g, startColour.b, alpha);
        }
        if (pointerImage != null)
        {
            pointerImage.color = new Color(startColour.r, startColour.g, startColour.b, alpha);
        }
    }
}
