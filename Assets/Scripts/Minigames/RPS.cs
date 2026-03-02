using UnityEngine;

public class RPS : MonoBehaviour
{

    //very simple: take the input, figure out who won,

    private int enemyDecision; //worked out right at the start

    void Start()
    {
        enemyDecision = Random.Range(0,3); 
    }

    void DecideVictory(int victory) //0=draw 1 win 2 lose
    {
        if(victory == 0)
        {
            //draw, start again
        }else if(victory == 1)
        {
            //we win
        }else if(victory == 2)
        {
            
        }


    }

    public void ButtonPressed(string decision)
    {
        if(decision == "rock")
        {
            switch (enemyDecision)
            {
                case 0 : 
                DecideVictory(0);
                break;

                case 1 : 
                DecideVictory(2);
                break;

                case 2 : 
                DecideVictory(1);
                break;                               

            }

        }
        else if(decision == "paper")
        {
            switch (enemyDecision)
            {
                case 0 : 
                DecideVictory(1);
                break;

                case 1 : 
                DecideVictory(0);
                break;

                case 2 : 
                DecideVictory(2);
                break;                               
            }            
        }
        else if(decision == "scissors")
        {
            switch (enemyDecision)
            {
                case 0 : 
                DecideVictory(2);
                break;

                case 1 : 
                DecideVictory(1);
                break;

                case 2 : 
                DecideVictory(0);
                break;                               
            }   

        }
    }
}
