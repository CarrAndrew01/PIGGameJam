using UnityEngine;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance { get; private set;}
    [SerializeField]
    Animator topAnimator;
    [SerializeField]
    Animator bottomAnimator;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftBracket))
        {
            topAnimator.SetTrigger("TopClose");
            bottomAnimator.SetTrigger("BottomClose");
        } 
        if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            topAnimator.SetTrigger("TopOpen");
            bottomAnimator.SetTrigger("BottomOpen");
        }       
    }
    
}
