using UnityEngine;

[CreateAssetMenu(fileName = "BallData", menuName = "Scriptable Objects/BallData")]
public class BallData : ScriptableObject
{
    [Header("General Info")]
    public string ballName;
    public Sprite ballSprite;
    [TextArea] public string description;
    public bool canBePurchased = true;
    public int shopCost = 2;
    [Header("Stats")]
    public float power = 1f; // Multiplier for shot force
    public float bounciness = 0f; // Additional velocity added after hitting a surface
    public float traction = 0.5f; // Bunker friction multiplier
    // public float magnetism = 0f; // Units away the ball can be pulled towards the hole when near it
    // public int resists = 0; // Number of times the ball can resist obstacles
    // public int income = 5; // Amount of in-game currency earned per hole with this ball
    // public bool allowMulligans = false; // Whether the ball allows for mulligans
    // public int mulliganCost = 0; // Cost of a mulligan for this ball
}
