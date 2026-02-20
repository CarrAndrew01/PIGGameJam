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
                textObject.color = new Color(textObject.color.r, textObject.color.g, textObject.color.b, textObject.color.a - 0.08f);
                yield return new WaitForFixedUpdate();
            }
        }
    }

    IEnumerator UnFadeTextCoroutine()
    {
        Debug.Log("ghtgrt");
        for(int i = 0; i < 255; i++){
            foreach (TMP_Text textObject in TextObjects)
            {
                //fade the text in
                textObject.color = new Color(textObject.color.r, textObject.color.g, textObject.color.b, textObject.color.a + 0.08f);
                yield return new WaitForFixedUpdate();
            }
        }
    }



    public void TransitionToPlanets()
    {
        Debug.Log("GoingToPlanets");
        gameObject.GetComponent<Animator>().SetBool("PlanetTransition", true);

        StartCoroutine(FadeTextCoroutine());
    }

    public void TransitionToSettings()
    {
        Debug.Log("GoingToSettings");
        gameObject.GetComponent<Animator>().SetBool("SettingsTransition", true);
        StartCoroutine(FadeTextCoroutine());
    }


    public void TransitionToMainMenuFromPlanets()
    {
        Debug.Log("GoingToMainMenu");
        gameObject.GetComponent<Animator>().SetBool("PlanetTransition", false);
        StartCoroutine(UnFadeTextCoroutine());
    }


    public void TransitionToMainMenuFromSettings()
    {
        Debug.Log("GoingToMainMenu");
        gameObject.GetComponent<Animator>().SetBool("SettingsTransition", false);
        StartCoroutine(UnFadeTextCoroutine());
    }



}
