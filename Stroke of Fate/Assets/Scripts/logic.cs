using UnityEngine;
using TMPro;
public class logic : MonoBehaviour
{
    [Header("Score")]
    private int score;
    [SerializeField] private TMP_Text scoreText;
    void Start()
    {
        score = 0;
        UpdateScoreText();
    }
    public void AddScore(int scoreToAdd)
    {
        score += scoreToAdd;
        UpdateScoreText();
    }
    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }
    public int GetScore()
    {
        return score;
    }
    [Header("Stroke")]
    private int stroke = 1;

    [SerializeField] private TMP_Text strokeText;

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
    [Header("Hole")]
    private int hole = 1;
    [SerializeField] private TMP_Text holeText;
    public void AddHole()
    {
        hole++;
        UpdateHoleText();
    }
    private void UpdateHoleText()
    {
        if (holeText != null)
        {
            holeText.text = "Hole #" + hole;
        }
    }

}
