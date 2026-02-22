using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CatInteract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Image outlineImage;
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
    public void OnPointerEnter(PointerEventData eventData)
    {
        OutlineCat(true);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        OutlineCat(false);
    }
    // onpointerclick is used here, somewhere else i will add button presses
    public void OnPointerClick(PointerEventData eventData)
    {
        InteractWithCat();
    }
    public void InteractWithCat()
    {

    }

    void OutlineCat(bool on)
    {
        // if the bool is true, set the alpha to full, otherwise set to 0
        var alpha = on ? 255 : 0;

        if (outlineImage != null)
        {
            outlineImage.color = new Color(outlineImage.color.r, outlineImage.color.g, outlineImage.color.b, alpha);
        }
    }
}
