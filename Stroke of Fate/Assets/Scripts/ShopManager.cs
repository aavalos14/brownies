using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private GameObject shopUI;
    public void PlayNextLevel()
    {
        shopUI.SetActive(false);
    }
}
