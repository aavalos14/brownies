using UnityEngine;
using TMPro;

public class GolfStrokeCounter : MonoBehaviour
{
    public int strokes = 0;

    public TMP_Text strokeText;

    // Call this every time the ball is hit
    public void AddStroke()
    {
        strokes++;
        UpdateStrokeText();
    }

    // Call this when the ball reaches the hole
    public void ResetStrokes()
    {
        strokes = 0;
        UpdateStrokeText();
    }

    // Update the number shown on screen
    private void UpdateStrokeText()
    {
        if (strokeText != null)
        {
            strokeText.text = "Strokes: " + strokes.ToString();
        }
    }

    private void Start()
    {
        UpdateStrokeText();
    }
}
