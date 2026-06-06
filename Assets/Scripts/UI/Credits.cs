using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Credits : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    bool skip = false;
    
    public GameObject anyButtonToSkipText;

    Animator anim;

    void Start()
    {
        //very simple, fade the fader back in, show the credits, any button press exits to main menu
        if(gameObject.GetComponent<Animator>() != null)
        {
            // Fade in the fader
            anim = gameObject.GetComponent<Animator>();
        }
    }

    void Update()
    {
        // Check for any key press to exit to main menu
        if (Input.anyKeyDown && skip)
        {
          //  SceneManager.LoadScene("Title");
            anim.SetTrigger("FadeOut");
        }
    }

    public void EnableSkip()
    {
        skip = true;
    }

    public void ExitToMainMenu()
    {
        SceneManager.LoadScene("Title");
    }
}
