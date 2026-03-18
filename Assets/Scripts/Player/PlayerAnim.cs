using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
    public Animator anim;

    private void OnEnable()
    {
        Bobber.BobberReturned += TriggerAnim;
        Bobber.BobberReturning += TriggerAnim;
        Fishing.OnCast += TriggerAnim;
        Fishing.OnEquip += TriggerAnim;
        Stardew.OnCatching += TriggerAnim;
    }

    private void OnDisable()
    {
        Bobber.BobberReturned -= TriggerAnim;
        Bobber.BobberReturning -= TriggerAnim;
        Fishing.OnCast -= TriggerAnim;
        Fishing.OnEquip -= TriggerAnim;
        Stardew.OnCatching -= TriggerAnim;
    }

    public void TriggerAnim(string t)
    {
        anim.SetTrigger(t);
    }
}
