using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// <para>Custom ScrollRect that allows temporarily suppressing the automatic scroll position reset that occurs when the content layout changes.
/// Provided by BillyMFT on unity forums: https://discussions.unity.com/t/scrollview-jumps-to-the-top-when-items-are-added/319806/3</para>
/// <para>Usage:</para>
/// 
/// <code>
/// yourScrollRect.SuppressNextLayoutJump();
/// someGOInScrollrectContent.SetActive(true);
/// </code>
/// </summary>

public class StableScrollRect : ScrollRect
{
    private bool _suppressLayoutJump = false;
    private Vector2 _savedPosition;

    public void SuppressNextLayoutJump()
    {
        _suppressLayoutJump = true;
        _savedPosition = normalizedPosition;
    }

    public override void SetLayoutVertical()
    {
        if (_suppressLayoutJump)
        {
            base.SetLayoutVertical(); // Still update layout
            normalizedPosition = _savedPosition; // Restore position
            _suppressLayoutJump = false;
        }
        else
        {
            base.SetLayoutVertical();
        }
    }
}
