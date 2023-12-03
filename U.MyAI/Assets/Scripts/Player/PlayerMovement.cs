using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Transform orientation;

    [Header("MOVEMENT PROPERTIES")]
    [SerializeField] private float walkSpeed;
    [Range(0, 0.5f)]
    [SerializeField] private float moveSmoothTime;
    private Vector3 movementVelocity;
    private Vector3 moveDampVelocity;
    private Rigidbody rb;
    private Vector3 playerInput;
    private Vector3 movementVector;

    [Header("GRAVITY PROPERTIES")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private float groundGravity;
    [SerializeField] private LayerMask groundMask;
    private float yVelocity;
    private bool IsGrounded => Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask);

    [Header("STEP OFFSET PROPERTIES")]
    [SerializeField] private Transform stepRayUpper;
    [SerializeField] private Transform stepRayLower;
    [SerializeField] private float upperRayLength;
    [SerializeField] private float lowerRayLength;
    [SerializeField] private float stepOffset;
    [SerializeField] private float stepSmooth;

    [Header("SLOPE PROPERTIES")]
    [SerializeField] private float maxSlopeAngle;
    [SerializeField] private float slopeCheckRayLength;
    private RaycastHit slopeHit;

    private void Awake()
    {
        stepRayUpper.position = new Vector3(stepRayUpper.position.x, stepOffset, stepRayUpper.position.z);
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        HandlePlayerInput();
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleGravity();
        HandleStepOffset();
    }

    private void HandleGravity()
    {
        //if(OnSlope()) return;

        if(IsGrounded)
        {
            yVelocity = groundGravity;
        }
        else
        {
            yVelocity += Physics.gravity.y * Time.fixedDeltaTime;
        }

        rb.velocity = new Vector3(rb.velocity.x, yVelocity, rb.velocity.z);
    }

    private void HandleMovement()
    {
        transform.forward = orientation.forward;
        movementVector = orientation.forward * playerInput.z + orientation.right * playerInput.x;
        movementVector.Normalize();

        if(OnSlope())
        {
            movementVector = GetSlopeDirection();
        }

        movementVelocity = Vector3.SmoothDamp(
            movementVelocity,
            movementVector * walkSpeed,
            ref moveDampVelocity,
            moveSmoothTime
        );

        rb.velocity = new Vector3(movementVelocity.x, rb.velocity.y, movementVelocity.z);
    }

    private void HandleStepOffset()
    {
        if(playerInput.magnitude == 0) return;
        if(!IsGrounded) return;
        if(OnSlope()) return;

        for(int x = -1; x <= 1; x++)
        {
            for(int z = -1; z <= 1; z++)
            {
                if(Physics.Raycast(stepRayLower.position, (transform.TransformDirection(x, 0, z)).normalized, lowerRayLength))
                {
                    if(!Physics.Raycast(stepRayUpper.position, (transform.TransformDirection(x, 0, z)).normalized, upperRayLength))
                    {
                        rb.position -= new Vector3(0, -stepSmooth, 0f);
                    }
                }
            }
        }
    }

    private bool OnSlope() // WARNING: THERE IS CRITICAL BUG! NOT WORKING!
    {
        //for(int x = -1; x < 2; x++){
        //    if(Physics.Raycast( transform.TransformDirection(new Vector3(transform.position.x + groundCheckRadius, transform.position.y, transform.position.z)), Vector3.down, out slopeHit,slopeCheckRayLength))
        //    {
        //        float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
        //        return angle < maxSlopeAngle && angle != 0;
        //    }
        //}

        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, slopeCheckRayLength))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    private Vector3 GetSlopeDirection()
    {
        return Vector3.ProjectOnPlane(movementVector, slopeHit.normal).normalized;
    }

    private void HandlePlayerInput()
    {
        playerInput = new(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
    }

    private void OnValidate()
    {
        for(int x = -1; x <= 1; x++)
        {
            for(int z = -1; z <= 1; z++)
            {
                if(Physics.Raycast(stepRayLower.position, (transform.TransformDirection(x, 0, z)).normalized, lowerRayLength))
                {
                    if(!Physics.Raycast(stepRayUpper.position, (transform.TransformDirection(x, 0, z)).normalized, upperRayLength))
                    {
                        rb.position -= new Vector3(0, -stepSmooth, 0f);
                    }
                }
            }
        }
    }
    

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);


        for(int x = -1; x <= 1; x++)
        {
            for(int z = -1; z <= 1; z++)
            {
                Gizmos.DrawRay(stepRayUpper.position, transform.TransformDirection(x, 0, z).normalized * upperRayLength);
                Gizmos.DrawRay(stepRayLower.position, transform.TransformDirection(x, 0, z).normalized * lowerRayLength);
            }
        }

        for(float x = -1; x < 2; x++)
        {
            Gizmos.DrawRay(transform.TransformDirection(new Vector3(transform.position.x + groundCheckRadius * x, transform.position.y, transform.position.z)), Vector3.down * slopeCheckRayLength);
        }

    }
}
