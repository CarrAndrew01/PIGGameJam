using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DebugTools : MonoBehaviour
{

    private static DebugTools _instance;

    public static DebugTools Instance
    {
        get
        {
            if (_instance == null)
            {
                var go = new GameObject("DebugTools");
                _instance = go.AddComponent<DebugTools>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    public GameObject debugMenu;
    public GameObject debugMenuInstantiate;

    void Update()
    {

        //F1 reloads the scene
        if (Input.GetKeyDown(KeyCode.F1))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (Input.GetKeyDown(KeyCode.F11))
        {
            //bring up the debug tool
            if(debugMenu != null)
            {
                debugMenu.SetActive(!debugMenu.activeInHierarchy);
                
                TMP_InputField inputField = debugMenu.GetComponent<TMP_InputField>();

                if(inputField != null){
                    inputField.Select();
                    inputField.ActivateInputField();
                }

                // Disable/enable the player UI buttons when the debug menu is toggled
                if (debugMenu.activeInHierarchy)
                {
                    MenuManager.Instance.enabled = false;
                    //this isn't great but I'd have to find all the inputs and disable otherwise and I'd rather not do that
                    //so this works as a dev solution since this isn't technically a player-forward feature anyway
                    Time.timeScale = 0f; //reset time scale to normal when opening debug menu

                } else
                {
                    Time.timeScale = 1f; //reset time scale to normal when closing debug menu

                    MenuManager.Instance.enabled = true;
                }
            }
            else
            {
                Debug.Log("Nope, no debug menu here, gotta put it in manually as I couldn't work out canvas things");

            }
        }

        if(debugMenu != null && debugMenu.activeInHierarchy)
        {
            
            TMP_InputField inputField = debugMenu.GetComponent<TMP_InputField>();

            if (Input.GetKeyDown(KeyCode.Return) && inputField != null)
            {
                string input = inputField.text;
                Debug.Log(input);
                inputField.text = string.Empty; //does this work?

                List<string> inputs = input.Split(" ").ToList();

                // Re-enable the player UI buttons when a command is entered
                MenuManager.Instance.enabled = true;

                // Disable the input field to prevent multiple commands being entered at once
                debugMenu.SetActive(false);
                    Time.timeScale = 1f; //reset time scale to normal when closing debug menu

                switch (inputs[0])
                {
                    case "giveall" : 
                    //give us all of everything everything

                    //first, set the inventory size to big enough no matter what
                        GameManager.Instance.playerStats.currentStats[StatType.fishStorage] = 1000f; //big inventory

                        GameManager.Instance.HasSeenIntro = true; //skip the intro

                        foreach (Fish fish in GameManager.Instance.allFish)
                        {
                            CaughtFish cf = new(fish, UnityEngine.Random.Range(fish.minWeight, fish.maxWeight) * GameManager.GetPlayerStat(StatType.fishWeight), "debug");
                            GameManager.AddFishToInventory(cf);                  
                        }

                        foreach(Upgrade upg in GameManager.Instance.allUpgrades)
                        {
                            GameManager.AddUpgrade(upg); //give us all upgrades
                        }
                        
                        break;

                    case "removeall" :
                        //completely nukes our inventory
                        GameManager.Instance.playerInventory.ClearInventory();
                        GameManager.Instance.playerStats.ClearUpgrades();
                        break;

                    case "givemoney" :
                        if(inputs.Count()> 1)
                        {   
                            GameManager.Instance.playerInventory.money += float.Parse(inputs[1]); //give us a specific amount of money
                        }
                        else
                        {
                            GameManager.Instance.playerInventory.money += 1000f; //give us a specific amount of money
                        }

                        break;

                    case "cleardata" :
                        //clear player data
                        GameManager.ClearPlayerData();
                        break;

                    case "givefish" :
                    //give us a specific thing
                        if(inputs.Count() > 1)
                        {
                            //first thing, 
                            int number = 1;

                            //if player specifies a number in the THIRD string, you get that much, otherwise sets count to 1
                            if(inputs.Count() > 2)
                            {
                                int.TryParse(inputs[2], out number);
                            }

                            //the second string specifies what fish we want
                            Fish fish = GameManager.Instance.GetFish(inputs[1]);

                            if (fish != null)
                            {
                                for(int i = 0; i < number; i++)
                                {
                                    CaughtFish cf = new(fish, UnityEngine.Random.Range(fish.minWeight, fish.maxWeight) * GameManager.GetPlayerStat(StatType.fishWeight), "debug");
                                    GameManager.AddFishToInventory(cf);
                                    
                                }
                            }
                        }
                        break;

                    case "giveupgrade" :
                        if(inputs.Count() > 1)
                        {
                            //the second string specifies what upgrade we want
                            Upgrade upg = GameManager.Instance.GetUpgrade(inputs[1]);

                            if (upg != null)
                            {
                                GameManager.AddUpgrade(upg);
                            }
                        }

                        break;


                }
            }
        }
    }
}
