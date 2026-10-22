

using UnityEngine;

public class Bunker : MonoBehaviour
{
    [Header("Sand Effects")]
    [Range(0.1f, 1f)]
    [SerializeField] private float speedMultiplier = 0.75f;

    [Range(0f, 30f)]
    [SerializeField] private float maxDirectionChange = 8f;

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

        ball.ApplyBunkerEffect(
            speedMultiplier,
            maxDirectionChange
        );
    }
}
