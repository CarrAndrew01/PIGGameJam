using UnityEngine;

public class WhaleBoss : BossBehavior
{


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
    }


    // Update is called once per frame

    //BEHAVIOR DESCRIPTION:
    //boss has 3 main moves for now
    //1. Underwater Blowhole: Blasts a massive water spout at the player. Divided into a follow behavior and an attack behavior
    //2. Underwater Headbutt: A rapid string of headbutts out of the water. Much smaller attack than the blowhole. Divided into a follow behavior and an attack behavior
    //3. Icebergs: Not really an "attack", but the whale disappears for a second and bunch of icebergs will float to the surface.  
    //4. Puts head up at one of the sides of the screen and rams sideways in a straight line. "charges up" while staying level with the player,
    //  then charges (2 behaviors), run it into an iceberg to win
    public override void Update()
    {
        base.Update();

        switch (behaviors[currentNodeIndex].nodeName.ToLower())
        {
            case "headbuttfollow" :
            //follows player
            FollowPlayer();


            break;

            case "blowholefollow" :
            
            break;
            
            case "headbuttattack" :
            //when we get here, 
            HeadbuttAttack();
            break;

            case "blowholeattack" :
            break;
            
            case "icebergs" :
            break;

            case "chargeup" :
            break;

            case "chargeattack" :
            break;

        }
    }




    public void FollowPlayer()
    {
        //physics or transform based movement?
        //physics looks nicer right
        
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, 1f * Time.deltaTime); //temp

        if(Vector2.Distance(transform.position, player.transform.position) < 0.001f)
        {
            //we're close enough to the player, go to the attack node
            CheckNodesForType(out NextNode next, EndCondition.Misc);
            if(next != null)
            {
                GoToNextNode(next);
            }
        }

        //we're close enough to the player, go to the attack node
    }

    public void HeadbuttAttack()
    {

        switch (behaviors[currentNodeIndex].currentStage)
        {
            case 0: //begin
            //just starting the headbutt. Play the animation
                Debug.Log("Stage 1");
                StageUpdate(behaviors[currentNodeIndex]);
            break;

            case 1: //begin
            //just starting the headbutt. Play the animation
                Debug.Log("Stage 2");
                StageUpdate(behaviors[currentNodeIndex]);
            break;

            case 2: //begin
            //just starting the headbutt. Play the animation
                Debug.Log("Stage 3");
                StageUpdate(behaviors[currentNodeIndex]);
            break;
        }
    }


    //called from all of the attacks at the end, either from an animation or a function, just increments the stage up
    //and goes to the next node if the stage is the final one
    public void EndOfAttack()
    {


        

    }
}
