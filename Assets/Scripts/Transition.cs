using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Transition : MonoBehaviour
{

    public List<TMP_Text> TextObjects = new();
 

    IEnumerator FadeTextCoroutine()
    {
        for(int i = 0; i < 255; i++){
            foreach (TMP_Text textObject in TextObjects)
            {
                //fade the text out
                textObject.color = new Color(textObject.color.r, textObject.color.g, textObject.color.b, textObject.color.a - 0.04f);

             }

            //the text elements are all firing at the same time so this is safe
            if(TextObjects[0].color.a <= 0f)
            {   
                break;
            }

            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator UnFadeTextCoroutine()
    {
        for(int i = 0; i < 255; i++){
            foreach (TMP_Text textObject in TextObjects)
            {
                //fade the text in
                //eh, good enough
                textObject.color = new Color(textObject.color.r, textObject.color.g, textObject.color.b, textObject.color.a + 0.01f);
            }

            //the text elements are all firing at the same time so this is safe
            if(TextObjects[0].color.a >= 1f)
            {
                break;
            }

            yield return new WaitForFixedUpdate();
        }
    }



    public void TransitionToPlanets()
    {
        gameObject.GetComponent<Animator>().SetBool("PlanetTransition", true);
        StartCoroutine(FadeTextCoroutine());
    }

    public void TransitionToSettings()
    {
        gameObject.GetComponent<Animator>().SetBool("SettingsTransition", true);
        StartCoroutine(FadeTextCoroutine());
    }


    public void TransitionToMainMenuFromPlanets()
    {
        gameObject.GetComponent<Animator>().SetBool("PlanetTransition", false);
        StartCoroutine(UnFadeTextCoroutine());
    }


    public void TransitionToMainMenuFromSettings()
    {
        gameObject.GetComponent<Animator>().SetBool("SettingsTransition", false);
        StartCoroutine(UnFadeTextCoroutine());
    }



}
