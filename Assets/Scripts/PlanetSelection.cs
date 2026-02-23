using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using UnityEngine.InputSystem;



[Serializable]
public class Planet
{
    public List<GameObject> planetSprites;

    public GameObject arrow;
    public GameObject name;
    public GameObject highlight;

    public void ExtraChange(bool enable)
    {
        arrow.SetActive(enable);
        name.SetActive(enable);
        highlight.SetActive(enable);
    }

}


public class PlanetSelection : MonoBehaviour
{
    public static PlanetSelection Instance { get; private set; }

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

    }


    public int currentPlanetIndex = 0; //index in the list of the planet

    [SerializeField]
    public List<Planet> planets; 
    //decided not to make an int that keeps track of all this
    

    public InputActionReference leftRight; // button input to reel the hook upward
    public InputActionReference Interact; // button input to reel the hook upward


    void PlanetChange(int disable, int enable)
    {
        planets[disable].ExtraChange(false); 
        planets[enable].ExtraChange(true); 
    }

    public void OnHoverNew(GameObject go)
    {
        //can we do gameobject comparison?
        for(int i = 0; i < planets.Count; i++)
        { 
            if (go == planets[i].planetSprites[0])
            {
                PlanetChange(currentPlanetIndex, i);
                currentPlanetIndex = i;
            }
        }
    }

    

    void Update()
    {
        if (leftRight.action.triggered)
        {
            Vector2 movement = leftRight.action.ReadValue<Vector2>();


            if (movement.x > 0.5f)
            {

                int previousIndex = currentPlanetIndex;

                if(currentPlanetIndex + 1 < planets.Count)
                {
                    currentPlanetIndex++;
                }

                PlanetChange(previousIndex, currentPlanetIndex);
            }
            else if (movement.x < -0.5f)
            {
                int previousIndex = currentPlanetIndex;

                if(currentPlanetIndex - 1 >= 0)
                {
                    currentPlanetIndex--;
                }

                PlanetChange(previousIndex, currentPlanetIndex);
            }

        }

        if(Interact.action.triggered)
        { 
            planets[currentPlanetIndex].planetSprites[0].GetComponent<Button>().onClick.Invoke();
        }
    }  
}
