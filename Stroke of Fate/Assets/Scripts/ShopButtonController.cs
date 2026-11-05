using UnityEngine;

public class ShopButtonController : MonoBehaviour
{
    public BallData itemSO;
    public GolfBallController golfBallController;
    [SerializeField] private logic logicScript;
    [SerializeField] private GameObject shopUI;
    public void Initialized(BallData newBallData) //fill place up with info
    {
        itemSO = newBallData;
    }

    public void OnClick()
    {
        if (itemSO == null)
        {
            Debug.LogError("Cannot equip this shop item because its BallData is not assigned.", this);
            return;
        }

        if (golfBallController == null)
        {
            Debug.LogError("Cannot equip this shop item because its GolfBallController is not assigned.", this);
            return;
        }
        logicScript.AddGold(-itemSO.shopCost);
        logicScript.UpdateIncomeText(itemSO.income);

        golfBallController.SetBallData(itemSO);
        
        gameObject.SetActive(false);
    }
    //reactivate the shop buttons when the shop is opened again
    public void ActivateShopButtons(GameObject[] shopButtons)
    {
        foreach (GameObject button in shopButtons)
        {
            button.SetActive(true);
        }
    }
}
