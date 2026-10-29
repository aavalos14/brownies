using UnityEngine;
using TMPro;
public class logic : MonoBehaviour
{
    public int score;
    void Start()
    {
        score = 0;
    }
    public void AddScore(int scoreToAdd)
    {
        score += scoreToAdd;
    }
    public int GetScore()
    {
        return score;
    }
      public int stroke = 1;

    public TMP_Text strokeText;

    // Call this every time the ball is hit
    public void AddStroke()
    {
        stroke++;
        UpdateStrokeText();
    }

    // Call this when the ball reaches the hole
    public void ResetStrokes()
    {
        stroke = 1;
        UpdateStrokeText();
    }

    // Update the number shown on screen
    private void UpdateStrokeText()
    {
        if (strokeText != null)
        {
            if (stroke == 4)
            {
                strokeText.color = Color.yellow;
                strokeText.text = "Stroke: " + stroke;
            }
            else if (stroke == 5)
            {
                strokeText.color = Color.red;
                strokeText.text = "Stroke: 5";
            }
            else if (stroke > 5)
            {
                strokeText.color = Color.red;
                strokeText.text = "Stroke: 5+";
            }
            else
            {
                strokeText.color = Color.white;
                strokeText.text = "Stroke: " + stroke;
            }
        }
    }

}
