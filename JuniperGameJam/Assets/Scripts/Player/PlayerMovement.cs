using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.Cinemachine;

public class PlayerMovement : BaseMovement
{
    #region Variables

    [Space(20)]

    [Header("Character - Ground Movement")]
    protected Vector3 movementDirection;                        // The interpreted 3D direction in which this character should move.
    [SerializeField] private float accelerationRate = 60f;      // The speed at which this character accelerates in m/s
    [SerializeField] private float decelerationRate = 12f;      // The speed at which this character decelerates in m/s
    [SerializeField] private float maxWalkSpeed = 1f;           // The max horizontal speed of this character (when walking) in m/s
    [SerializeField] private float maxVerticalSpeed = 10f;      // The maximum vertical move speed of this character in m/s
    [SerializeField] private float minimumWorldHeight = 0.5f;   // The minimum height the character's pivot can go before being set to this value

    [Header("Character - Air Movement")]
    [SerializeField] protected float jumpForce = 6f;            // The amount of power used by this character to jump.
    public UnityEvent OnCharacterJump;                          // Event that fires when the character jumps
    private bool isJumping = false;

    [Header("Character - Rotation")]
    [SerializeField] private float groundRotationRate = 10f;    // The rate at which the player rotates (when grounded)

    [Header("Character - Camera")]
    [SerializeField] private Transform cameraTransform;         // The transform component of our Character's Camera

    [Header("Virtual Camera References")]
    [SerializeField] private CinemachineCamera mainCamera;
    [SerializeField] private CinemachineCamera altCamera;

    [Header("Character - Component/Object References")]
    private CapsuleCollider capsuleCollider;

    //[Header("Audio Sources")]
    //[SerializeField] private AudioSource jumpSource;
    //[SerializeField] private AudioSource landSource;
    //[SerializeField] private AudioSource runningSource;
    //[SerializeField] private AudioSource camSwitchSource;

    //[SerializeField] private float walkPitch = .6f;
    //[SerializeField] private float runPitch = 1.0f;


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

        JumpFunc();

        //make sure we limit out speed AFTER we start to speed
        LimitVelocity();

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

    // Rotate the character model to face direction of movement
    protected override void Rotate()
    {
        if (movementDirection != Vector3.zero) //if we're trying to move
        {
            //rotate mesh by aligning their forward vector with movement direction, Slerp is spherical interpolation
            characterModel.forward = Vector3.Slerp(characterModel.forward, movementDirection.normalized, groundRotationRate * Time.deltaTime);
        }
    }

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
    }

    // accelerate character upwards, continuous force
    public override void Launch()
    {
        isJumping = true;
    }

    private void JumpFunc()
    {
        if (isJumping)
        {
            //jump accounting for pre-existing vertical velocity
            float adjustedJumpForce = jumpForce - rb.linearVelocity.y;
            rb.AddForce(Vector3.up * adjustedJumpForce, ForceMode.Acceleration);
        }
    }

    // Allow for partial jumps
    public override void CancelLaunch()
    {
        isJumping = false;

        //if we're still moving upwards, cut vertical velocity in half
        //if (rb.linearVelocity.y > 0f)
        //{
        //    rb.AddForce(Vector3.down * (rb.linearVelocity.y * 0.8f), ForceMode.VelocityChange);
        //}
    }

    // Tell the character to start sprinting

    #endregion

    //#region Ground Checking

    //// Check if the character is on the ground
    //private void CheckIsGrounded()
    //{
    //    //record the grounded status from previous check
    //    wasGroundedLastFrame = isGrounded;

    //    //calc the origin of the sphere just below the center of the bottom sphere of the capsule collider
    //    //we assume the origin is at the bottom center of the capsule
    //    Vector3 p1 = transform.position + (Vector3.up * (capsuleCollider.radius * 0.9f));

    //    //OverlapSphere returns an array of colliders it hits
    //    Collider[] groundColliders = Physics.OverlapSphere(p1, capsuleCollider.radius * 0.95f, environmentLayerMask); //center of sphere, radius of sphere, environment LayerMask (hittable layers)

    //    //we're on the ground if there's atleast 1 collider under us
    //    isGrounded = groundColliders.Length > 0;

    //    //if we became grounded this frame
    //    if (!wasGroundedLastFrame && isGrounded)
    //    {
    //        currentJump = 0;

    //        landSource.Play();
    //    }
    //    //we became airborne this frame
    //    else if (wasGroundedLastFrame && !isGrounded)
    //    {
    //        //if we didn't jump since jumping would set it to 1
    //        if (currentJump == 0)
    //        {
    //            currentJump += 1; //expend jump when becoming airborne
    //        }
    //    }
    //}

    //#endregion

    #endregion

    #region Debug

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = isGrounded ? Color.green : Color.red;
    //    Vector3 p1 = transform.position + (Vector3.up * (capsuleCollider.radius * 0.9f));

    //    Gizmos.DrawWireSphere(p1, capsuleCollider.radius * 0.95f);
    //}

    #endregion  

}
