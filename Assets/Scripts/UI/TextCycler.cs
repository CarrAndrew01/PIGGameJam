using UnityEngine;
using TMPro;
using System.Collections;

public class TextCycler : MonoBehaviour
{
    public string[] text;
    public TextMeshProUGUI uiText;
    int stringCount;
    int currentIndex = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        uiText = GetComponent<TextMeshProUGUI>();
        uiText.text = text[0];
        stringCount = text.Length;

        StartCoroutine("CycleText");
    }

    // Update is called once per frame
    void Update()
    {

    }
    IEnumerator CycleText()
    {
        while (true)
        {
        uiText.text = text[currentIndex];

        currentIndex++;
        if(currentIndex >= stringCount)
            {
                currentIndex = 0;
            }
        yield return new WaitForSeconds(0.5f);
        }
    }
        
}
