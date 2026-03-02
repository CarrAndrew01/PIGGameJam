using System.Diagnostics;
using UnityEngine;

public class AmbientSoundGenerator : MonoBehaviour
{
    public AreaType areaType;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayOrStopAudio(true);
    }
    private void OnDisable()
    {
        // AudioManager.stopSound?.Invoke("Water");
        // AudioManager.stopSound?.Invoke("Ice_Planet_Music");
        PlayOrStopAudio(false);
    }

    // Update is called once per frame
    void Update()
    {

    }
    void PlayOrStopAudio(bool play)
    {
        if (play)
        {
            switch (areaType)
            {
                case AreaType.Title:
                    break;
                case AreaType.Ice_Planet:
                    AudioManager.playSound?.Invoke("Ice_Planet_Music");
                    AudioManager.playSound?.Invoke("Water");
                    break;
                case AreaType.Lava_Planet:
                    AudioManager.playSound?.Invoke("Lava_Planet_Music");
                    AudioManager.playSound?.Invoke("Lava_Bubbling");
                    break;
                case AreaType.Bio_Planet:
                    AudioManager.playSound?.Invoke("Bio_Planet_Music");
                    break;
                case AreaType.Cloud_Planet:
                    AudioManager.playSound?.Invoke("Cloud_Planet_Music");
                    break;
                case AreaType.Cat_Ship:
                    AudioManager.playSound?.Invoke("Cat_Ship_Music");
                    break;
                case AreaType.Poker:
                    AudioManager.playSound?.Invoke("Poker_Music");
                    break;
                default:
                    break;
            }
        }
        else
        {
            switch (areaType)
            {
                case AreaType.Title:
                    break;
                case AreaType.Ice_Planet:
                    AudioManager.stopSound?.Invoke("Ice_Planet_Music");
                    AudioManager.stopSound?.Invoke("Water");
                    break;
                case AreaType.Lava_Planet:
                    AudioManager.stopSound?.Invoke("Lava_Planet_Music");
                    AudioManager.stopSound?.Invoke("Lava_Bubbling");
                    break;
                case AreaType.Bio_Planet:
                    AudioManager.stopSound?.Invoke("Bio_Planet_Music");
                    break;
                case AreaType.Cloud_Planet:
                    AudioManager.stopSound?.Invoke("Cloud_Planet_Music");
                    break;
                case AreaType.Cat_Ship:
                    AudioManager.stopSound?.Invoke("Cat_Ship_Music");
                    break;
                case AreaType.Poker:
                    AudioManager.stopSound?.Invoke("Poker_Music");
                    break;
                default:
                    break;
            }
        }
    }
}
public enum AreaType
{
    Title,
    Ice_Planet,
    Lava_Planet,
    Bio_Planet,
    Cloud_Planet,
    Cat_Ship,
    Poker

}
