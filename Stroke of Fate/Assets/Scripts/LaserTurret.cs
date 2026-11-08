


using UnityEngine; // Gives access to Unity features like Physics2D, Transform, and Time.

// Makes sure this GameObject has a LineRenderer component.
[RequireComponent(typeof(LineRenderer))]

public class LaserTurret : MonoBehaviour
{
    // ---------------- LASER SETTINGS ----------------

    [Header("Laser Settings")]

    // Maximum distance the laser can travel.
    [SerializeField] private float laserLength = 15f;

    // Thickness of the laser beam.
    [SerializeField] private float laserWidth = 0.15f;

    // Determines which layers the laser can detect.
    // ~0 means all layers are included by default.
    [SerializeField] private LayerMask collisionLayers = ~0;


    // ---------------- LASER APPEARANCE ----------------

    [Header("Laser Appearance")]

    // Color of the laser beam.
    [SerializeField] private Color laserColor = Color.red;


    // ---------------- LASER TIMING ----------------

    [Header("Laser Timing")]

    // How long the laser stays turned on.
    [SerializeField] private float activeDuration = 3f;

    // How long the laser stays turned off.
    [SerializeField] private float inactiveDuration = 2f;


    // ---------------- PRIVATE VARIABLES ----------------

    // Stores the LineRenderer component.
    private LineRenderer lineRenderer;

    // Tracks whether the laser is currently firing.
    private bool laserActive = true;

    // Prevents the ball from being reset repeatedly while touching the laser.
    private bool ballWasHit;

    // Tracks how much time has passed in the current laser state.
    private float timer;


    // ---------------- AWAKE ----------------

    private void Awake()
    {
        // Gets the LineRenderer component attached to this GameObject.
        lineRenderer = GetComponent<LineRenderer>();

        // Makes the LineRenderer use world coordinates instead of local coordinates.
        lineRenderer.useWorldSpace = true;

        // Sets the thickness at the beginning of the laser.
        lineRenderer.startWidth = laserWidth;

        // Sets the thickness at the end of the laser.
        lineRenderer.endWidth = laserWidth;

        // Sets the color at the beginning of the laser.
        lineRenderer.startColor = laserColor;

        // Sets the color at the end of the laser.
        lineRenderer.endColor = laserColor;

        // Tells the LineRenderer that it needs two positions to draw a line.
        lineRenderer.positionCount = 2;
    }


    // ---------------- UPDATE ----------------

    private void Update()
    {
        // Adds the time passed since the previous frame to the timer.
        timer += Time.deltaTime;

        // Checks which duration should be used.
        // If the laser is active, use activeDuration.
        // Otherwise, use inactiveDuration.
        float duration = laserActive
            ? activeDuration
            : inactiveDuration;

        // Checks if enough time has passed to switch laser states.
        if (timer >= duration)
        {
            // Resets the timer back to zero.
            timer = 0f;

            // Switches the laser from active to inactive or vice versa.
            laserActive = !laserActive;

            // Allows the ball to be hit again during the next active cycle.
            ballWasHit = false;
        }

        // Makes the LineRenderer visible only when the laser is active.
        lineRenderer.enabled = laserActive;

        // If the laser is inactive, stop running the rest of Update().
        if (!laserActive)
            return;


        // ---------------- LASER DETECTION ----------------

        // Gets the starting position of the laser.
        Vector2 origin = transform.position;

        // Gets the direction the laser is facing.
        // transform.right means the laser travels along its local right direction.
        Vector2 direction = transform.right;

        // Casts a circular detection area forward from the laser.
        // This detects objects within the laser's thickness.
        RaycastHit2D hit = Physics2D.CircleCast(
            origin,                  // Starting position of the laser.
            laserWidth * 0.5f,       // Radius of the detection area.
            direction,               // Direction the laser travels.
            laserLength,             // Maximum distance the laser travels.
            collisionLayers          // Layers the laser is allowed to detect.
        );


        // ---------------- LASER END POSITION ----------------

        // If something was hit, use the distance to that object.
        // Otherwise, use the full laser length.
        float distance = hit.collider != null
            ? hit.distance
            : laserLength;

        // Calculates where the laser should end.
        // The laser starts at origin and travels forward by distance.
        Vector3 endPosition = origin + direction * distance;

        // Sets the starting point of the visible laser.
        lineRenderer.SetPosition(0, transform.position);

        // Sets the ending point of the visible laser.
        lineRenderer.SetPosition(1, endPosition);


        // ---------------- GOLF BALL DETECTION ----------------

        // Creates a variable to store the player's last safe position script.
        PlayerLastPosition player = null;

        // Checks whether the laser hit something.
        if (hit.collider != null)
        {
            // Looks for PlayerLastPosition on the object that was hit.
            player = hit.collider.GetComponent<PlayerLastPosition>();

            // If the script wasn't found, check the object's parent.
            if (player == null)
            {
                player = hit.collider
                    .GetComponentInParent<PlayerLastPosition>();
            }
        }

        // Checks if the laser hit the golf ball.
        // Also checks whether the ball has already been hit.
        if (player != null && !ballWasHit)
        {
            // Records that the ball has been hit.
            ballWasHit = true;

            // Calls ResetPosition() from the PlayerLastPosition script.
            // This sends the golf ball back to its last safe position.
            player.ResetPosition();
        }


        // ---------------- RESET HIT DETECTION ----------------

        // Checks if the ball was previously hit but is no longer
        // touching the laser beam.
        if (ballWasHit && !BallStillInBeam())
        {
            // Allows the laser to hit the ball again.
            ballWasHit = false;
        }
    }


    // ---------------- CHECK IF BALL IS IN LASER ----------------

    // This function checks whether the golf ball is still inside the laser.
    // Returns true if the ball is detected, otherwise returns false.
    private bool BallStillInBeam()
    {
        // Gets the starting position of the laser.
        Vector2 origin = transform.position;

        // Gets the direction the laser is facing.
        Vector2 direction = transform.right;

        // Casts another circular detection area along the laser beam.
        RaycastHit2D[] hits = Physics2D.CircleCastAll(
            origin,                  // Starting position.
            laserWidth * 0.5f,       // Radius of the detection area.
            direction,               // Direction of the laser.
            laserLength,             // Maximum detection distance.
            collisionLayers          // Layers to detect.
        );

        // Goes through every object detected by the laser.
        foreach (RaycastHit2D hit in hits)
        {
            // Checks if the detected object has PlayerLastPosition.
            // If not, checks its parent for that script.
            if (hit.collider.GetComponent<PlayerLastPosition>() != null ||
                hit.collider.GetComponentInParent<PlayerLastPosition>() != null)
            {
                // Returns true because the golf ball is still in the beam.
                return true;
            }
        }

        // Returns false if the golf ball is no longer in the beam.
        return false;
    }
}
