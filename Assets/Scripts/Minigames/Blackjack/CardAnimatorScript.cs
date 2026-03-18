using UnityEngine;

public class CardAnimatorScript : MonoBehaviour
{
    Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        BlackjackScript.onCardsSpawn += AnimateCard;
    }
    private void OnDisable()
    {
        BlackjackScript.onCardsSpawn -= AnimateCard;
    }

    // Update is called once per frame
    void Update()
    {
        // animator.SetTrigger("Wiggle");
    }
    void AnimateCard()
    {
        if (animator != null)
        {
            animator.SetTrigger("Wiggle");
        }
    }
}
