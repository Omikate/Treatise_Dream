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
    private PlayerStats playerStats; 

    private bool canMove = true;
    private bool hasJumped = false;
    
    // เพิ่มตัวแปรเช็คสถานะ "หมดแรง"
    private bool isExhausted = false; 

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerStats = GetComponent<PlayerStats>(); 

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
        bool isMoving = Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0;

        // 1. ถ้ายืนเฉยๆ หรือไม่มี PlayerStats ให้คืนค่าปกติไปก่อน
        if (playerStats == null) return 1;

        // 2. เช็คอาการหมดแรง (Stamina ใกล้ 0 ให้ติดสถานะ Exhausted)
        if (playerStats.currentStamina <= 0.1f)
        {
            isExhausted = true;
        }
        // 3. ฟื้นตัวจากอาการหมดแรง (ต้องรอให้หลอดกลับมาอย่างน้อย 20% ถึงจะวิ่งใหม่ได้)
        else if (isExhausted && playerStats.currentStamina > 20f)
        {
            isExhausted = false;
        }

        // 4. สั่งงานการเคลื่อนที่
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            playerStats.isSprinting = false;
            return walkModifier;
        }
        // เปลี่ยนเงื่อนไข: จะวิ่งได้ต้อง "ไม่ติดสถานะหมดแรง (isExhausted)" เท่านั้น
        else if (Input.GetKey(KeyCode.LeftShift) && isMoving && !isExhausted)
        {
            playerStats.isSprinting = true; 
            return sprintModifier;          
        }
        else
        {
            playerStats.isSprinting = false; 
            return 1; 
        }
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, groundCheckDistance, groundLayer);
    }
}