using UnityEngine;

public class AmbientSoundGenerator : MonoBehaviour
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AudioManager.playSound?.Invoke("Water");
        AudioManager.playSound?.Invoke("Ice_Planet_Music");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
