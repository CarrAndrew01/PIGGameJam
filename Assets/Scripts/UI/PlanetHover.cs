using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class PlanetHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [Header("Planet Data")]
    public string planetName;
    public string planetSceneName;
    public StatType requiredEntryStat;
    public bool canEnter => GameManager.GetPlayerStat(requiredEntryStat) >= 1f;

    [Header("Visuals")]
    public GameObject arrow;
    public GameObject nameLabel; 
    private Button button;

    public GameObject noAccessShadow;


    private void Awake()
    {
       // SetVisualsActive(false); no ned for this, they're disabled on default now
        button = GetComponent<Button>();

        if(noAccessShadow != null) noAccessShadow.SetActive(!canEnter);
    }

    private void OnEnable()
    {
        MenuManager.OnMenuStateChanged += HandleMenuStateChanged;

        // Sync to current state in case events were missed while disabled
        if (button != null)
            button.enabled = !MenuManager.IsAnyMenuOpen;
    }

    private void OnDisable()
    {
        MenuManager.OnMenuStateChanged -= HandleMenuStateChanged;
    }

    private void HandleMenuStateChanged(bool menuOpen)
    {
        if (button != null)
            button.enabled = !menuOpen;

        if (menuOpen)
        {
            if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == gameObject)
                EventSystem.current.SetSelectedGameObject(null);
            SetVisualsActive(false);
        }
    }

    // Mouse

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetVisualsActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetVisualsActive(false);
    }

    // Controller / Keyboard navigation

    public void OnSelect(BaseEventData eventData)
    {
        SetVisualsActive(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        SetVisualsActive(false);
    }

    // Entry (called by Button.onClick)
    public void TryEnter()
    {
        if (canEnter)
        {
            if (TransitionManager.Instance != null) {
            TransitionManager.Instance.BeginSceneTransition(planetSceneName);
            } else {
            PlanetSelection.Instance.GoToPlanetScene(planetSceneName);
            }
        }
        else
        {
            Toast.ShowToast("Your ship can't go there yet!");
        }
    }

    private void SetVisualsActive(bool active)
    {
        if (!active)
        {
            //disable everything if we're deactivating
            if(nameLabel!=null) nameLabel.SetActive(false);
         //   if(arrow!=null) arrow.SetActive(false);
        }
        else
        {
            if (canEnter)
            {
                nameLabel.GetComponent<TextMeshProUGUI>().text = planetName;
                if(nameLabel!=null) nameLabel.SetActive(true);
                //if(arrow!=null) arrow.SetActive(true);
            }
            else
            {
                nameLabel.GetComponent<TextMeshProUGUI>().text = "???\n<color=#FF0000><size=65%>[Inaccessible]</size></color>";
                if(nameLabel!=null) nameLabel.SetActive(true);
             //   if(arrow!=null) arrow.SetActive(false);
            }
        }
    }

    //working this out now to code later
    //first function takes bool for enable/disable
    //passes it to a new function depending on whether we can enter that planet yet

    //


    //everything is just disabled from default and we enable it whern hovered, I dont know why thats not working but I'll fix it
    //because its a better way to do it
 





}
