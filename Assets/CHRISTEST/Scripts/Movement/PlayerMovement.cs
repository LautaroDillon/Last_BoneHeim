using System.Collections;
using UnityEngine;
using EZCameraShake;
public enum SurfaceType
{
    Default,
    Stone,
    Metal,
    Wood
}

public class PlayerMovement : MonoBehaviour
{
    private SurfaceType currentSurface;

    public Animator animator;
    public static PlayerMovement instance;

    [Header("Abilites")]
    public bool canDoubleJump = false;
    public bool canDash = false;
    public bool normalSpeed = false;

    [Header("References")]
    public Transform orientation;
    public Camera mainCam;
    public PlayerCamera cam;
    public PlayerClimb playerClimb;

    [Header("Movement")]
    public float walkSpeed;
    public float sprintSpeed;
    public float slideSpeed;
    public float dashSpeed;
    private float moveSpeed;
    public float climbSpeed;
    public float vaultSpeed;

    public float dashSpeedChangeFactor;
    public float maxYSpeed;
    public float groundDrag;

    public float speedIncreaseMultiplier;
    public float slopeIncreaseMultiplier;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    [SerializeField] private int jumpCount;
    bool readyToJump;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public bool grounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    [Header("Bools")]
    public bool dashing;
    public bool sliding;
    public bool wallrunning;
    public bool freeze;
    public bool unlimited;
    public bool vaulting;
    public bool climbing;

    float horizontalInput;
    float verticalInput;
    private float stepRate = 0.5f;
    private float stepCoolDown;
    bool isWalking;

    Vector3 moveDirection;

    Rigidbody rb;

    public MovementState state;

    public enum MovementState
    {
        freeze,
        unlimited,
        walking,
        sprinting,
        sliding,
        dashing,
        wallrunning,
        climbing,
        vaulting,
        air
    }

    private void Start()
    {
        PlayerMovement.instance = this;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;

        startYScale = transform.localScale.y;

        jumpCount = 0;

        if(normalSpeed == true)
        {
            jumpForce = 15;
            walkSpeed = 12;
        }
        else
        {
            jumpForce = 0;
            walkSpeed = 3;
        }
    }

    private void Update()
    {
        if (PauseManager.isPaused || PlayerHealth.hasDied)
            return;
        else
        {
            // ground check
            grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
            sprintSpeed = walkSpeed;
            crouchSpeed = walkSpeed + 4;

            stepCoolDown -= Time.deltaTime;

            if ((Input.GetAxis("Horizontal") != 0f || Input.GetAxis("Vertical") != 0f) && stepCoolDown < 0f && grounded && state == MovementState.walking)
            {
                PlayStepSound();
                stepCoolDown = stepRate;
            }

            if (grounded)
            {
                currentSurface = GetSurfaceType();
                jumpCount = 0;
            }

            MyInput();
            SpeedControl();
            StateHandler();

            // handle drag
            if (state == MovementState.walking || state == MovementState.sprinting)
                rb.drag = groundDrag;
            else
                rb.drag = 0;

            if (state == MovementState.air || state == MovementState.dashing || state == MovementState.sliding)
                rb.drag = 0;
            else
                rb.drag = groundDrag;
        }

    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(jumpKey) && readyToJump)
        {
            if (grounded || (canDoubleJump && jumpCount < 1))
            {
                readyToJump = false;
                jumpCount++;
                Jump();

                
                Invoke(nameof(ResetJump), jumpCooldown);
            }
        }
    }

    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;
    private MovementState lastState;
    private bool keepMomentum;

    private void StateHandler()
    {
        if (freeze)
        {
            state = MovementState.freeze;
            rb.velocity = Vector3.zero;
            desiredMoveSpeed = 0f;
        }

        else if (unlimited)
        {
            state = MovementState.unlimited;
            desiredMoveSpeed = 999f;
        }

        else if (vaulting)
        {
            state = MovementState.vaulting;
            desiredMoveSpeed = vaultSpeed;
        }

        else if (climbing)
        {
            state = MovementState.climbing;
            desiredMoveSpeed = climbSpeed;
        }

        if (dashing)
        {
            state = MovementState.dashing;
            desiredMoveSpeed = dashSpeed;
            speedChangeFactor = dashSpeedChangeFactor;
        }

        if (sliding && grounded)
        {
            state = MovementState.sliding;

            if (OnSlope() && rb.velocity.y < 0.1f)
                desiredMoveSpeed = slideSpeed;
            else
                desiredMoveSpeed = sprintSpeed;
        }

        else if (grounded)
        {
            if (lastState == MovementState.air)
            {
                PlayLandingSound();
            }

            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
        }

        else
        {
            state = MovementState.air;

            if (desiredMoveSpeed < sprintSpeed)
                desiredMoveSpeed = walkSpeed;
            else
                desiredMoveSpeed = sprintSpeed;
        }

        bool desiredMoveSpeedHasChanged = desiredMoveSpeed != lastDesiredMoveSpeed;
        if (lastState == MovementState.dashing)
            keepMomentum = true;

        if (Mathf.Abs(desiredMoveSpeed - lastDesiredMoveSpeed) > 4f && moveSpeed != 0)
        {
            StopAllCoroutines();
            StartCoroutine(SmoothlyLerpMoveSpeed());
        }
        else
        {
            moveSpeed = desiredMoveSpeed;
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;

        lastState = state;
    }

    private float speedChangeFactor;
    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        // smoothly lerp movementSpeed to desired value
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);

            if (OnSlope())
            {
                float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                float slopeAngleIncrease = 1 + (slopeAngle / 90f);

                time += Time.deltaTime * speedIncreaseMultiplier * slopeIncreaseMultiplier * slopeAngleIncrease;
            }
            else
                time += Time.deltaTime * speedIncreaseMultiplier;

            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
    }

    private void MovePlayer()
    {
        if (playerClimb.exitingWall)
            return;
        if (state == MovementState.dashing)
            return;

        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // on slope
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection(moveDirection) * moveSpeed * 20f, ForceMode.Force);

            if (rb.velocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }

        // on ground
        else if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        // in air
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

        // turn gravity off while on slope
        if(wallrunning)
            rb.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {
        // limiting speed on slope
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;
        }

        // limiting speed on ground or in air
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            // limit velocity if needed
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }

        // limit y vel
        if (maxYSpeed != 0 && rb.velocity.y > maxYSpeed)
            rb.velocity = new Vector3(rb.velocity.x, maxYSpeed, rb.velocity.z);
    }

    private void Jump()
    {
        exitingSlope = true;

        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

        PlayJumpSound();
    }

    private void ResetJump()
    {
        readyToJump = true;
        exitingSlope = false;
    }

    public bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }

    public static float Round(float value, int digits)
    {
        float mult = Mathf.Pow(10.0f, (float)digits);
        return Mathf.Round(value * mult) / mult;
    }

    private void PlayStepSound()
    {
        switch (currentSurface)
        {
            case SurfaceType.Stone:
                AudioManager.instance.PlaySFXOneShot("StoneWalk", 1f);
                break;
            case SurfaceType.Metal:
                AudioManager.instance.PlaySFXOneShot("GrassWalk", 1f);
                break;
            case SurfaceType.Wood:
                AudioManager.instance.PlaySFXOneShot("WoodWalk", 1f);
                break;
            default:
                AudioManager.instance.PlaySFXOneShot("Walk", 1f); // Default walk sound
                break;
        }
    }

    private void PlayJumpSound()
    {
        switch (currentSurface)
        {
            case SurfaceType.Stone:
                AudioManager.instance.PlaySFX("StoneJump", 1f, false);
                break;
            case SurfaceType.Metal:
                AudioManager.instance.PlaySFX("GrassJump", 1f, false);
                break;
            case SurfaceType.Wood:
                AudioManager.instance.PlaySFX("WoodJump", 1f, false);
                break;
            default:
                AudioManager.instance.PlaySFX("Jump", 1f, false);
                break;
        }
    }

    private void PlayLandingSound()
    {
        switch (currentSurface)
        {
            case SurfaceType.Stone:
                AudioManager.instance.PlaySFXOneShot("StoneLand", 1f);
                break;
            case SurfaceType.Metal:
                AudioManager.instance.PlaySFXOneShot("GrassLand", 1f);
                break;
            case SurfaceType.Wood:
                AudioManager.instance.PlaySFXOneShot("WoodLand", 1f);
                break;
            default:
                AudioManager.instance.PlaySFXOneShot("Landing", 1f);
                break;
        }
    }

    private SurfaceType GetSurfaceType()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            if (slopeHit.collider.CompareTag("StoneGround"))
                return SurfaceType.Stone;
            /*else if (slopeHit.collider.CompareTag("MetalGround"))
                return SurfaceType.Metal;
            //else if (slopeHit.collider.CompareTag("WoodGround"))
                return SurfaceType.Wood;*/
        }
        return SurfaceType.Default;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Heart")
        {
            walkSpeed = 12;
            jumpForce = 15;
            normalSpeed = true;
            AudioManager.instance.PlaySFXOneShot("Heartbeat", 1f);
            AudioManager.instance.PlayMusic("Background Music", 1f);
            Destroy(other.gameObject);
        }

        if (other.gameObject.tag == "Stomach")
        {
            canDoubleJump = true;
            AudioManager.instance.PlaySFXOneShot("Stomach", 1f);
            Destroy(other.gameObject);
        }
        if (other.gameObject.tag == "Lungs")
        {
            canDash = true;
            AudioManager.instance.PlaySFXOneShot("Lungs", 1f);
            Destroy(other.gameObject);
        }
    }
}
