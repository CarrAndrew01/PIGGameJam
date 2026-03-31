using UnityEngine;

//this represents the particular battle thats taking place, its where field effects etc. would be put
//yeah i'm just using this an excuse to make pokemon battles, I might do something with all this on another project at some point
[CreateAssetMenu(fileName = "New Attack", menuName = "Attack")]
public class Attack : ScriptableObject
{
    public string nameOfMove; 
    public string description;
    public string animationTrigger;

    //I've decided to do it like this instead of using enums or something
    public int damage = -10000; //default is -10000 just in case we want something that functions like an attack but does 0 damage or a negative idk
    public int healAmount = -10000; //if this is set to something other than -10000, it will heal the user for that amount
    public int accuracy = 100; //chance to hit, from 0 to 100

    public int AnimationReturner()
    {
        switch(animationTrigger)
        {
            case "ReelIn":
                Debug.Log("reel in used");
                return 0;
            case "Wrestle":
                return 1;
            case "Relax":
                return 2;
            case "FakeRelax":
                return 3;
            case "FishAttack":
                return 4;
            case "FishRelax":
                return 5;
            default:
                return -1; //if this is -1, the animation will be skipped
        }
    }

}

 