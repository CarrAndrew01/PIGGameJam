using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class WAMtarget : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{   
    public WhackAMole parent;

    public virtual void OnPointerClick(PointerEventData eventData)
    {        
        parent.OnClicked(this.gameObject);
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {

    }
    public virtual void OnPointerExit(PointerEventData eventData)
    {

    }

}
