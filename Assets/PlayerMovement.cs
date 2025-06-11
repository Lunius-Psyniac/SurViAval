using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Stats")]
    public float speed = 6f;
    public float pushPower = 2f;

    [Header("Camera Controls")]
    public float mouseSensitivity = 2f;
    public Transform head; // For vertical rotation

    [Header("Physics")]
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;
    public Transform groundCheck; // We'll create this in Unity
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    private CharacterController controller;
    private float verticalRotation = 0f;

    private Vector3 velocity;
    private bool isGrounded;
    
    // A flag to control whether player input is processed.
    private bool canMove = true;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // If movement is frozen, don't process any input.
        if (!canMove) return;

        // Ground check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Stick to ground
        }

        // Mouse look
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(0f, mouseX, 0f);

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);
        head.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);

        // Movement
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        controller.Move(move * speed * Time.deltaTime);

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    /// <summary>
    /// Freezes or unfreezes player movement and look controls.
    /// </summary>
    public void SetMovement(bool shouldMove)
    {
        canMove = shouldMove;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // --- Study Zone Interaction ---
        if (hit.gameObject.TryGetComponent<StudyZone>(out StudyZone zone))
        {
            FindFirstObjectByType<StudyManager>().OnPlayerEnterZone(zone);
            return; // Exit early to prevent other physics interactions
        }

        // --- Pushing Physics Objects ---
        Rigidbody body = hit.collider.attachedRigidbody;

        // no rigidbody
        if (body == null || body.isKinematic)
        {
            return;
        }

        // We dont want to push objects below us
        if (hit.moveDirection.y < -0.3)
        {
            return;
        }

        // Calculate push direction from move direction,
        // we only push objects to the sides never up and down
        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);

        // If you know how fast your character is trying to move,
        // then you can also multiply the push velocity by that.
        
        // Apply the push
        body.AddForceAtPosition(pushDir * pushPower, hit.point);
    }
}
