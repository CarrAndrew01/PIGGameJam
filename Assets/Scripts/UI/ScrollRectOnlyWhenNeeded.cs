using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class ScrollRectOnlyWhenNeeded : MonoBehaviour
{
    public bool checkVertical = true;
    public bool checkHorizontal = true;

    ScrollRect scrollRect;
    RectTransform content;
    RectTransform viewport;

    void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
        content = scrollRect.content;
        viewport = scrollRect.viewport != null ? scrollRect.viewport : (RectTransform)scrollRect.transform;
    }

    void LateUpdate()
    {
        if (content == null || viewport == null) return;

        // Force layout update if needed (especially with Content Size Fitter)
        Canvas.ForceUpdateCanvases();

        bool needVertical = content.rect.height > viewport.rect.height + 0.1f;
        bool needHorizontal = content.rect.width > viewport.rect.width + 0.1f;

        if (checkVertical)
            scrollRect.vertical = needVertical;

        if (checkHorizontal)
            scrollRect.horizontal = needHorizontal;

        // Optional: also hide the scrollbars completely
        if (scrollRect.verticalScrollbar != null)
            scrollRect.verticalScrollbar.gameObject.SetActive(needVertical);

        if (scrollRect.horizontalScrollbar != null)
            scrollRect.horizontalScrollbar.gameObject.SetActive(needHorizontal);
    }
}