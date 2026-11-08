

using System.Collections; // Allows the use of Coroutines and IEnumerator.
using UnityEngine; // Gives access to Unity components and functions.

public class FlameObstacle : MonoBehaviour
{
    // ---------------- FLAME SETTINGS ----------------

    [Header("Flame Settings")]

    // Maximum distance the flame can extend.
    [SerializeField] private float maxLength = 5f;

    // How long it takes for the flame to fully extend.
    [SerializeField] private float extendTime = 1f;

    // How long the flame stays fully extended.
    [SerializeField] private float holdTime = 1f;

    // How long it takes for the flame to retract.
    [SerializeField] private float retractTime = 0.5f;

    // How long the flame waits before starting another burst.
    [SerializeField] private float delayBetweenBursts = 1f;


    // ---------------- PARTICLE SETTINGS ----------------

    [Header("Particle Settings")]

    // Reference to the Particle System used to display the flames.
    [SerializeField] private ParticleSystem flameParticles;


    // ---------------- DETECTION ----------------

    [Header("Detection")]

    // Determines which Unity layers can be detected by the flame.
    // This should include the golf ball's layer.
    [SerializeField] private LayerMask ballLayer;


    // ---------------- PRIVATE VARIABLES ----------------

    // Tracks whether the golf ball has already been hit.
    // Prevents the ball from being reset repeatedly.
    private bool ballHit;

    // Tracks the current length of the flame as it extends and retracts.
    private float currentLength;


    // ---------------- START ----------------

    private void Start()
    {
        // Makes sure the particle system starts turned off.
        flameParticles.Stop();

        // Starts the coroutine that controls the flame's behavior.
        StartCoroutine(FlameRoutine());
    }


    // ---------------- FLAME ROUTINE ----------------

    // Coroutine that controls the entire flame cycle.
    // IEnumerator allows the function to pause and continue over time.
    private IEnumerator FlameRoutine()
    {
        // Keeps repeating the flame cycle forever.
        while (true)
        {
            // ---------------- EXTEND FLAME ----------------

            // Turns on the particle system.
            flameParticles.Play();

            // Creates a timer to track the extension progress.
            float timer = 0f;

            // Continues extending until the extendTime has passed.
            while (timer < extendTime)
            {
                // Adds the time passed since the previous frame.
                timer += Time.deltaTime;

                // Calculates the extension progress from 0 to 1.
                // 0 means the flame is starting.
                // 1 means the flame is fully extended.
                float progress = Mathf.Clamp01(timer / extendTime);

                // Gradually increases the flame length from 0 to maxLength.
                // Mathf.Lerp smoothly interpolates between two values.
                currentLength = Mathf.Lerp(0f, maxLength, progress);

                // Updates the particle system to match the current flame length.
                UpdateFlame();

                // Checks whether the golf ball is touching the flame.
                CheckForBall();

                // Pauses the coroutine until the next frame.
                yield return null;
            }


            // ---------------- HOLD FLAME ----------------

            // Makes sure the flame is at its maximum length.
            currentLength = maxLength;

            // Resets the timer for the holding phase.
            timer = 0f;

            // Keeps the flame fully extended for holdTime seconds.
            while (timer < holdTime)
            {
                // Tracks the time spent holding the flame.
                timer += Time.deltaTime;

                // Updates the particle system.
                UpdateFlame();

                // Continues checking for the golf ball.
                CheckForBall();

                // Waits until the next frame.
                yield return null;
            }


            // ---------------- RETRACT FLAME ----------------

            // Resets the timer for the retraction phase.
            timer = 0f;

            // Continues retracting until retractTime has passed.
            while (timer < retractTime)
            {
                // Adds the time passed since the previous frame.
                timer += Time.deltaTime;

                // Calculates the retraction progress from 0 to 1.
                float progress = Mathf.Clamp01(timer / retractTime);

                // Gradually decreases the flame length from maxLength to 0.
                currentLength = Mathf.Lerp(maxLength, 0f, progress);

                // Updates the particle system to match the shrinking flame.
                UpdateFlame();

                // Checks whether the ball touches the flame.
                CheckForBall();

                // Waits until the next frame.
                yield return null;
            }


            // ---------------- END FLAME BURST ----------------

            // Turns off the particle system after the flame retracts.
            flameParticles.Stop();

            // Resets the flame length to zero.
            currentLength = 0f;

            // Allows the flame to hit the ball again during its next burst.
            ballHit = false;

            // Waits before starting the next flame burst.
            yield return new WaitForSeconds(delayBetweenBursts);
        }
    }


    // ---------------- UPDATE FLAME ----------------

    // Updates the Particle System based on the current flame length.
    private void UpdateFlame()
    {
        // Gets the Shape module of the Particle System.
        // This controls the area where particles are initially emitted.
        var shape = flameParticles.shape;

        // Sets the particle emission shape radius.
        shape.radius = 0.1f;

        // Gets the Main module of the Particle System.
        // This controls general particle properties.
        var main = flameParticles.main;

        // Changes the particle starting speed based on flame length.
        // As the flame gets longer, particles travel faster.
        // Mathf.Max prevents division by zero.
        main.startSpeed = currentLength / Mathf.Max(extendTime, 0.01f);

        // Changes the particle system's scale as the flame extends.
        // The X scale increases with the flame length.
        flameParticles.transform.localScale =
            new Vector3(currentLength / maxLength, 1f, 1f);
    }


    // ---------------- CHECK FOR BALL ----------------

    // Checks whether the golf ball is touching the flame.
    private void CheckForBall()
    {
        // If the ball was already hit, or the flame has no length,
        // stop checking for collisions.
        if (ballHit || currentLength <= 0f)
            return;

        // Gets the starting position of the flame.
        Vector2 origin = transform.position;

        // Gets the direction the flame is facing.
        // transform.right means the flame travels along its local right direction.
        Vector2 direction = transform.right;

        // Casts a circular detection area in front of the obstacle.
        RaycastHit2D hit = Physics2D.CircleCast(
            origin,       // Starting position of the detection area.
            0.3f,         // Radius of the detection area.
            direction,    // Direction the flame travels.
            currentLength,// Current length of the flame.
            ballLayer     // Only detects objects on the selected layers.
        );

        // If nothing was detected, stop checking.
        if (hit.collider == null)
            return;

        // Looks for the PlayerLastPosition script on the detected object.
        PlayerLastPosition player =
            hit.collider.GetComponent<PlayerLastPosition>();

        // If the script wasn't found, check the object's parent.
        if (player == null)
        {
            player = hit.collider.GetComponentInParent<PlayerLastPosition>();
        }

        // Checks if the detected object is the golf ball.
        if (player != null)
        {
            // Records that the ball has been hit.
            ballHit = true;

            // Calls ResetPosition() to return the ball to its last safe position.
            player.ResetPosition();
        }
    }
}
