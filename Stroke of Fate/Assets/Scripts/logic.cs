using UnityEngine;
using TMPro;
public class logic : MonoBehaviour
{
    public LevelManager levelManager;
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
    public int stroke = 1;

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
    [Header("Bird")]
    [SerializeField] private TMP_Text BirdText;
    [SerializeField] private TMP_Text BirdScoreText;
    [SerializeField] private GameObject BirdTextObject;
    public void UpdateBirdText()
    {
        BirdTextObject.SetActive(true);
        if (levelManager.par - stroke == 0)
        {
            BirdText.text = "Par";
            BirdScoreText.text = "+1000";
            BirdText.color = Color.green;
            BirdScoreText.color = Color.green;
        }
        else if (levelManager.par - stroke == 1)
        {
            BirdText.text = "Birdie!";
            BirdScoreText.text = "+2000";
            BirdText.color = Color.yellow;
            BirdScoreText.color = Color.yellow;
        }
        else if (levelManager.par - stroke == 2)
        {
            BirdText.text = "Eagle!!";
            BirdScoreText.text = "+3000";
            BirdText.color = Color.orange;
            BirdScoreText.color = Color.orange;
        }
        else if (levelManager.par - stroke >= 3)
        {
            BirdText.text = "Albatross!!!";
            BirdScoreText.text = "+4000";
            BirdText.color = Color.red;
            BirdScoreText.color = Color.red;
        }
        else
        {
            BirdText.text = "Bogey";
            BirdScoreText.text = "+0";
            BirdText.color = Color.blue;
            BirdScoreText.color = Color.blue;
        }

    }
    public void RemoveBirdText()
    {
        BirdTextObject.SetActive(false);
    }
}
