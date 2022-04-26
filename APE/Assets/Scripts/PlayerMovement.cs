//
// Bachelor of Software Engineering
// Media Design School
// Auckland
// New Zealand
//
// (c) Media Design School
//
// File Name : PlayerMovement.cs
// Description : Managers manoeuvrability of the player.
// Author : Ethan Uy
// Mail : ethan.uy@mds.ac.nz
//

using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("General")]
    [SerializeField] Transform Camera;

    [Header("Settings - Camera")]
    [SerializeField] bool LockCursor;
    [SerializeField] float CameraSensitivity;

    [Header("Settings - Movement")]
    [SerializeField] float MovementSpeed;
    [SerializeField][Range(0.0f, 0.5f)] float MovementSmoothTime = 0.3f;
    [SerializeField] float MovementGravity;

    [Header("Private Variables")]
    // General
    private CharacterController controller;
    // Camera
    private float cameraPitch;
    // Movement
    private float velocityY;
    private Vector2 currentDirection = Vector2.zero;
    private Vector2 currentDirectionVel = Vector2.zero;


    private void Start()
    {
        // Set the Character Controller.
        controller = GetComponent<CharacterController>();
        
        // Set the cursor to be locked or unlocked.
        Cursor.lockState = LockCursor ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !LockCursor;
    }

    private void Update()
    {
        UpdateMouseLook();
        UpdateMovement();
    }

    private void UpdateMouseLook()
    {
        // Get mouse inputs.
        Vector2 MouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        // Determine camera rotation.
        cameraPitch -= MouseDelta.y * CameraSensitivity;
        cameraPitch = Mathf.Clamp(cameraPitch, -90.0f, 90.0f);  // Disallow the camera to look past 90 degrees vertically.

        // Change camera rotation.
        Camera.localEulerAngles = Vector3.right * cameraPitch;
        
        // Apply rotation to physical character.
        transform.Rotate(Vector3.up * MouseDelta.x * CameraSensitivity);
    }

    private void UpdateMovement()
    {
        // Get Target Input
        Vector2 TargetDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        // Normalize it (prevents faster movement when diagonal)
        TargetDirection.Normalize();

        // Smoothing into current direction.
        currentDirection = Vector2.SmoothDamp(currentDirection, TargetDirection, ref currentDirectionVel, MovementSmoothTime);

        // Applying gravity
        if (controller.isGrounded)
            velocityY = 0.0f;

        velocityY += MovementGravity * Time.deltaTime;

        // Calculate velocity
        Vector3 Velocity = (transform.forward * currentDirection.y + transform.right * currentDirection.x);
        Velocity *= MovementSpeed;
        Velocity += (Vector3.up * velocityY);

        // Apply movement
        controller.Move(Velocity * Time.deltaTime);
    }

}
