using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CatInteract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Image outlineImage;
    public Image pointerImage;
    Color startColour;

    public CatSound catsound = CatSound.Medium_Meow;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Start()
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
        switch (catsound)
        {
            case (CatSound.High_Meow):
                AudioManager.playSound?.Invoke("High_Meow");
                break;
            case (CatSound.Medium_Meow):
                AudioManager.playSound?.Invoke("Medium_Meow");
                break;
            case (CatSound.Low_Meow):
                AudioManager.playSound?.Invoke("Low_Meow");
                break;

        }
    }
    public virtual void InteractWithCat()
    {

    }

    void OutlineCat(bool on)
    {
        // if the bool is true, set the alpha to full, otherwise set to 0
        var alpha = on ? 1 : 0;

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
public enum CatSound
{
    High_Meow,
    Medium_Meow,
    Low_Meow
}
