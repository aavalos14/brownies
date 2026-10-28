using NUnit.Framework.Constraints;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopButton : MonoBehaviour
{
    public BallData itemSO;
    public TMP_Text PowerText;
    public TMP_Text CostText;
    public Image itemImage;
    private int Cost;
 public void Initialized(BallData newBallData, int Cost)
    {
        //filll place up with info
itemSO = newBallData;
itemImage.sprite = itemSO.ballSprite;
PowerText.text = itemSO.ballName;
this.Cost = Cost;
CostText.text = Cost.ToString(); 
    }
}
