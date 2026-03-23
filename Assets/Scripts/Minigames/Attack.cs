using UnityEngine;

//this represents the particular battle thats taking place, its where field effects etc. would be put
//yeah i'm just using this an excuse to make pokemon battles, I might do something with all this on another project at some point
[CreateAssetMenu(fileName = "New Attack", menuName = "Attack")]
public class Attack : ScriptableObject
{
    public string nameOfMove; 
    public string description;

    //I've decided to do it like this instead of using enums or something
    public int damage = -10000; //default is -10000 just in case we want something that functions like an attack but does 0 damage or a negative idk
    public int healAmount = -10000; //if this is set to something other than -10000, it will heal the user for that amount
    public int healEnemyAmount = -10000; //if this is set to something other than -10000, it will heal the enemy for that amount, this is for moves like leech seed or whatever
    public int accuracy = 100; //chance to hit, from 0 to 100
}