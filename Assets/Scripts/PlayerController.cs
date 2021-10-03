using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Random")]
    public float movementSpeed = 6f;

    [SerializeField] Transform orientation;

    float movementMultiplier = 10f;
    [SerializeField] float airMultiplier = 0.04f;
    float horizontalMovement;
    float verticalMovement;
    float groundDrag = 6f;
    float airDrag = 0.5f;
    float groundDistance = 0.4f;

    float playerHeight = 2f;
    public float jumpForce = 5f;

    bool isOnGround;

    [SerializeField] Transform groundCheck;

    Vector3 moveDirection;
    Vector3 slopeMoveDirection;

    Rigidbody rb;
    [SerializeField] LayerMask groundMask;
    RaycastHit slopeHit;

    [Header("Keybinds")]
    [SerializeField] KeyCode jumpKey = KeyCode.Space;
    [SerializeField] KeyCode sprintKey = KeyCode.LeftShift;

    [Header("Sprinting")]
    [SerializeField] float walkSpeed = 5f;
    [SerializeField] float sprintSpeed = 10f;
    [SerializeField] float acceleration = 10f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }
    private void Update()
    {
        isOnGround = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        MyInput();
        ControlDrag();
        ControlSpeed();

        if(Input.GetKeyDown(jumpKey) && isOnGround)
        {
            Jump();
        }

        slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);
    }

    void ControlSpeed()
    {
        if(Input.GetKey(sprintKey) && isOnGround)
        {
            movementSpeed = Mathf.Lerp(movementSpeed, sprintSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            movementSpeed = Mathf.Lerp(movementSpeed, walkSpeed, acceleration * Time.deltaTime);
        }
    }


    void Jump()
    {
        if (isOnGround)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
    }

    void MyInput()
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");

        moveDirection = orientation.forward * verticalMovement + orientation.right * horizontalMovement;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        if (isOnGround && !OnSlope())
        {
            rb.AddForce(moveDirection.normalized * movementSpeed * movementMultiplier, ForceMode.Acceleration);
        }
        else if (isOnGround && OnSlope())
        {
            rb.AddForce(slopeMoveDirection.normalized * movementSpeed * movementMultiplier, ForceMode.Acceleration);
        }
        else if(!isOnGround)
        {
            rb.AddForce(moveDirection.normalized * movementSpeed * movementMultiplier * airMultiplier, ForceMode.Acceleration);
        }
    }

    void ControlDrag()
    {
        if (isOnGround)
        {
            rb.drag = groundDrag;
        }
        else
        {
            if(rb.drag > 0.0f)
                rb.drag = airDrag;
        }
    }

    private bool OnSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.5f))
        {
            if(slopeHit.normal != Vector3.up)
            {
                return true;
            }else
            {
                return false;
            }
        }
        return false;
    }
}
