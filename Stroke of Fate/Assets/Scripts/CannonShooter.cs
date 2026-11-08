
using UnityEngine; // Gives access to Unity features such as GameObjects, Rigidbody2D, and Time.

public class CannonShooter : MonoBehaviour
{
    // ---------------- PROJECTILE SETTINGS ----------------

    [Header("Projectile Settings")]

    // Stores the missile prefab that the cannon will shoot.
    // This prefab should contain a Rigidbody2D component.
    public GameObject missilePrefab;

    // Determines where the missile spawns when fired.
    // Its position and rotation control the missile's starting location and direction.
    public Transform firePoint;


    // ---------------- FIRING SETTINGS ----------------

    [Header("Firing Settings")]

    // Determines how many seconds the cannon waits between shots.
    public float fireInterval = 3f;

    // Determines how fast the missile travels.
    public float missileSpeed = 6f;


    // ---------------- PRIVATE VARIABLES ----------------

    // Tracks how much time has passed since the last shot.
    private float fireTimer;


    // ---------------- UPDATE ----------------

    // Update runs once every frame.
    void Update()
    {
        // Adds the time passed since the previous frame to the timer.
        fireTimer += Time.deltaTime;

        // Checks if enough time has passed to fire another missile.
        if (fireTimer >= fireInterval)
        {
            // Calls the FireMissile function to create and launch a missile.
            FireMissile();

            // Resets the timer so the cannon starts counting toward its next shot.
            fireTimer = 0f;
        }
    }


    // ---------------- FIRE MISSILE ----------------

    // Function responsible for creating and launching the missile.
    void FireMissile()
    {
        // Checks whether the missile prefab or fire point is missing.
        // If either is missing, exit the function to prevent errors.
        if (missilePrefab == null || firePoint == null)
            return;


        // Creates a new missile in the scene.
        GameObject missile = Instantiate(
            missilePrefab,       // The prefab being created.
            firePoint.position,  // The position where the missile spawns.
            firePoint.rotation   // The rotation the missile starts with.
        );


        // Gets the Rigidbody2D component attached to the newly created missile.
        Rigidbody2D rb = missile.GetComponent<Rigidbody2D>();

        // Checks whether the missile has a Rigidbody2D component.
        if (rb != null)
        {
            // Disables gravity so the missile travels in a straight line.
            rb.gravityScale = 0f;

            // Sets the missile's movement speed and direction.
            // firePoint.up determines which direction the missile travels.
            // missileSpeed determines how fast it moves.
            rb.linearVelocity = firePoint.up * missileSpeed;
        }
    }
}
