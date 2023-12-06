using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Public variables
    // Can be set in engine for testing
    private float speed = 100;                 
    private float jumpStrength = 100;          

    // Private variables
    private float horizontalInput;
    private float jumpInput;
    private Rigidbody2D playerBody;

    // Colliders
    public LayerMask layerMask;
    private BoxCollider2D groundCollider;

    // Jump timing
    private float jumpTimer;

    // Variables sent to the packet for the server
    Vector2 playerPosition;

    // Start is called before the first frame update
    void Start()
    {
        // Fetch the Rigidbody from the GameObject with this script attached
        playerBody = GetComponent<Rigidbody2D>();
        playerBody.drag = 1.2f;

        // Fetch the colliders from the GameObject
        BoxCollider2D[] colliders = GetComponents<BoxCollider2D>();
        groundCollider = colliders[1];
        jumpTimer = 0.0f;
    }

    // Check ground colider to see if on the ground or a player
    bool IsGrounded()
    {
        bool grounded = groundCollider.IsTouchingLayers(layerMask);
        return grounded;
    }

    // Update is called once per frame
    void Update()
    {
        // Update timers
        jumpTimer += Time.deltaTime;

        // Local variables
        Vector2 moveVector;
        Vector2 jumpVector;

        // Check if the player is on the ground
        if (IsGrounded())
        {
            jumpInput = Input.GetAxis("Jump");
            if (jumpInput != 0 && jumpTimer >= 0.1f) 
            {
                jumpVector = new Vector2(0.0f, jumpInput * jumpStrength);
                playerBody.AddForce(jumpVector, ForceMode2D.Impulse);
                jumpTimer = 0.0f;
            }

            // Move the player horizontally
            // Adds a force so that there is slight momentum
            horizontalInput = Input.GetAxis("Horizontal");
            moveVector = new Vector2(horizontalInput * speed * Time.deltaTime, 0.0f);
            playerBody.AddForce(moveVector, ForceMode2D.Impulse);
        }
        // If timer > 2 then the player is either stuck or about to land so let them nudge themselves 
        else if (jumpTimer > 2)
        {
            horizontalInput = Input.GetAxis("Horizontal");
            moveVector = new Vector2(horizontalInput * speed * Time.deltaTime, 0.0f);
            playerBody.AddForce(moveVector, ForceMode2D.Impulse);
        }
        // The player is airborne so let them mildly adjust their trajectory
        else
        {
            horizontalInput = Input.GetAxis("Horizontal");
            moveVector = new Vector2(horizontalInput * 0.2f * speed * Time.deltaTime, 0.0f);
            playerBody.AddForce(moveVector, ForceMode2D.Impulse);
        }

        playerPosition = playerBody.transform.position;
    }
}
