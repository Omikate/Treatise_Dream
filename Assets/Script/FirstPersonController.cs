using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FirstPersonController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 6f;
    public float walkModifier = 0.5f;
    public float sprintModifier = 1.8f;
    public float jumpForce = 3f; 

    [Header("Camera")]
    public Transform cameraTransform;
    public float mouseSensitivty = 3f;
    public float maxLookAngle = 85f;
    private float rotationX = 0f;

    [Header ("Ground Check")]
    public float groundCheckDistance = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody rb; 

    private bool canMove = true;
    private bool hasJumped = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void FixedUpdate()
    {
        if (canMove)
        {
            Move();
        }

        if (hasJumped)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            hasJumped = false;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            if (canMove)
                DisableController();
            else
                EnableController();
        }
        if (canMove)
        {
            Look();

            if(Input.GetKeyDown (KeyCode.Space) && IsGrounded())
                hasJumped = true;
        }
    }

    void Move()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 inputDir = (transform.forward * v + transform.right * h).normalized;
        Vector3 moveVelocity = inputDir * walkSpeed * GetSpeedModifier();
        Vector3 currentVelocity = rb.velocity;

        rb.velocity = new Vector3(moveVelocity.x, currentVelocity.y, moveVelocity.z);
    }

    void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivty;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivty;

        transform.Rotate(0f, mouseX, 0f);
        
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -maxLookAngle, maxLookAngle);
        cameraTransform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
    }

    public void DisableController()
    {
        canMove = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }

    public void EnableController()
    {
        canMove = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        rb.constraints = RigidbodyConstraints.None;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    float GetSpeedModifier()
    {
        if (Input.GetKey(KeyCode.LeftAlt))
            return walkModifier;
        else if (Input.GetKey(KeyCode.LeftShift))
            return sprintModifier;
        else
            return 1;
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, groundCheckDistance, groundLayer);
    }
}
