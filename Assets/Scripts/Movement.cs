using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    //Basics, Movement, Jump
    [SerializeField] private Rigidbody playerRB;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float jumpHeight = 20f;
    [SerializeField] private int maxJump = 2;
    [SerializeField] private int groundDrag = 2;
    [SerializeField] private int airDrag = 0;

    //Ground check related variables
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;
    private bool isGrounded;

    private Vector3 velocity;
    private int tempJump;

    //Input
    private float xInput;
    private float zInput;
    private bool jumpPressed;

    //Grapple variables
    public bool freeze;
    public bool activeGrapple;
    [SerializeField] private float grappleSpeed = 2f;

    // Start is called before the first frame update
    void Start()
    {
        tempJump = maxJump;
    }

    // Update is called once per frame
    void Update()
    {
        GrappleFreeze();    //graplling tutorial

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        MoveInput();
        Jump(jumpPressed, isGrounded);
    }

    void FixedUpdate()
    {
        Move(xInput, zInput);
    }

    //Movement
    private void Move(float xMove, float zMove)
    {
        if (activeGrapple)
        {
            playerRB.drag = 0;
            return;
        }

        Vector3 direction = transform.right * xMove + transform.forward * zMove;
        playerRB.AddForce(direction.normalized * speed * 10, ForceMode.Force);
        
        //Clamp horizontal speed to maxSpeed
        ControlSpeed();
        
        if(isGrounded) 
        {
            playerRB.drag = groundDrag; 
        }
        else 
            playerRB.drag = airDrag;

    }

    private void ControlSpeed()
    {
        Vector3 flatVelocity = new Vector3(playerRB.velocity.x, 0f, playerRB.velocity.z);

        
        if(flatVelocity.magnitude > maxSpeed) 
        {
            //recalculate speed to be within limits of maxSpeed;
            Vector3 maxVelocity = flatVelocity.normalized * maxSpeed;

            //set player x and z velocity to new limited velocity
            playerRB.velocity = new Vector3(maxVelocity.x, playerRB.velocity.y, maxVelocity.z);
        }
    }

    private void Jump(bool jumpButton, bool isGrounded)
    {
        if (activeGrapple)
        {
            return;
        }

        if (jumpButton && tempJump > 0)
        {
            Vector3 jumpVector = new Vector3(0, jumpHeight, 0);
            playerRB.AddForce(jumpVector, ForceMode.Impulse);
            tempJump--;
        }

        if (tempJump <= 0 && isGrounded)
        {
            tempJump = maxJump;
        }
    }

    //Create Input
    private void MoveInput()
    {
        xInput = Input.GetAxisRaw("Horizontal");
        zInput = Input.GetAxisRaw("Vertical");
        jumpPressed = Input.GetButtonDown("Jump");
    }

    //Grappling tutorial//
    private void GrappleFreeze()
    {
        if (freeze)
        {
            playerRB.velocity = Vector3.zero;
        }
    }

    public void ResetRestrictions()
    {
        activeGrapple = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (enableMovementOnNextTouch)
        {
            enableMovementOnNextTouch = false;
            ResetRestrictions();

            GetComponent<Grappling>().StopGrapple();
        }
    }

    private bool enableMovementOnNextTouch;

    public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
    {
        activeGrapple = true;

        velocityToSet = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);
        Invoke(nameof(SetVelocity), 0.1f);

        Invoke(nameof(ResetRestrictions), 3f);
    }

    private Vector3 velocityToSet;

    private void SetVelocity()
    {
        enableMovementOnNextTouch = true;
        playerRB.velocity = velocityToSet;
    }

    public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = Physics.gravity.y;
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ * grappleSpeed / (Mathf.Sqrt(-2 * trajectoryHeight / gravity) + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

        return velocityXZ + velocityY;
    }
}
