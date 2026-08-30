using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class BackgroundImage : MonoBehaviour
{
    private Image image;

    void Awake()
    {
        image = GetComponent<Image>();
    }

    void Start()
    {
        Sprite sprite = Environment.Background;

        if (sprite != null)
            image.sprite = sprite;
    }
}
