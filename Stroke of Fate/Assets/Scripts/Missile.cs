using UnityEngine;

public class Missile : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerLastPosition player =
            collision.gameObject.GetComponent<PlayerLastPosition>();

        if (player != null)
        {
            player.ResetPosition();
        }

        Destroy(gameObject);
    }
}
