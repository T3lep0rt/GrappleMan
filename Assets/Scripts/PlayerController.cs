using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float movementSpeed = 6f;

    [SerializeField] Transform orientation;

    float movementMultiplier = 10f;
    [SerializeField] float airMultiplier = 0.04f;
    float horizontalMovement;
    float verticalMovement;
    float groundDrag = 6f;
    float airDrag = 0.5f;

    float playerHeight = 2f;
    public float jumpForce = 5f;

    bool isOnGround;

    Vector3 moveDirection;

    Rigidbody rb;

    [Header("Keybinds")]
    [SerializeField] KeyCode jumpKey = KeyCode.Space;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }
    private void Update()
    {
        isOnGround = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.1f);

        MyInput();
        ControlDrag();

        if(Input.GetKeyDown(jumpKey) && isOnGround)
        {
            Jump();
        }
    }

    void Jump()
    {
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
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
        if (isOnGround)
        {
            rb.AddForce(moveDirection.normalized * movementSpeed * movementMultiplier, ForceMode.Acceleration);
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
            rb.drag = airDrag;
        }
    }
}
