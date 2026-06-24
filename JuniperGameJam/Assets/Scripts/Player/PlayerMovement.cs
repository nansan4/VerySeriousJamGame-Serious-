using UnityEngine;
using UnityEngine.Events;
using Unity.Cinemachine;

public class PlayerMovement : BaseMovement
{
    #region Variables

    [Space(20)]

    [Header("Character - Ground Movement")]
    [SerializeField] protected Vector3 movementDirection;                        // The interpreted 3D direction in which this character should move.
    [SerializeField] private float accelerationRate = 60f;      // The speed at which this character accelerates in m/s
    [SerializeField] private float decelerationRate = 12f;      // The speed at which this character decelerates in m/s
    [SerializeField] private float maxWalkSpeed = 1f;           // The max horizontal speed of this character (when walking) in m/s
    [SerializeField] private float maxVerticalSpeed = 10f;      // The maximum vertical move speed of this character in m/s
    [SerializeField] private float minimumWorldHeight = 0.5f;   // The minimum height the character's pivot can go before being set to this value

    [Header("Character - Air Movement")]
    [SerializeField] protected float jumpForce = 6f;            // The amount of power used by this character to jump.
    [SerializeField] [Range(0f,1f)] protected float jumpForceAcceleration = 0.015f;
    private float currentJFA = 0f;
    [SerializeField] [Range(0f,1f)] protected float jumpForceDeceleration = 0.015f;
    private float currentJFD = 0f;
    public UnityEvent OnCharacterJump;                          // Event that fires when the character jumps
    private bool isGainingHeight = false;
    private bool isReducingHeight = false;
    private bool isReducingVerticalSpeed = false;
    private bool isGainingVerticalSpeed = false;
    [SerializeField] private float idleBobAmount = 0.2f;
    [SerializeField] [Range(0,2)] private float idleBobSpeed = 1.2f;
    private float currentBobOffset = 0f;
    [SerializeField] private float bobBlendSpeed = 5f;

    [Header("Character - Rotation")]
    [SerializeField] private float groundRotationRate = 10f;    // The rate at which the player rotates (when grounded)
    [SerializeField] private float responsiveness = 0.75f;
    [SerializeField] private float maxPitchAngle = 45f;          // The maximum angle the character can pitch up or down when moving
    [SerializeField] private float maxRollAngle = 45f;           // The maximum angle the character can roll left or right when moving
    private bool isBeingSpun = false;
    private float currentSpinVelocity;

    [Header("Fans")]
    [SerializeField] private float fanForce = 10f;

    [Header("Character - Camera")]
    [SerializeField] private Transform cameraTransform;         // The transform component of our Character's Camera

    [Header("Virtual Camera References")]
    [SerializeField] private CinemachineCamera mainCamera;
    

    [Header("Character - Component/Object References")]
    private CapsuleCollider capsuleCollider;

    //[Header("Audio Sources")]
    //[SerializeField] private AudioSource jumpSource;
    //[SerializeField] private AudioSource landSource;

    #endregion

    #region Unity Functions

    private void Awake()
    {
        //get ref to more specific collider type
        capsuleCollider = collider as CapsuleCollider;

        currentMaxSpeed = maxWalkSpeed;

        //init any other *internal* data needed when starting the game
    }

    private void Start()
    {

    }

    private void FixedUpdate()
    {
        //check grounded first so that info is available to other functions
        //CheckIsGrounded();

        //apply any character movement
        Move();
        ApplyTilt();
        //Rotate();

        GainHeight();
        ReduceHeight();
        if (!isGainingHeight && !isReducingHeight)
        {
            ReduceVerticalSpeed();
        }

        //make sure we limit out speed AFTER we start to speed
        LimitVelocity();
        if (currentJFA < 0){currentJFA = 0;}
        if (currentJFD < 0){currentJFD = 0;}
    }

    private void Update()
    {
        CalculateCameraRelativeInput();

        //rotate character visually, no physics involved, to face our movement
        Rotate();
    }

    #endregion

    #region Custom Functions

    #region Input

    // Receive movement input from the PlayerController
    public override void SetMovementInput(Vector2 moveInput)
    {
        base.SetMovementInput(moveInput); //call parent implementation of this func

    }

    // Convert movement input to be relative to the camera's view
    void CalculateCameraRelativeInput()
    {
        if (movementInput == Vector2.zero) //if there isnt any input, do nothing
        {
            movementDirection = Vector3.zero;
            return;
        }

        //always flat since camera doesn't roll
        Vector3 cameraRight = cameraTransform.right;

        //get forward vector w/o any y value
        Vector3 cameraForward = cameraTransform.forward;
        cameraForward.y = 0f; //remove vertical component
        cameraForward.Normalize(); //normalise to make length 1 because cutting the y makes it shorter

        //scale camera direction vectors by the magnitude of input values then add them
        movementDirection = (cameraForward * movementInput.y) + (cameraRight * movementInput.x);

        //cap movement to be 1 for diagonal case so movement on the diagonal isn't faster than on the straight
        //sqr mag because if mag > 1 sqr is too, so quick optimisation
        if (movementDirection.sqrMagnitude > 1f) { movementDirection.Normalize(); }
    }

    #endregion

    #region Movement

    // Apply forces to accelerate / decelerate the character
    protected override void Move()
    {
        if (movementDirection != Vector3.zero) //if we're trying to move
        {
            //apply force as acceleration, constant regardless of mass
            rb.AddForce(movementDirection * accelerationRate, ForceMode.Acceleration);
        }
        else if (movementDirection == Vector3.zero) //if we aren't trying to move
        {
            Vector3 currentVelocity = GetHorizontalRBVelocity();

            if (currentVelocity.magnitude > 0.005f) //if we're still moving
            {
                Vector3 counteractDirection = currentVelocity.normalized * -1f; //get direction w/o magnitute
                rb.AddForce(counteractDirection * decelerationRate, ForceMode.Acceleration);
            }

            //Vector3 modelEuler = characterModel.transform.rotation.eulerAngles;
            //Quaternion modelTargetRotation = Quaternion.Euler(0f, modelEuler.y, 0f);
            //characterModel.transform.rotation = Quaternion.Slerp(characterModel.transform.rotation, modelTargetRotation, groundRotationRate * Time.fixedDeltaTime);

            //Vector3 bodyEuler = transform.rotation.eulerAngles;
            //Quaternion bodyTargetRotation = Quaternion.Euler(0f, bodyEuler.y, 0f);
            //transform.rotation = Quaternion.Slerp(transform.rotation, bodyTargetRotation, groundRotationRate * Time.fixedDeltaTime);
        }

        //set player position to minimum world height
        if(transform.position.y < minimumWorldHeight)
        {
            Vector3 vel = rb.linearVelocity;
            vel.y = 0f;
            rb.linearVelocity = vel;
            //clamp player y pos to minimum world height
            rb.position = new Vector3(rb.position.x, minimumWorldHeight, rb.position.z);
        }
    }

    #region Rotation
    // Rotate the character model to face direction of movement
    protected override void Rotate()
    {
        if (movementDirection != Vector3.zero && !isBeingSpun) //if we're trying to move and are not caught in a fan
        {
            //rotate mesh by aligning their forward vector with movement direction, Slerp is spherical interpolation
            characterModel.forward = Vector3.Slerp(characterModel.forward, movementDirection.normalized, groundRotationRate * Time.deltaTime);
        }
    }

    /// <summary>
    /// Tilts the character model based on horizontal velocity to give a sense of momentum and weight. The tilt is applied by calculating the desired pitch and roll angles based on the character's local velocity and then smoothly interpolating the character model's rotation towards these target angles using Slerp for smooth transitions. The responsiveness variable controls how quickly the character tilts in response to changes in velocity, while maxPitchAngle and maxRollAngle limit the maximum tilt angles to prevent excessive tilting. This function is called in FixedUpdate to ensure that the tilt is updated consistently with the physics simulation.
    /// </summary>
    private void ApplyTilt()
    {
        Vector3 velocity = GetHorizontalRBVelocity();
        velocity.y = 0f; //we only want horizontal velocity for tilting
        Vector3 localVelocity = characterModel.InverseTransformDirection(velocity);

        float pitch = Mathf.Clamp(localVelocity.z * maxPitchAngle, -maxPitchAngle, maxPitchAngle);
        float roll = Mathf.Clamp(localVelocity.x * maxRollAngle, -maxRollAngle, maxRollAngle);

        float currentYaw = characterMesh.localEulerAngles.y;
        Quaternion targetTilt = Quaternion.Euler(pitch, currentYaw, roll);
        characterMesh.localRotation = Quaternion.Slerp(characterMesh.localRotation, targetTilt, responsiveness * Time.fixedDeltaTime);
    }
    #endregion

    // Limit horizontal and vertical velocity
    private void LimitVelocity()
    {
        //limit horizontal velocity
        //check if current velocity is greater than our max allowed vel
        Vector3 currentVelocity = GetHorizontalRBVelocity();

        //magnitude gets the square root of the vector, since is expensive do multiplication instead
        if (currentVelocity.sqrMagnitude > (currentMaxSpeed * currentMaxSpeed))
        {
            //use velocity change (i.e impulse) to slow down because we're using acceleration (both ignore mass of the rb)
            Vector3 counteractDirection = currentVelocity.normalized * -1f;
            float counteractAmount = currentVelocity.magnitude - currentMaxSpeed;
            rb.AddForce(counteractDirection * counteractAmount, ForceMode.VelocityChange);
        }

        //limit vertical velocity
        if (Mathf.Abs(rb.linearVelocity.y) > maxVerticalSpeed)
        {
            Vector3 counteractDirection = Vector3.up * Mathf.Sign(rb.linearVelocity.y) * -1f;
            float counteractAmount = Mathf.Abs(rb.linearVelocity.y) - maxVerticalSpeed;
            rb.AddForce(counteractDirection * counteractAmount, ForceMode.VelocityChange);
        }

        //if the player is idling and not at the set world height, move them to it
        if (!isReducingHeight && !isGainingHeight && !isReducingVerticalSpeed)
        {
            float targetBobOffset = Mathf.Sin(Time.time * idleBobSpeed) * idleBobAmount;
            currentBobOffset = Mathf.Lerp(currentBobOffset, targetBobOffset, bobBlendSpeed * Time.fixedDeltaTime);
            Vector3 sinePos = new Vector3(transform.position.x, transform.position.y + currentBobOffset, transform.position.z);

            rb.MovePosition(sinePos);
        }
    }

    // accelerate character upwards, continuous force
    public override void Launch()
    {
        isGainingHeight = true;
        isReducingHeight = false;
        currentJFD = 0;
    }

    public override void Fall()
    {
        isGainingHeight = false;
        isReducingHeight = true;
        currentJFD = 0;
    }

    private void GainHeight()
    {
        if (isGainingHeight && !isReducingHeight)
        {
            //jump accounting for pre-existing vertical velocity
            if (currentJFA < 1) currentJFA += jumpForceAcceleration; //JFA = JumpForceAccleration
            if (currentJFA > 1) currentJFA = 1;
            float adjustedJumpForce = (jumpForce - rb.linearVelocity.y) * currentJFA;
            // Debug.Log(adjustedJumpForce);
            rb.AddForce(Vector3.up * adjustedJumpForce, ForceMode.Acceleration);
        }
    }

    private void ReduceHeight()
    {
        if (isReducingHeight && !isGainingHeight)
        {
            if (currentJFA < 1) currentJFA += jumpForceAcceleration;
            if (currentJFA > 1) currentJFA = 1;
            float adjustedJumpForce = (jumpForce - rb.linearVelocity.y) * currentJFA;
            rb.AddForce(Vector3.down * adjustedJumpForce, ForceMode.Acceleration);
        }
    }

    // Allow for partial jumps
    public override void CancelLaunch()
    {
        isGainingHeight = false;
        currentJFA = 0;

        //if we're still moving upwards, cut vertical velocity in half
        //if (rb.linearVelocity.y > 0f)
        //{
        //    rb.AddForce(Vector3.down * (rb.linearVelocity.y * 0.8f), ForceMode.VelocityChange);
        //}
    }

    public override void CancelFall()
    {
        isReducingHeight = false;
        currentJFA = 0;
    }

    private void ReduceVerticalSpeed()
    {
        if (rb.linearVelocity.sqrMagnitude < 0.0015f) 
        {
            isReducingVerticalSpeed = false;
            return;
        }
        isReducingVerticalSpeed = true;

        if (currentJFD < 1) currentJFD += jumpForceDeceleration;
        if (currentJFD > 1) currentJFD = 1;
        float adjustedJumpForce = (jumpForce - rb.linearVelocity.y) * currentJFD;

        Vector3 verticalDirection = rb.linearVelocity.y < 0f ? Vector3.up : Vector3.down;
        rb.AddForce(verticalDirection * adjustedJumpForce, ForceMode.Acceleration);

        
    }

    #endregion

    #region Collision/Triggers

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Fan"))
        {
            isBeingSpun = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Fan"))
        return;

        Debug.Log("Being pushed by fan!");

        rb.AddForce(other.transform.up * fanForce, ForceMode.Acceleration);

        float velocity = GetHorizontalRBVelocity().magnitude;
        float maxSpinSpeed = (fanForce + velocity) * 120f;

        // Randomly change spin direction and magnitude
        currentSpinVelocity += Random.Range(-maxSpinSpeed, maxSpinSpeed) * Time.fixedDeltaTime;
        currentSpinVelocity = Mathf.Clamp(currentSpinVelocity, -maxSpinSpeed, maxSpinSpeed);

        characterModel.Rotate(0f, currentSpinVelocity * Time.fixedDeltaTime, 0f, Space.Self);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Fan"))
        {
            isBeingSpun = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        PlayerAudioManager.Instance.PlayRandomCollisionSFX();
    }

    #endregion

    #endregion

    #region Debug

    private void OnDrawGizmos()
    {
        //draw movement direction vector
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + movementDirection * 2);

        //draw velocity vector
        if (rb != null)
        {Gizmos.color = new Color(0f, 0.5f, 1f); // bright blue
            Gizmos.DrawLine(transform.position, transform.position + rb.linearVelocity);
        }
    }

    #endregion  

}
