using UnityEngine;
using UnityEngine.UIElements;
using Febucci.TextAnimatorForUnity;
using TMPro; // <- import Text Animator's namespace

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI typewriter;  // Assign in Inspector


    



    public void DisplayNewText(string newDialogue)
    {
        typewriter.text = newDialogue;
    }

  

}

