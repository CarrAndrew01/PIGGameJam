using UnityEngine;
using UnityEngine.UI;

public class BaitMenu : MonoBehaviour
{   
    // State
    private bool isBaitSelected = false;

    // Variables

    [Header("Components")]
    public Menu menuComponent;
    public Button selectButton;

    void Awake()
    {
        menuComponent.selectedIndex = GameManager.Instance.playerInventory.baits.FindIndex(b => b.baitUpgrade == GameManager.Instance.playerInventory.currentBaitUpgrade);
    }

    void Start()
    {
        if (menuComponent != null)
        {
            menuComponent.PopulateListWithBaits(GameManager.Instance.playerInventory.baits);
        }
    }

    public void OnListItemSelected()
    {
        if (menuComponent.selectedIndex != -1)
        {
            selectButton.interactable = true;
            isBaitSelected = true;
        }
    }

    public void SelectBait()
    {
        int baitIndex = menuComponent.selectedIndex;
        if (baitIndex != -1 && baitIndex < GameManager.Instance.playerInventory.baits.Count)
        {
            Bait selectedBait = GameManager.Instance.playerInventory.baits[baitIndex];
            GameManager.SelectBaitUpgrade(selectedBait.baitUpgrade);
        }
        else
        {
            Debug.LogWarning("Invalid bait index selected.");
        }
    }
}
