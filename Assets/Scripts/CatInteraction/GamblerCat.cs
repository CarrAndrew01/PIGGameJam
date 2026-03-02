using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GamblerCat : CatInteract
{
    private Menus menus;
    private Coroutine interactionCoroutine;

    public override void Start()
    {
        base.Start();
        menus = GameManager.MenuPopup.GetComponent<Menus>();
    }

    public override void InteractWithCat()
    {
        base.InteractWithCat();

        // Goto blackjack scene
        if (interactionCoroutine == null)
            interactionCoroutine = StartCoroutine(WaitAndGoToBlackjack());
    }

    // Coroutine to wait for 1 second
    private IEnumerator WaitAndGoToBlackjack()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Jack's Minigame");
    }
}
