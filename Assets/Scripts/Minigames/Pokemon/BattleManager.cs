using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using System.Collections;



//I never know what to call these, this an actual pokemon that exists in the game itself, which references the stats
//A more complete implementation would have three structures: a stats structure that never "exists" in game, a Pokemon struct for the pokemon in your party
//and a BattlePokemon struct for battle, which has stat drops and such
[System.Serializable]
public class Pokemon
{
 
    public PokemonStats stats = new(); //what the actual pokemon it is

    public List<Attack> attacks = new(); //the attacks this pokemon can use, which will be used in the battle system

    public GameObject healthbar; //gameobject for now, this shouldn't be here but I'm making ANOTHER class for screen-representative pokemon here
    public float healthBarScaleInitial = 0.5f;

    //MODIFIERS
    //rather than stats, this 

    //not going to worry about keep reference to what upgrades are being applied for speed and health, we'll just change these in Start()
    public float currentSpeed; 
    public float currentHealth = 100;

    public float defenceModifier;
    public float attackModifier;
    public float chanceToUseThird; //fish has a third attack, this determines how often it uses it. But, the 




    public float accuracyModifier = 1f;
    //A more complete implementation would have this be some kind of data structure instead of a bool

    //fffffuck I should hav eput the animator on the main parent object probably. Wah
    public Animator animator;

    public GameObject go; //the actual gameobject in the scene representing this pokemon, which will be used for animations and such. 

    //fucking FUCK WHY CANT I JUST INSTANTIATE IN UNITIES ANIMATION SYSTEM GOD FUCKING DAMN IT
    //its all hack code, all the way down
    public GameObject spawnPoint; //where Z's spawn

    //we're using none of these lol
    // public int attackStat;
    // public int defenseStat;
    // public int specialAttackStat;
    // public int specialDefenseStat;
    // public int speedStat;

    public Attack currentAttack;//null if nothing- NULL CHECKS NEEDED
}


[CreateAssetMenu(fileName = "Pokemon Stats", menuName = "Pokemon Stats")]
public class PokemonStats
{
    public string nameOfPokemon;

    //might as well put the base stats in here, and the upgrade/fish stat effected versions in the Pokemon variables
    public int maxHealth = 100;//for now we'll use this to store the max health since we're not doing stats or levels
    public int speed = 100;
    //defence/attack doesn't need a stat here really


    public List<Attack> attacks = new(); //the attacks this pokemon could potentially learn

    //we're using none of these lol
    // public int attackStat;
    // public int defenseStat;
    // public int specialAttackStat;
    // public int specialDefenseStat;
    // public int speedStat;
}


[System.Serializable]
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

        public Attack attack;



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

    [SerializeField]
    public BattleData battleData = new BattleData();

    public GameObject attacksPanel;
    public GameObject optionsPanel;
    public GameObject descriptionPanel;

    public TextMeshProUGUI mainText;

    public InputActionReference moveAction; // expects Vector2, only the x component is used for left/right movement
    public InputActionReference selectAction; 
    public InputActionReference backAction;


    bool movingLastFrame = false; //for some reason you have to do this to stop it being fast for the vector thing whatever
    
    //bigger number = easier to catch
    private float catchModifier = 0f;

    public FishShadow lastFish;

    public Attack struggle;
    public Attack restAttack;


    //Animation stuff
    //non-flexible implementation
    //protected classes? Something else?
    //hm.
    public GameObject restZ;


    #region UIStuff

    public void OnAttackButtonPressed(bool enableOrNot)
    {
        attacksPanel.SetActive(enableOrNot);
    }
    #endregion

    void Start()
    {
        currentSelectedButton = optionButtons[0];
        currentSelectedButton.arrow.SetActive(true);

        battleData.currentEnemyMon.attacks.Add(struggle);
        battleData.currentEnemyMon.attacks.Add(restAttack);

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

        battleData.currentPlayerMon.stats.nameOfPokemon = "You";
        battleData.currentEnemyMon.stats.nameOfPokemon = "Fish";

        if(Fishing.LastFishShadow != null){
            lastFish = Fishing.LastFishShadow;
        }


    }

    void Update()
    {
        InputManager();
    }


    public void InitializeBattle()
    {
        //set up the battle data, get the player stats and the enemy stats, etc.
    }

    //keeping this simple right now, just don't have time to do a full extensible implementation
    public IEnumerator RestAnimationCR()
    {
        GameObject go1 = Instantiate(restZ, battleData.currentPlayerMon.spawnPoint.transform.position, Quaternion.identity, battleData.currentPlayerMon.spawnPoint.transform);
        yield return new WaitForSeconds(0.5f);
        GameObject go2 = Instantiate(restZ, battleData.currentPlayerMon.spawnPoint.transform.position, Quaternion.identity, battleData.currentPlayerMon.spawnPoint.transform);
        yield return new WaitForSeconds(0.5f);
        GameObject go3 = Instantiate(restZ, battleData.currentPlayerMon.spawnPoint.transform.position, Quaternion.identity, battleData.currentPlayerMon.spawnPoint.transform);
        //animation ending is just handled in the central animation for now


    }

    //keeping this simple right now, just don't have time to do a full extensible implementation
    public IEnumerator RestAnimationEnemyCR()
    {
        GameObject go1 = Instantiate(restZ, battleData.currentEnemyMon.spawnPoint.transform.position, Quaternion.identity, battleData.currentEnemyMon.spawnPoint.transform);
        yield return new WaitForSeconds(0.5f);
        GameObject go2 = Instantiate(restZ, battleData.currentEnemyMon.spawnPoint.transform.position, Quaternion.identity, battleData.currentEnemyMon.spawnPoint.transform);
        yield return new WaitForSeconds(0.5f);
        GameObject go3 = Instantiate(restZ, battleData.currentEnemyMon.spawnPoint.transform.position, Quaternion.identity, battleData.currentEnemyMon.spawnPoint.transform);
        //animation ending is just handled in the central animation for now

    }


    public IEnumerator FakeRestAnimationCR()
    {
        GameObject go1 = Instantiate(restZ);
        yield return new WaitForSeconds(0.5f);
        GameObject go2 = Instantiate(restZ);
        yield return new WaitForSeconds(0.4f);
        //animation ending is just handled in the central animation for now
        Destroy(go1);
        Destroy(go2);
        
    }



    /*
    ATTACK ANIMATIONS POKEMON
    how would I do this in a more complete implementation, given the unity asset system isn't itself set up for instantiation inside animations really
    probably in a seperate file, have a big list of animations. No real way around having an animator on the instantiated objects 
    thoughts: the unity animation system really isn't fit for purpose for this. It can't instantiate objects without an object and has no native way of 
    keeping track of instantiated objects. 

    FOR LATER:
    "Attack Animation" is it's own class, referenced in each Attack class. Do they all need their own controller? not sure. 
    but we basically enter into an animating state whenever an attack starts, and at the end of every attack a function is called that puts it back
    thats what we're essnetially doing here with states.
    I don'y think there's a great way around having to manually put an endpoint in the animation, and if we're instantiating like 5 things for an animation
    then someone has to work out which objects animation is the "final" one OR calculate how much time the animation takes in total and 
    put it in a central animation. I don't get why the unity animation system can't just figure out instantiation and let you move things within an animation
    it doesn't seem impossible.


    //FOR NOW, we're just gonna hard end the instantiating animations and go off rough timings
    */
    public void RestAnimation(string name)
    {
        switch(name)
        {
            case "Rest":
                StartCoroutine(RestAnimationCR());
                break;
            case "FakeRest":
                StartCoroutine(FakeRestAnimationCR());
                break;
            
        }
    }



    public ButtonUI GetNextButton(List<ButtonUI> buttons, Vector2 inputDirection)
    {
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
        if(currentUIState == UIState.Paused)
        {
            return;
        }
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
                        
                        if(currentSelectedButton.attack != null){
                            descriptionPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = currentSelectedButton.attack.description;
                        }
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

                        if(currentSelectedButton.attack != null){
                            descriptionPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = currentSelectedButton.attack.description;
                        }
                    }

                    break;
            }
        }

        if(selectAction.action.WasPressedThisFrame())
        {
            currentSelectedButton.InvokeOnChosen();
        }

        if(backAction.action.WasPressedThisFrame())
        {
            switch (currentUIState)
            {
                case UIState.Main:
                    // Handle main UI state
                    //exit? or nothing.
                    break;
                case UIState.Attack:
                    OnOptionsButtonPressed();
                    break;
            }
        }



        movingLastFrame  = isMovingThisFrame;
    }

    public void StartTurn()
    {
        //player always moves first, not doing speed atm
        attacksPanel.SetActive(true);
        descriptionPanel.SetActive(true);
        currentUIState = UIState.Attack;
        currentSelectedButton = attackButtons[0];
        currentSelectedButton.arrow.SetActive(true);
        battleData.currentPlayerMon.animator.SetInteger("AttackID",-1);
        battleData.currentEnemyMon.animator.SetInteger("AttackID",-1);

        mainText.text = "";

        second = false;
    }
    
    bool second;
    //called by the animation event at the end of the attack animation
    public void OnAnimationEnd(bool player)
    {
        Debug.Log("animend  " + player + "    second: " + second);
        //switch to the other attack or go to the next turn.

        if(second)
        {
            if (player)
            {
                ImplementMove(battleData.currentEnemyMon, battleData.currentPlayerMon, battleData.currentPlayerMon.currentAttack);
            }
            else
            {
                ImplementMove(battleData.currentPlayerMon, battleData.currentEnemyMon, battleData.currentEnemyMon.currentAttack);
            }
            
            StartTurn();
            return;
        }

        if (player)
        {
            ImplementMove(battleData.currentEnemyMon, battleData.currentPlayerMon, battleData.currentPlayerMon.currentAttack);
            ImplementMoveAnimation(battleData.currentPlayerMon, battleData.currentEnemyMon, battleData.currentEnemyMon.currentAttack);

            battleData.currentPlayerMon.animator.SetInteger("AttackID",-1);
        }
        else
        {
            ImplementMove(battleData.currentPlayerMon, battleData.currentEnemyMon, battleData.currentEnemyMon.currentAttack);
            ImplementMoveAnimation(battleData.currentEnemyMon, battleData.currentPlayerMon, battleData.currentPlayerMon.currentAttack);
 
            battleData.currentEnemyMon.animator.SetInteger("AttackID", -1);
        }

        second = true;

    }

    //as any fish can randomly generate the pokemon minigame, this is based on their stats
    void AdjustBasedOnFishStats()
    {
        //first, assign each of the fish's stat
        Pokemon pkmn = battleData.currentEnemyMon;

        //everything starts at a base value which is multiplied by 1 + the fish stats (between 0 and 1). so speed starts at 100 and becomes between 100 and 200
        pkmn.currentSpeed *= 1 + lastFish.fishData.fish.Speed;
        pkmn.attackModifier *= 1 + lastFish.fishData.fish.Jumpiness;
        pkmn.defenceModifier *= 1 + lastFish.fishData.fish.Size;

        //stubborness makes it just slightly more difficult to catch, between 0 and 0.1
        catchModifier -= 0.1f * lastFish.fishData.fish.Stubbornness;
    }

    void AdjustBasedOnPlayerUpgrades()
    { 
        
        int statCatchArea, statHookGravity, statFishEscapeRate, statCatchSpeed;

        //amount of damage you do
       // catchModifier = GameManager.Instance.GetAmountOfUpgrades("CatchSpeed");


        
        //Catch area effects out damage
        statCatchArea = GameManager.Instance.GetAmountOfUpgrades("CatchArea");
        battleData.currentPlayerMon.attackModifier = 0.05f * statCatchArea;

        //fish escape rate lowers enemy accuracy
        statFishEscapeRate = GameManager.Instance.GetAmountOfUpgrades("EscapeRate");
        battleData.currentEnemyMon.accuracyModifier = 1 - (0.05f * statFishEscapeRate);


        //catch speed == better catch chance
        statCatchSpeed = GameManager.Instance.GetAmountOfUpgrades("CatchSpeed");
        catchModifier = 0.03f * statCatchSpeed;
        
        //hook gravity == buffs every attack in a slightly different way
        //actually lets just give us an attack buff
        statHookGravity = GameManager.Instance.GetAmountOfUpgrades("Sinker");
        battleData.currentPlayerMon.attackModifier += 0.05f * statHookGravity;
    }

    //works in damage and healing
    void AdjustHealth(float num, Pokemon pokemon)
    {

        if(pokemon.currentHealth + num > pokemon.stats.maxHealth)
        {
            pokemon.currentHealth = pokemon.stats.maxHealth;
        }
        else if(pokemon.currentHealth + num <= 0)
        {
            pokemon.currentHealth = 0;
            //trigger end of battle
        }else{
            //adjust the UI element of the healthbar, and also the actual health variable.
            pokemon.currentHealth += num;
        }

        AdjustHealthbar(pokemon);
    }


    //takes everything and work out the damage
    float CalculateDamage(Pokemon target, Pokemon originator, Attack attack)
    {
        //takes the base damage of the attack, multiply it by the attack 
        float damage = attack.damage;


        damage *= 1 + originator.attackModifier;
        damage *= 1 - target.defenceModifier; //this is quite high but we can just make base damage on moves quite high
        damage = Mathf.RoundToInt(damage);

        return damage;
    }

    void ImplementMove(Pokemon target, Pokemon originator, Attack attack)
    {
        //hit

        float dmg = CalculateDamage(target, originator, attack);

        if(attack.damage != -10000)
        {
            AdjustHealth(-dmg, target);
        }
        
        if(attack.healAmount != -10000)
        {
            AdjustHealth(attack.healAmount, originator);
        }
    }

    IEnumerator MoveMissed(Pokemon originator)
    {
        yield return new WaitForSeconds(1.5f);
        mainText.text = "The attack missed!";
        yield return new WaitForSeconds(1.5f);
        OnAnimationEnd(originator == battleData.currentPlayerMon);
    }
 
    void ImplementMoveAnimation(Pokemon target, Pokemon originator, Attack attack)
    {            
        mainText.text = $"{originator.stats.nameOfPokemon} used {attack.nameOfMove}!";

        int roll = Random.Range(0, 100);

        if(roll <= attack.accuracy * originator.accuracyModifier)
        {
            originator.animator.SetInteger("AttackID", attack.AnimationReturner());

        }
        else
        {
            StartCoroutine(MoveMissed(originator));
        }
    }

    void AttackAnimationsPause()
    {
        descriptionPanel.SetActive(false);
        attacksPanel.SetActive(false);
        currentSelectedButton.arrow.SetActive(false);
        currentUIState = UIState.Paused;
    }

    /*
    <summary>Handles the player's move based on the input number.</summary>
    <param name="num">0 and 1 are the top row of buttons, 2 and 3 are the bottom row of buttons.</param>
    */
    public void OnMovePressed(Attack atk)
    {
        //num always corresponds to the position in the attacks list

        //speed determines attacker
        battleData.currentPlayerMon.currentAttack = atk;
        battleData.currentEnemyMon.currentAttack = EnemyAIDecider();
 
        if(atk!=null)
        {
            AttackAnimationsPause();

            if(battleData.currentPlayerMon.currentSpeed > battleData.currentEnemyMon.currentSpeed)
            {
                mainText.text = $"{battleData.currentPlayerMon.stats.nameOfPokemon} uses {battleData.currentPlayerMon.currentAttack.nameOfMove}!";
                ImplementMoveAnimation(battleData.currentEnemyMon, battleData.currentPlayerMon, atk);
            }
            else
            {
                mainText.text = $"Enemy {battleData.currentEnemyMon.stats.nameOfPokemon} uses {battleData.currentEnemyMon.currentAttack.nameOfMove}!";
                ImplementMoveAnimation(battleData.currentPlayerMon, battleData.currentEnemyMon, battleData.currentEnemyMon.currentAttack);
            }

            //for now, no animation so immediately call 
        }
        else
        { 
            Debug.Log("Player tried to use an attack that doesn't exist");
            lastFish.EndFishing(false);
        }
    }


    Attack EnemyAIDecider()
    {
        //just decide randomly I think
        int rand = Random.Range(0, battleData.currentEnemyMon.attacks.Count);
        return battleData.currentEnemyMon.attacks[rand];
    }

    public IEnumerator OnFleeCR()
    {
        mainText.text = "You fled!";
        yield return new WaitForSeconds(1f);
        if(lastFish != null)
        {
            Debug.Log("Ending fish");
            lastFish.EndFishing(false);
        }
        //kill the minigame unceremoniously
    }

    public void OnFleePressed()
    {
        StartCoroutine(OnFleeCR());
    }

    public IEnumerator OnCatchPressedCR()
    {
        //randomly decide if the catch is successful or not based on factors like health etc.
        float range = Random.Range(0f,1f);

        float baseCatchRate = 0.1f; //with absolutely no modifiers, the catch rate is 10%

        float changePerHealthPercentageMissing = 0.01f;//at 50% health, catch rate here would be 0.6f. at 10% health its 100%. 
        //this would need to be adjusted down if extra things are added i.e. status effects.

        mainText.text = ".";
        yield return new WaitForSeconds(0.5f);
        mainText.text = "..";
        yield return new WaitForSeconds(0.5f);
        mainText.text = "...";
        yield return new WaitForSeconds(0.5f);

        if(range < baseCatchRate + (changePerHealthPercentageMissing * 
        (1 - (battleData.currentEnemyMon.currentHealth / battleData.currentEnemyMon.stats.maxHealth))) + catchModifier)
        {
            //catch successful
            mainText.text = "Success!";
            yield return new WaitForSeconds(2f);
            if(lastFish != null)
            {
                lastFish.fishData.valueMultiplier = 3; //fish caught through pokemon are worth 3x as much.
                lastFish.EndFishing(true);
            }
        }
        else
        {
            mainText.text = "It got away...";
            yield return new WaitForSeconds(2f);
            if(lastFish != null)
            {
                lastFish.EndFishing(false);
            }
        }


    }

    public void OnCatchPressed()
    {
        AttackAnimationsPause();
        StartCoroutine(OnCatchPressedCR());
    }


    void AdjustHealthbar(Pokemon pokemon)
    {
        Debug.Log(pokemon.currentHealth);
        //set the scale of the healthbar based on the pokemon's current health
        
        //adjust the healthbar scale, taking itno account its initial scale
        float healthPercent = (float)pokemon.currentHealth / (float)pokemon.stats.maxHealth;
        pokemon.healthbar.transform.localScale = new Vector3(pokemon.healthBarScaleInitial * healthPercent, 
        pokemon.healthbar.transform.localScale.y, pokemon.healthbar.transform.localScale.z);
    }

    public void OnAttacksButtonPressed()
    {
        optionsPanel.SetActive(false); ;
        attacksPanel.SetActive(true);
        currentSelectedButton.arrow.SetActive(false);   

        currentUIState = UIState.Attack;
        currentSelectedButton = attackButtons[0];
        currentSelectedButton.arrow.SetActive(true);
        descriptionPanel.SetActive(true);
        descriptionPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = currentSelectedButton.attack.description; //lol
    }

    public void OnOptionsButtonPressed()
    {
        optionsPanel.SetActive(true); ;
        attacksPanel.SetActive(false);
        currentSelectedButton.arrow.SetActive(false);   

        currentUIState = UIState.Main;
        currentSelectedButton = optionButtons[0];
        currentSelectedButton.arrow.SetActive(true);   
        descriptionPanel.SetActive(false);

    }
}
