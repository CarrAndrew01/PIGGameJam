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
        PlayOrStopAudio(true);
    }

    // Update is called once per frame
    void Update()
    {

    }
    void PlayOrStopAudio(bool play)
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
                break;
            case AreaType.Cat_Ship:
                break;
            case AreaType.Poker:
                break;
            default:
                break;
        }
    }
}
public enum AreaType
{
    Title,
    Ice_Planet,
    Lava_Planet,
    Cat_Ship,
    Poker

}
