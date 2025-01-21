using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Editor_Variables
    [Header("Movement")]
    [SerializeField] float accelerationSpeed;
    [SerializeField] float decelerationSpeed;
    [SerializeField] float airAccelSpeedFactor = 0.5f;
    [SerializeField] float maxSpeed;    

    [Header("Jump")]
    [SerializeField] float jumpForce;

    [Header("Raycast - Ground")]
    [SerializeField] LayerMask groundMask;
    [SerializeField] float rayLength;
    [SerializeField] Transform groundCheck;
    #endregion

    #region Private_Variables
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
    #endregion
}
