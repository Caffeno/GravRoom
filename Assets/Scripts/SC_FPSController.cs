using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SC_FPSController : MonoBehaviour
{
    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float jumpHeight = 2f;
    public float gravityScale = 2f;
    public Camera playerCamera;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;
    public float rotationSpeed = 10f;
    RotatingCharacterController characterController;
    public Vector3 moveDirection = Vector3.zero;

    [SerializeField] private LayerMask groundMask;

    float rotationX = 0;

    [HideInInspector]
    public bool canMove = true;

    private GravityController gravityController;

    private float lastPrint = 0;
    private Rigidbody playerRidgedbody;
    private CapsuleCollider playerCollider;
    private bool isGrounded = false;
    private float bottomSphereOffset;
    private IEnumerator fixingRotationCoroutine;


    void Start()
    {
        characterController = GetComponent<RotatingCharacterController>();

        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        gravityController = FindObjectOfType<GravityController>();
        gravityController.OnGravityShift += OnGravityShift;
        characterController.OnFloorUpdate += OnFloorUpdate;
        playerCollider = GetComponent<CapsuleCollider>();
        playerRidgedbody = GetComponent<Rigidbody>();

        bottomSphereOffset = (playerCollider.height / 2 - playerCollider.radius);

    }

    void Update()
    {
        CheckRotation();
        moveDirection = playerRidgedbody.velocity;
        // We are grounded, so recalculate move direction based on axes
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        Vector3 up = transform.TransformDirection(Vector3.up);
        // Press Left Shift to run
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
        float verticalSpeed = Vector3.Dot(moveDirection, up);

        moveDirection = (forward * curSpeedX) + (right * curSpeedY) + (up * verticalSpeed);

        Ray groundRay = new Ray(transform.position - bottomSphereOffset * transform.up, Physics.gravity);

        isGrounded = Physics.SphereCast(groundRay, playerCollider.radius * 0.9f, playerCollider.radius * 0.2f, groundMask) && verticalSpeed < 0;

        if (Input.GetButton("Jump") && canMove && isGrounded) 
        {

            moveDirection += Mathf.Sqrt(2 * jumpHeight * gravityScale * Physics.gravity.magnitude) * up;
        }
        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)


        if (!isGrounded)
        {
            moveDirection += Physics.gravity * gravityScale * Time.deltaTime;
        }


        // Move the controller
        //moveDirection = characterController.Move(moveDirection);
        playerRidgedbody.velocity = moveDirection;


        // Player and Camera rotation
        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }

    private void OnGravityShift(Vector3 gravityDirection)
    {
        Vector3 forward = transform.forward;

        if (Mathf.Abs(gravityDirection.x) == 1)
        {
            forward.x = 0;
        }
        else if (Mathf.Abs(gravityDirection.y) == 1)
        {
            forward.y = 0;
        }
        else
        {
            forward.z = 0;
        }
        forward = forward.normalized;
        //transform.rotation = Quaternion.LookRotation(forward, -gravityDirection);
        Quaternion targetRotation = Quaternion.LookRotation(forward, -gravityDirection);
        if (fixingRotationCoroutine != null)
        {
            StopCoroutine(fixingRotationCoroutine);
        }
        fixingRotationCoroutine = FixRotation(targetRotation);
        StartCoroutine(fixingRotationCoroutine);
    }

    private void OnFloorUpdate(bool nowGrounded)
    {
        if (nowGrounded)
        {
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);
            // Press Left Shift to run
            bool isRunning = Input.GetKey(KeyCode.LeftShift);
            float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
            float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
            moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        }
    }

    private IEnumerator FixRotation(Quaternion targetRotation)
    {
        while (transform.rotation != targetRotation)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            yield return null;
        }
    }

    private void LateUpdate()
    {

    }

    private void CheckRotation()
    {
        Quaternion correctRotation = (transform.rotation * Quaternion.FromToRotation(transform.up, -Physics.gravity));
        if (transform.rotation != correctRotation)
        {
            //StartCoroutine("FixRotation", correctRotation);
        }
    }
}
