using UnityEngine;

public class Pokemon : MonoBehaviour
{

    
    void AdjustBasedOnPlayerUpgrades()
    { 
        
        int statCatchSpeed, statCatchArea, statHookGravity, statFishEscapeRate, statHookPullForce;
        
        //amount of damage you do
        statCatchSpeed = GameManager.Instance.GetAmountOfUpgrades("CatchSpeed", 1, 0, true);
        
        //amount of health enemy has
        statCatchArea = GameManager.Instance.GetAmountOfUpgrades("CatchArea");
        

        statHookGravity = GameManager.Instance.GetAmountOfUpgrades("Sinker");
        
        statFishEscapeRate = GameManager.Instance.GetAmountOfUpgrades("EscapeRate");






    }

    //keeping it simple, player has 4 options:

    //Reel
    //damages?

    //wrestle
    //

    //relax
    //heal self

    //catch
    //risk of failure if health isn't low enough




    //run



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {



        
    }

    // Update is called once per frame
    void Update()
    {
        


    }
}
