using UnityEngine;

public class GolfHole : MonoBehaviour
{
    public GolfStrokeCounter strokeCounter;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (strokeCounter != null)
            {
                strokeCounter.ResetStrokes();
            }
        }
    }
}