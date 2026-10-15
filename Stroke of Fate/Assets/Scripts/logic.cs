using UnityEngine;

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
}
