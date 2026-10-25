using UnityEngine;

[CreateAssetMenu(fileName = "CharmData", menuName = "Scriptable Objects/CharmData")]
public class CharmData : ScriptableObject
{
    public string charmName;
    public Sprite charmIcon;
    [TextArea] public string description;
    
    // Type of trigger, e.g., OnShotTaken, OnBounce, OnHoleComplete
    //public OnBounce triggerType;
    public float modifierValue; // e.g., +2 score, 1.5x multiplier
}
