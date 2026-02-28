using UnityEngine;

public class AmbientSoundGenerator : MonoBehaviour
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AudioManager.playSound?.Invoke("Water");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
