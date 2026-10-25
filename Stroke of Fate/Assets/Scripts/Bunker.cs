

using UnityEngine;

public class Bunker : MonoBehaviour
{
    [Header("Sand Effects")]
    [Range(0.1f, 1f)]
    public float speedMultiplier = 0.5f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        GolfBallController ball =
            other.GetComponent<GolfBallController>();

        if (ball == null)
        {
            ball = other.GetComponentInParent<GolfBallController>();
        }

        if (ball == null)
            return;

        ball.ApplyBunkerEffect(speedMultiplier);
    }
}
