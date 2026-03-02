using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Hand
{
    public string win;
    public string lose;
    public string draw;

    public Sprite sp;

}

public class RPS : MonoBehaviour
{

    //very simple: take the input, figure out who won,

    private int enemyDecision; //worked out right at the start
    FishShadow fishShadow;

    int currentSprite = 0;
    public List<Hand> hands = new();

    public Image ourDisplay;
    public Image theirDisplay;

    public TextMeshProUGUI declarationText;

    public Sprite mysteryBox;

    public float timer;

    bool gameEndScreen = false;

    void Start()
    {
        fishShadow = Fishing.LastFishShadow;

        enemyDecision = Random.Range(0,3); 
        timer = 0;
    }

    void ChangeSprite()
    {
        timer = 0;
        
        if(currentSprite + 1 >= hands.Count)
        {
            currentSprite = 0;
        }
        else
        {
            currentSprite++;
        }
        ourDisplay.sprite = hands[currentSprite].sp;
    }

    void Update()
    {
        if(gameEndScreen) return;

        timer += Time.deltaTime;

        if(timer > 0.7f)
        {
            ChangeSprite();    
        }
    }
    
    void ReloadGame()
    {
        enemyDecision = Random.Range(0,3); 
        timer = 0;
        gameEndScreen = false;
        theirDisplay.sprite = mysteryBox;
    }

    IEnumerator DecideVictory(int victory) //0=draw 1 win 2 lose
    {   
        theirDisplay.sprite = hands[enemyDecision].sp;

        if(victory == 0)
        {
            declarationText.text = "Draw!";
            //draw, start again
            //so do nothing for now

            yield return new WaitForSeconds(2f);
            ReloadGame();

        }else if(victory == 1)
        { 

            //we win
            declarationText.text = "You Win!";
            yield return new WaitForSeconds(2f);
            //ReloadGame();

            fishShadow.EndFishing(true);

        }else if(victory == 2)
        {
            
            declarationText.text = "You Lose!";

            yield return new WaitForSeconds(2f);
            //ReloadGame();
            //we done lost
            fishShadow.EndFishing(false);
        }
    }

    public void ButtonPressed()
    {
        gameEndScreen = true;
        Hand current = hands[currentSprite];
 
        if(hands[enemyDecision].draw == current.draw)
        {
            StartCoroutine(DecideVictory(0));
        }else if(hands[enemyDecision].draw == current.win)
        {
            StartCoroutine(DecideVictory(1));
        }else if(hands[enemyDecision].draw == current.lose)
        {
            StartCoroutine(DecideVictory(2));
        }
    }
}
