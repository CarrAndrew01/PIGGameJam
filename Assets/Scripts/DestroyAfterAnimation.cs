using UnityEngine;

public class DestroyAfterAnimation : MonoBehaviour
{
    public void DestroySelf()
    {
        Destroy(gameObject.transform.parent.transform.gameObject); ;
    }
}
