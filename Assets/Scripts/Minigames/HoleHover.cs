using UnityEngine;
using UnityEngine.EventSystems;

public class HoleHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{


    public void OnPointerEnter(PointerEventData eventData)
    {
        //PlanetSelection.Instance.OnHoverNew(gameObject);
        Debug.Log("enter  " + eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("exit  " + eventData);
    }
    
    // onpointerclick is used here, somewhere else i will add button presses
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("click  " + eventData);
    }
}
