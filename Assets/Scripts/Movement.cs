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

    // Start is called before the first frame update
    void Start()
    {
        tempJump = maxJump;
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        MoveInput();
        Move(xInput, zInput);
        Jump(jumpPressed, isGrounded);
    }

    private void Move(float xMove, float zMove)
    {
        Vector3 direction = transform.right * xMove * speed + transform.forward * zMove * speed;
        playerRB.AddForce(direction);
    }

    private void Jump(bool jumpButton, bool isGrounded)
    {
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
}
