using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Editor_Variables
    [Header("Movement")]
    [SerializeField] float accelerationSpeed;
    [SerializeField] float decelerationSpeed;
    [SerializeField] float airAccelSpeedFactor = 0.8f;
    [SerializeField] float maxSpeed;    

    [Header("Jump")]
    [SerializeField] float jumpForce;

    [Header("Raycast - Ground")]
    [SerializeField] LayerMask groundMask;
    [SerializeField] float rayLength;
    [SerializeField] Transform groundCheck;    

    // Needed time to reach the grappling point
    [SerializeField] float timeToReachPoint = 2f;
    #endregion

    #region Private_Variables
    // Player pos. offset respect ot the Grappling point
    [SerializeField] Vector3 offset;

    // Raycast vars
    Ray ray;
    RaycastHit hit;
    Vector3 rayOffsetOrigin = new Vector3(0, 0.1f, 0);

    // Movement vectors & vars.
    Vector2 horizontalMovement;
    Vector3 slowdown;
    float horizontal;
    float vertical;

    // Boolean flags
    bool isGrounded;
    bool jumpPressed;

    // GO Components
    Rigidbody rb;
    #endregion

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    
    void Update()
    {
        InputPlayer();   
        JumpPressed();
    }

    void FixedUpdate()
    {
        DetectGround();
        Movement();
        Jump();
    }

    void DetectGround()
    {        
        // Define Raycast origin and direction
        ray.origin = groundCheck.position + rayOffsetOrigin;
        ray.direction = -transform.up;

        // Update Ground detection
        isGrounded = Physics.Raycast(ray, out hit, rayLength, groundMask);

        //Debugging
        Debug.DrawRay(ray.origin, ray.direction * rayLength, Color.red);        
    }
    #region InputPlayer_Methods
    void InputPlayer()
    {
        horizontal = Input.GetAxis("Horizontal");   // AD
        vertical = Input.GetAxis("Vertical");       // WS
    }
    void JumpPressed()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            jumpPressed = true;
    }
    #endregion
    #region Movement_Methods
    void Movement()
    {
        // RB MAXIMUM SPEED LIMITATION

        // Store the Rb velocity on the XZ Plane
        horizontalMovement = new Vector2 (rb.velocity.x,rb.velocity.z);
        if(horizontalMovement.magnitude > maxSpeed)
        {
            // Limit the speed movement
            horizontalMovement = horizontalMovement.normalized;         // Gets the vector movement direction
            horizontalMovement *= maxSpeed;                             // Limit the vector movement magnitude
                                                                        // to the max speed value.
        }
        // Applies to the rb the updated speed
        rb.velocity = new Vector3(horizontalMovement.x,rb.velocity.y,horizontalMovement.y);

        // XZ PLANE MOVEMENT UPDATE THROUGH INPUT PLAYER

        // AddRelativeForce allows to implement a fluid movement with accels. and decels.
        // Adding Force to the player in the desired direction in order to move him
        if (isGrounded)        
            rb.AddRelativeForce(
                horizontal*accelerationSpeed*Time.fixedDeltaTime,       // X-Axis mov.
                0,                                                      // Y-Axis mov.
                vertical * accelerationSpeed * Time.fixedDeltaTime);    // Z-Axis mov.                    
        else
            rb.AddRelativeForce(
                horizontal * (accelerationSpeed*airAccelSpeedFactor) * Time.fixedDeltaTime, // X-Axis mov.
                0,                                                                          // Y-Axis mov.
                vertical * (accelerationSpeed * airAccelSpeedFactor) * Time.fixedDeltaTime);  // Z-Axis mov.                    

        // RB DECELERATION
        if (isGrounded)
            rb.velocity = Vector3.SmoothDamp(rb.velocity, 
                                            new Vector3(0,rb.velocity.y,0),
                                            ref slowdown, 
                                            decelerationSpeed);
    }    
    void Jump()
    {        
        if (jumpPressed)
        {
            jumpPressed = false;
            rb.AddForce(Vector3.up * jumpForce);
        }
    }
    public IEnumerator MoveTowardsPoint(Vector3 targetPos)
    {
        // Init the timer
        float timeElapsed = 0f;
        // Get the Player Start Position
        Vector3 playerStartPos = transform.position;
        // Calculate the dir. from the player towards the target
        Vector3 directionToTarget = (targetPos - transform.position).normalized;
        // Apply the offset on the oposite dir. to the target
        targetPos = targetPos - directionToTarget * offset.magnitude;

        // Keep moving towards the Point as long as we haven't reached it
        while (timeElapsed < timeToReachPoint)
        {
            timeElapsed += Time.deltaTime;  // Timer increase

            // Position interpolation
            transform.position = Vector3.Lerp(playerStartPos, targetPos, timeElapsed / timeToReachPoint);

            // Wait for the next frame to continue
            yield return null;
        }

        // Assure we reach the exact end position
        transform.position = targetPos;
    }
    #endregion
}
