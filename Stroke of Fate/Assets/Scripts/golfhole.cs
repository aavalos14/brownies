using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class golfhole : MonoBehaviour
{
    private bool ballCaptured;
    [SerializeField] private logic logic;
    [SerializeField] private TMP_Text scoreText;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (ballCaptured || !other.CompareTag("Player"))
        {
            return;
        }

        Rigidbody2D ballBody = other.attachedRigidbody;
        if (ballBody == null)
        {
            return;
        }

        ballCaptured = true;
        logic.AddScore(1); // Increment score by 1 when the ball enters the hole
        scoreText.text = "Score: " + logic.GetScore().ToString();
        ballBody.position = new Vector2(transform.position.x, transform.position.y);
        ballBody.linearVelocity = Vector2.zero;
        ballBody.angularVelocity = 0f;

        GolfBallController ballController = ballBody.GetComponent<GolfBallController>();
        if (ballController != null)
        {
            ballController.enabled = false;
        }

        ballBody.bodyType = RigidbodyType2D.Static;
    }
}


