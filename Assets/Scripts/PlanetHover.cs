using UnityEngine;
using UnityEngine.EventSystems;

public class PlanetHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public string planetName;
    public StatType requiredEntryStat;
    public bool CanEnter => GameManager.GetPlayerStat(requiredEntryStat) >= 1f;

    public void OnPointerEnter(PointerEventData eventData)
    {
        PlanetSelection.Instance.OnHoverNew(gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("exit  " + eventData);
    }

    // onpointerclick is used here, somewhere else i will add button presses
    public void OnPointerClick(PointerEventData eventData)
    {
        // Check player stat
        if (CanEnter)
        {
            PlanetSelection.Instance.GoToPlanetScene(planetName);
        }
        else
        {
            Debug.Log("Player does not have the required stat to enter " + planetName);
        }
    }
}
