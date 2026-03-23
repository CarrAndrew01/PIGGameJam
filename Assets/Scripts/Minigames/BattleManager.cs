using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Events;



//I never know what to call these, this an actual pokemon that exists in the game itself, which references the stats
public class Pokemon
{
    public PokemonStats stats = new(); //what the actual pokemon it is

    public List<Attack> attacks = new(); //the attacks this pokemon can use, which will be used in the battle system

    public GameObject healthbar; //gameobject for now, this shouldn't be here but I'm making ANOTHER class for screen-representative pokemon here
    public float healthBarScaleInitial = 0.5f;
    public float currentHealth = 100;
    //A more complete implementation would have this be some kind of data structure instead of a bool


    //we're using none of these lol
    // public int attackStat;
    // public int defenseStat;
    // public int specialAttackStat;
    // public int specialDefenseStat;
    // public int speedStat;
}


[CreateAssetMenu(fileName = "Pokemon Stats", menuName = "Pokemon Stats")]
public class PokemonStats
{
    public int maxHealth = 100;//for now we'll use this to store the max health since we're not doing stats or levels
    public List<Attack> attacks = new(); //the attacks this pokemon could potentially learn

    //we're using none of these lol
    // public int attackStat;
    // public int defenseStat;
    // public int specialAttackStat;
    // public int specialDefenseStat;
    // public int speedStat;
}



public class BattleData
{
    //only 2 at the moment, its gen 1 rules
    public Pokemon currentPlayerMon = new Pokemon();
    public Pokemon currentEnemyMon = new Pokemon();
}

[System.Serializable]
public class ButtonPress : UnityEvent
{
    // Parameterless version (equivalent to Action / void Method())
}

public class BattleManager : MonoBehaviour
{

    public enum UIState
    {
        Main,
        Attack,
        Paused
    }

    public UIState currentUIState;

    [System.Serializable]
    public class ButtonUI
    {
        public string name;
        public GameObject arrow;

        public TextMeshProUGUI buttonText;

        [SerializeField]
        private ButtonPress onChosen;           // ← appears in Inspector like a Button's onClick

        public void InvokeOnChosen()
        {
            onChosen?.Invoke();
        }


        public int horizontalPos; //0 for left, 1 for right.
        //other way to do this is to reference a left-direction button, right direction etc. to each ive done that befor ebut its a pain todrag all the refernces
        public int verticalPos; 
    }

    public List<ButtonUI> attackButtons;
    public List<ButtonUI> optionButtons;

    public ButtonUI currentSelectedButton;


    public BattleData battleData = new BattleData();

    public GameObject AttacksPanel;
    public GameObject OptionsPanel;

    public TextMeshProUGUI mainText;

    public InputActionReference moveAction; // expects Vector2, only the x component is used for left/right movement
    public InputActionReference selectAction; // expects Vector2, only the x component is used for left/right movement

    bool movingLastFrame = false; //for some reason you have to do this to stop it being fast for the vector thing whatever
 

    public FishShadow lastFish;

    public Attack Struggle;
    public Attack RestAttack;

    


    #region UIStuff

    public void OnAttackButtonPressed(bool enableOrNot)
    {
        AttacksPanel.SetActive(enableOrNot);
    }
    #endregion

    void Start()
    {
        currentSelectedButton = optionButtons[0];
        currentSelectedButton.arrow.SetActive(true);

        battleData.currentEnemyMon.attacks.Add(Struggle);
        battleData.currentEnemyMon.attacks.Add(RestAttack);







        Transform[] allDescendants = transform.GetComponentsInChildren<Transform>(true);

        foreach (Transform t in allDescendants)
        {
            // Skip the root itself
            if (t == transform) continue;

            if (t.gameObject.name == "HealthBarPlayer")           // Exact match
            // if (t.gameObject.name.Contains(targetName)) // Partial match
            {
                battleData.currentPlayerMon.healthbar = t.gameObject;
            }else if (t.gameObject.name == "HealthBarEnemy")
            {
                battleData.currentEnemyMon.healthbar = t.gameObject;
            }
        }


        battleData.currentPlayerMon.healthBarScaleInitial = battleData.currentPlayerMon.healthbar.transform.localScale.x;
        battleData.currentEnemyMon.healthBarScaleInitial = battleData.currentEnemyMon.healthbar.transform.localScale.x;
    }

    void Update()
    {
        InputManager();
    }


    public void InitializeBattle()
    {
        //set up the battle data, get the player stats and the enemy stats, etc.
    }

    public void UpdateHealthBar()
    {
        
    }

    public ButtonUI GetNextButton(List<ButtonUI> buttons, Vector2 inputDirection)
    {
        Debug.Log(inputDirection);
        //Based on currentlySelectedButton, which of the two lists we're using, we return the buttonUI that matches

        foreach(ButtonUI button in buttons)
        {
            //There HAS to be a better way of doing this with less lines of code
            if(inputDirection.x > 0.2f
            && button.verticalPos == currentSelectedButton.verticalPos
            && button.horizontalPos == currentSelectedButton.horizontalPos + 1)//arbitrary values I just want the joystick to not accidentally trigger it
            {
                return button;
            }
            else if(inputDirection.x < -0.2f
            && button.verticalPos == currentSelectedButton.verticalPos
            && button.horizontalPos == currentSelectedButton.horizontalPos - 1)//arbitrary values I just want the joystick to not accidentally trigger it
            {                
                return button;
            }else if(inputDirection.y > 0.2f
            && button.horizontalPos == currentSelectedButton.horizontalPos
            && button.verticalPos == currentSelectedButton.verticalPos - 1)
            {
                return button;
            }
            else if(inputDirection.y < -0.2f
            && button.horizontalPos == currentSelectedButton.horizontalPos
            && button.verticalPos == currentSelectedButton.verticalPos + 1)
            {
                return button;
            }
        }
        return null;
    }

    public void InputManager()
    {
        //I can't even remember how the inputs work
        //fuck it I'm coming back to this stupid fucking minigame I hate it so much
        Vector2 currentDirection = moveAction.action.ReadValue<Vector2>();

        bool isMovingThisFrame  = currentDirection.sqrMagnitude > 0.01f;           // small deadzone
        bool startedMovingThisFrame = isMovingThisFrame && !movingLastFrame;


        if(startedMovingThisFrame){
            switch (currentUIState)
            {
                case UIState.Main:
                    // Handle main UI state
                    //in order to facilitate proper
                    ButtonUI nextOpt = GetNextButton(optionButtons, currentDirection);
                    if(nextOpt != null){
                        currentSelectedButton.arrow.SetActive(false);
                        currentSelectedButton = nextOpt;
                        currentSelectedButton.arrow.SetActive(true);
                    }
                    
                    break;
                case UIState.Attack:
                    // Handle attack UI state
                    // Handle main UI state
                    //in order to facilitate proper
                    ButtonUI nextAtk = GetNextButton(attackButtons, currentDirection);
                    if(nextAtk != null){
                        currentSelectedButton.arrow.SetActive(false);
                        currentSelectedButton = nextAtk;
                        currentSelectedButton.arrow.SetActive(true);
                    }

                    break;
            }
        }

        if(selectAction.action.WasPressedThisFrame())
        {
            currentSelectedButton.InvokeOnChosen();
        }

        movingLastFrame  = isMovingThisFrame;
    }

    public void StartTurn()
    {
        //player always moves first, not doing speed atm


    }

    //called by the animation event at the end of the attack animation
    public void OnAnimationEnd(bool player)
    {
        //switch to the other attack or go to the next turn.
        if (player)
        {
            ImplementMove(battleData.currentPlayerMon, battleData.currentEnemyMon, EnemyAIDecider());
        }
        else
        {
            StartTurn();
        }
    }


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

    //works in damage and healing
    void AdjustHealth(int num, Pokemon pokemon)
    {
        //adjust the UI element of the healthbar, and also the actual health variable.
        pokemon.currentHealth += num;

        AdjustHealthbar(pokemon);

        //UI stuff

    }

    void ImplementMove(Pokemon target, Pokemon originator, Attack attack)
    {
        int roll = Random.Range(0, 100);

        if(roll <= attack.accuracy)
        {
            //hit
            if(attack.damage != -10000)
            {
                AdjustHealth(-attack.damage, target);
            }
            
            if(attack.healAmount != -10000)
            {
                AdjustHealth(attack.healAmount, originator);
            }
        }
        else
        {
            //miss
        }
    }

    /*
    <summary>Handles the player's move based on the input number.</summary>
    <param name="num">0 and 1 are the top row of buttons, 2 and 3 are the bottom row of buttons.</param>
    */
    public void OnMovePressed(Attack atk)
    {
        //num always corresponds to the position in the attacks list
 
        if(atk!=null)
        {
            ImplementMove(battleData.currentEnemyMon, battleData.currentPlayerMon, atk);
            //for now, no animation so immediately call 
          //  OnAnimationEnd(true);
        }
        else
        {
            Debug.Log("Player tried to use an attack that doesn't exist");
        }
    }


    Attack EnemyAIDecider()
    {
        //just decide randomly I think
        int rand = Random.Range(0, battleData.currentEnemyMon.attacks.Count);
        return battleData.currentEnemyMon.attacks[rand];
    }

    
    public void OnFleePressed()
    {
        //kill the minigame unceremoniously  
        if(lastFish != null)
        {
            lastFish.EndFishing(false);
        }
    }

    public void OnCatchPressed()
    {
        //randomly decide if the catch is successful or not

    }

    void AdjustHealthbar(Pokemon pokemon)
    {
        Debug.Log(pokemon.currentHealth);
        //set the scale of the healthbar based on the pokemon's current health
        
        //adjust the healthbar scale, taking itno account its initial scale
        float healthPercent = pokemon.currentHealth / pokemon.stats.maxHealth;
        pokemon.healthbar.transform.localScale = new Vector3(pokemon.healthBarScaleInitial * healthPercent, 
        pokemon.healthbar.transform.localScale.y, pokemon.healthbar.transform.localScale.z);
        
    }

    public void OnAttacksButtonPressed()
    {
        OptionsPanel.SetActive(false); ;
        AttacksPanel.SetActive(true);
        currentUIState = UIState.Attack;
        currentSelectedButton = attackButtons[0];
        currentSelectedButton.arrow.SetActive(true);   
    }
}
