using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CatInteract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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
        if (outlineImage != null)
        {
            outlineImage.color = new Color(outlineImage.color.r, outlineImage.color.g, outlineImage.color.b, 255);
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (outlineImage != null)
        {
            outlineImage.color = new Color(outlineImage.color.r, outlineImage.color.g, outlineImage.color.b, 0);
        }
    }
}
