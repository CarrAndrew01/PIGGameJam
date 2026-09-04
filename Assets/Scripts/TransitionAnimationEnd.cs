using UnityEngine;

public class TransitionAnimationEnd : MonoBehaviour
{
    // This script handles the transition animations starting and finishing
    // When they finish they fire off these events which allow other things to only happen when transitions are done
    public void OpenStart()
    {
          Debug.Log("starting open");
    }
    public void OpenFinish()
    {
        Debug.Log("finishing open");
    }
    public void CloseStart()
    {
        Debug.Log("starting close");
    }
    public void CloseFinish()
    {
        Debug.Log("finishing close");
    }
}
