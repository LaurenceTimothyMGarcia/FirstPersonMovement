using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private Rigidbody playerRB;

    [SerializeField] private float speed = 2f;
    [SerializeField] private float jumpHeight = 10f;
    [SerializeField] private int maxJump = 2;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;

    private Vector3 velocity;
    private bool isGrounded;
    private int tempJump;

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
        Move(xInput, zInput);
        Jump(jumpPressed, isGrounded);
    }

    private void Move(float xMove, float zMove)
    {
        if (activeGrapple)
        {
            return;
        }

        Vector3 direction = transform.right * xMove * speed + transform.forward * zMove * speed;
        playerRB.AddForce(direction);
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

    private void MoveInput()
    {
        xInput = Input.GetAxisRaw("Horizontal");
        zInput = Input.GetAxisRaw("Vertical");
        jumpPressed = Input.GetButtonDown("Jump");
    }

    //Grappling tutorial
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
