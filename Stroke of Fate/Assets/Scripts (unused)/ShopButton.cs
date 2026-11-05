using UnityEngine;

public class ShopButton : MonoBehaviour
{
    public BallData itemSO;
    public GolfBallController golfBallController;
    public GameObject shopButton;

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

        golfBallController.SetBallData(itemSO);
        shopButton.SetActive(false);
    }
}
