using System.Collections.Generic;
using UnityEngine;


//keeping this outside the class so I can access it anywhere
public enum EndCondition
{
    DEFAULT, //i.e this is the node if something breaks and we just need to reset
    TakeDamage,
    Timer,
    Repeats,//this behavior repeats a certain amount of times
    StageEnd, //this behavior has stages, and we want to go to the next node when the last stage ends
    Misc 
}


[System.Serializable]
/*
every node can go to other nodes, with a condition for ending. NextNode is essentially just a key-value pair that I can actually serialize
*/
public class NextNode
{
    public List<EndCondition> conditions;
    public string nextNodeName; //for readability I'd rather use the name as a string instead of the index
    //but remember to toLowerCase everything! Otherwise we WILL end up accidentally capitalizing something like a dumbass
}

[System.Serializable]
public class BehaviorNodes
{
    public string nodeName; // This is what we reference for behaviors. Instead of using enums for enemy states we'll use strings. That way you only have to define this once and then use
    //it for all the children classes of BOssBehavior.

    public List<NextNode> nextNodes; //potential next nodes to go to


    /*
    TIMER 
    */
    public float timer;//the actual timer that counts up
    public float totalTime;//how long does this behavior last until it stops

    /*
    REPEATS
    */
    public int repeatCount; //how many times this behavior repeats

    /*
    STAGES

    each behavior has stages, preventing the need far too many behavior nodes. 
    for example An attack might have to do something only at the very start of the behavior (stage 0), 
    play an animation (stage 1), and do something at the end of the attack (stage 3)

    theres only 1 set of stages that a behavior can have, and a behavior can only go to 1 NextNode through getting to the last stage

    stages SHOULD be linear, thats why they only have an index to go through them. You should probably make a new behavior node if thats too restrictive.
    */

    public List<string> stages;
    public int currentStage; 


    /*
        FUNCTIONS:
    */

    /*
    resets everything about the node. Called whenever we go to the next node or if we need to reset the current node for some reason. 
    */
    public void ResetNode()
    {
        currentStage = 0;
        timer = 0f;
        currentStage = 0;
    }
}
 

/*
General extensible script for boss behavior etc. 
all virtual functions, override with the specific script for that boss as I don't know *how* different each will be from the other
*/
public class BossBehavior : MonoBehaviour
{
    public float health;
    public Fish fish; //might be useful, lets you access stats anyway.
    public FishShadow fishShadow; //iirc we need this for stuff right?

    public List<BehaviorNodes> behaviors; //assign these in the inspector? Or here?
    public int currentNodeIndex;//

    public GameObject player; //the player, every boss is going to need this

    public Animator anim;


    public virtual void Start()
    {   
        //at default, currentNode should just be the first behaviornode
        if(behaviors.Count > 0)
        {
            currentNodeIndex = 0;
        }

        if(GetComponent<Animator>() != null)
        {
            anim = GetComponent<Animator>();
        }

        if(GameObject.FindGameObjectWithTag("Player") != null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

    }

    public virtual void Update()
    {
        //put a switch here with the different decisionNodes on the boss script
        //REMEMBER TO PUT base.Update THOUGH
 
        //TODO: Check timers

    }

    //override with boss behavior when damaged
    public virtual void OnDamaged()
    {



        
    }

    //checks all the NextNode conditions for the current Node and see if they match whatver we passed in
    //or just return the first one if we pass nothing in
    public void CheckNodesForType( out NextNode next, EndCondition endCondition = EndCondition.DEFAULT, BehaviorNodes node = null)
    {
        //this can work for any node, but usually it's going to be whatever we're currently on
        if(node == null)
        {
            node = behaviors[currentNodeIndex];
        }

        for(int i = 0; i < node.nextNodes.Count; i++)
        {
            for(int j = 0; j < node.nextNodes[i].conditions.Count; j++)
            {
                if(node.nextNodes[i].conditions[j] == endCondition)
                {
                    next = node.nextNodes[i];
                    return;
                }
            }
        }
        next = null;
    }

    public BehaviorNodes GetNextNode(string nodeName)
    {
        for(int i = 0; i < behaviors.Count; i++)
        {
            if(behaviors[i].nodeName.ToLower() == nodeName.ToLower())
            {
                return behaviors[i];
            }
        }
        return null;
    }

    
    public virtual void GoToNextNode(NextNode next)
    {
        //find the index of the next node in the behaviors list and set currentNode to that
        for(int i = 0; i < behaviors.Count; i++)
        {
            if(behaviors[i] == GetNextNode(next.nextNodeName))
            {
                behaviors[currentNodeIndex].ResetNode();
                currentNodeIndex = i;
                return;
            }
        }

        //no nodes, guess we just end it here for now
        Debug.Log("Thats the end");
    }


    /*
    Updates to the next stage of the animation, 
    */
    public virtual void StageUpdate(BehaviorNodes node)
    {
        //check if we've gone over the maximum stage and progress to the next node if we have. Otherwise, just increment the stage.
        //loop the next nodes

        node.currentStage++;
        if(node.currentStage >= node.stages.Count)
        {
            CheckNodesForType(out NextNode next, EndCondition.StageEnd, node);

            if(next != null)
            {
                GoToNextNode(next);
            }
        }
    }
}
