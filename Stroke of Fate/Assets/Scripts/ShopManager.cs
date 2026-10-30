using TMPro;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private GameObject shopUI;
    [SerializeField] private TMP_Text goldRewardText;

    private void Start()
    {
        if (shopUI == null)
        {
            return;
        }

        if (goldRewardText == null)
        {
            Transform rewardTransform = shopUI.transform.Find("GoldRewardText");
            if (rewardTransform != null)
            {
                goldRewardText = rewardTransform.GetComponent<TMP_Text>();
            }
        }
    }

    /// <summary>
    /// Shows the completed-hole result, gold earned, and persistent wallet balance.
    /// </summary>
    public void ShowHoleReward(logic roundLogic)
    {
        if (roundLogic == null)
        {
            return;
        }

        if (goldRewardText != null)
        {
            goldRewardText.text =
                $"<size=80%>HOLE COMPLETE</size>\n{roundLogic.LastHoleResult}\n<color=#F4C542>+{roundLogic.LastGoldReward} GOLD</color>\nTotal Gold: {roundLogic.TotalGold}";
        }
        else
        {
            Debug.LogWarning("ShopManager could not find the GoldRewardText label under ShopUI.");
        }
    }

    /// <summary>
    /// Hides the shop and reward screen when the player continues.
    /// </summary>
    public void PlayNextLevel()
    {
        if (shopUI != null)
        {
            shopUI.SetActive(false);
        }
    }
}
