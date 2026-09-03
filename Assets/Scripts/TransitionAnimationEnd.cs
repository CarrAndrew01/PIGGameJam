using UnityEngine;

public class TransitionAnimationEnd : MonoBehaviour
{
    public void CompleteAnimation()
    {
        SceneSwitcher.TriggerCompleteTransition();
    }
}
