using UnityEngine;

public class CannonShooter : MonoBehaviour
{
    [Header("Projectile Settings")]
    public GameObject missilePrefab;
    public Transform firePoint;

    [Header("Firing Settings")]
    public float fireInterval = 3f;
    public float missileSpeed = 6f;

    private float fireTimer;

    void Update()
    {
        fireTimer += Time.deltaTime;

        if (fireTimer >= fireInterval)
        {
            FireMissile();
            fireTimer = 0f;
        }
    }

    void FireMissile()
    {
        if (missilePrefab == null || firePoint == null)
            return;

        GameObject missile = Instantiate(
            missilePrefab,
            firePoint.position,
            firePoint.rotation
        );

        Rigidbody2D rb = missile.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.linearVelocity = firePoint.up * missileSpeed;
        }
    }
}