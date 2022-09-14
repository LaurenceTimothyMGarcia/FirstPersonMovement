using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private Rigidbody playerRB;

    [SerializeField] private float speed = 15f;
    [SerializeField] private float jumpHeight = 10f;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;

    private Vector3 velocity;
    private bool isGrounded;

    private float xInput;
    private float zInput;
    private bool jumpPressed;

    // Start is called before the first frame update
    void Start()
    {
        
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
        Vector3 direction = transform.right * xMove + transform.forward * zMove;

        playerRB.AddForce(direction);
    }

    private void Jump(bool jumpButton, bool isGrounded)
    {
        if (isGrounded && jumpButton)
        {
            Debug.Log("Jump");
            Vector3 jumpVector = new Vector3(0, jumpHeight, 0);
            playerRB.AddForce(jumpVector, ForceMode.Impulse);
        }
    }

    private void MoveInput()
    {
        xInput = Input.GetAxisRaw("Horizontal");
        zInput = Input.GetAxisRaw("Vertical");
        jumpPressed = Input.GetButtonDown("Jump");
    }
}