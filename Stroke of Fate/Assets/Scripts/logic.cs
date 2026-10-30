using TMPro;
using UnityEngine;

public class logic : MonoBehaviour
{
    private const string GoldBalanceKey = "StrokeOfFate_GoldBalance";
    private const string LastGoldRewardKey = "StrokeOfFate_LastGoldReward";
    private const int BaseGoldReward = 100;
    private const int GoldBonusPerStrokeUnderPar = 25;
    private const int GoldPenaltyPerStrokeOverPar = 10;
    private const int MinimumGoldReward = 25;

    public int score;
    public int stroke;
    public int par = 5;
    public TMP_Text strokeText;
    public TMP_Text goldBalanceText;

    public int LastGoldReward { get; private set; }
    public int TotalGold { get; private set; }
    public string LastHoleResult => lastHoleResult;

    private TMP_Text scoreText;
    private string lastHoleResult = "Par";
    private bool holeCompleted;
    private bool hasCompletedAnyHole;

    private void Start()
    {
        score = 0;
        stroke = 0;
        TotalGold = PlayerPrefs.GetInt(GoldBalanceKey, 0);
        LastGoldReward = PlayerPrefs.GetInt(LastGoldRewardKey, 0);
        holeCompleted = false;
        hasCompletedAnyHole = false;
        UpdateStrokeText();
        UpdateScoreText();
        UpdateGoldBalanceText();
    }

    /// <summary>
    /// Adds a legacy point amount to the running score.
    /// </summary>
    public void AddScore(int scoreToAdd)
    {
        score += scoreToAdd;
        UpdateScoreText();
    }

    public int GetScore()
    {
        return score;
    }

    /// <summary>
    /// Connects the score label so stroke changes can update the live result.
    /// </summary>
    public void UpdateScoreDisplay(TMP_Text display)
    {
        scoreText = display;
        UpdateScoreText();
    }

    /// <summary>
    /// Records one shot and refreshes both stroke and live over-par scoring.
    /// </summary>
    public void AddStroke()
    {
        stroke++;
        UpdateStrokeText();
        UpdateScoreText();
    }

    /// <summary>
    /// Finalizes this hole's strokes relative to par and adds them to the round score.
    /// </summary>
    public void CompleteHole(TMP_Text display)
    {
        if (holeCompleted)
        {
            return;
        }

        scoreText = display;
        int strokesOverPar = stroke - Mathf.Max(1, par);
        score -= strokesOverPar;
        lastHoleResult = GetHoleResultName(strokesOverPar);
        LastGoldReward = CalculateGoldReward(strokesOverPar);
        TotalGold += LastGoldReward;
        PlayerPrefs.SetInt(GoldBalanceKey, TotalGold);
        PlayerPrefs.SetInt(LastGoldRewardKey, LastGoldReward);
        PlayerPrefs.Save();
        UpdateGoldBalanceText();
        holeCompleted = true;
        hasCompletedAnyHole = true;
        UpdateScoreText();
    }

    /// <summary>
    /// Begins the next hole's stroke count without clearing the round score.
    /// </summary>
    public void ResetStrokes()
    {
        stroke = 0;
        holeCompleted = false;
        UpdateStrokeText();
        UpdateScoreText();
    }

    private int CalculateGoldReward(int strokesOverPar)
    {
        int reward = BaseGoldReward;
        if (strokesOverPar < 0)
        {
            reward += -strokesOverPar * GoldBonusPerStrokeUnderPar;
        }
        else
        {
            reward -= strokesOverPar * GoldPenaltyPerStrokeOverPar;
        }

        return Mathf.Max(MinimumGoldReward, reward);
    }


    private void UpdateGoldBalanceText()
    {
        if (goldBalanceText != null)
        {
            goldBalanceText.text = $"GOLD: {TotalGold}";
        }
    }

    private void UpdateStrokeText()
    {
        if (strokeText == null)
        {
            return;
        }

        int configuredPar = Mathf.Max(1, par);
        strokeText.text = $"Strokes: {stroke}";
        strokeText.color = stroke > configuredPar
            ? Color.red
            : stroke == configuredPar
                ? Color.yellow
                : Color.white;
    }

    private void UpdateScoreText()
    {
        if (scoreText == null)
        {
            return;
        }

        if (holeCompleted || (hasCompletedAnyHole && stroke == 0))
        {
            scoreText.text = $"Score: {FormatScore(score)} | Last: {lastHoleResult}";
            return;
        }

        int configuredPar = Mathf.Max(1, par);
        if (stroke > 0)
        {
            int strokesOverPar = stroke - configuredPar;
            int liveScore = score - strokesOverPar;
            string liveResult = GetHoleResultName(strokesOverPar);
            scoreText.text = $"Score: {FormatScore(liveScore)} | Current: {liveResult} ({stroke})";
        }
        else
        {
            scoreText.text = $"Score: {FormatScore(score)} | Strokes: 0/{configuredPar}";
        }
    }

    private string GetHoleResultName(int strokesOverPar)
    {
        switch (strokesOverPar)
        {
            case -3: return "Albatross";
            case -2: return "Eagle";
            case -1: return "Birdie";
            case 0: return "Par";
            case 1: return "Bogey";
            case 2: return "Double Bogey";
            case 3: return "Triple Bogey";
            case 4: return "Quadruple Bogey";
            case 5: return "Quintuple Bogey";
            default:
                return strokesOverPar > 5
                    ? $"{strokesOverPar} Over Par"
                    : $"{Mathf.Abs(strokesOverPar)} Under Par";
        }
    }

    private string FormatScore(int value)
    {
        if (value > 0)
        {
            return $"+{value}";
        }

        return value == 0 ? "E" : value.ToString();
    }
}
