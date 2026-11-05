

using UnityEngine;

public class Bunker : MonoBehaviour
{
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

        ball.ApplyBunkerEffect();
    }
}
