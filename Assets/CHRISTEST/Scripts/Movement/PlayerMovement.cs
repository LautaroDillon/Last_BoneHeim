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
    #region Variables
    private SurfaceType currentSurface;

    public Animator handAnimator;
    public Animator legAnimator;
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
    public GameObject lungsPosition;
    public GameObject stomachPosition;

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

    [Header("Damping")]
    public float dampingFactor = 5f;

    [Header("Momentum")]
    public float momentum = 0f;
    public float maxMomentum = 10f;
    public float momentumBuildRate = 2f;
    public float momentumDecayRate = 1f;
    public float momentumSpeedMultiplier = 0.5f;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    public float extraAirGravity = 10f;
    [SerializeField] private int jumpCount;
    bool readyToJump;

    [Header("Coyote Time")]
    public float coyoteTime = 0.2f;
    private float coyoteTimeCounter;

    [Header("Jump Buffering")]
    public float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    [Header("Airtime")]
    public Transform arms;
    public float maxAirTime = 1.0f;
    public float maxOffset = 0.2f;
    private float airTime = 0f;
    private Vector3 armsInitialPos;

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
    private Vector2 smoothedInput;
    private float stepRate = 0.5f;
    private float stepCoolDown;
    bool isWalking;
    public float inputSmoothSpeed = 10f;

    Vector3 moveDirection;

    Rigidbody rb;

    public MovementState state;

    #endregion

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

    #region Start/Update

    private void Start()
    {
        stomachPosition.SetActive(false);
        lungsPosition.SetActive(false);
        PlayerMovement.instance = this;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;

        startYScale = transform.localScale.y;

        jumpCount = 0;

        if (normalSpeed == true)
        {
            walkSpeed = 12;
        }
        else
        {
            walkSpeed = 3;
        }

        armsInitialPos = arms.localPosition;
        legAnimator.SetLayerWeight(1, 1f);
    }

    private void Update()
    {
        if (PauseManager.isPaused || PlayerHealth.hasDied)
            return;

        jumpBufferCounter -= Time.deltaTime;

        Vector3 camForward = mainCam.transform.forward;
        camForward.y = 0f;

        if (camForward.sqrMagnitude > 0.01f)
            orientation.forward = camForward.normalized;

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
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (grounded && jumpBufferCounter > 0f && readyToJump)
        {
            jumpBufferCounter = 0f;
            Jump();
            readyToJump = false;
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        MyInput();
        SpeedControl();
        StateHandler();
        Airtime();
        HandleMomentum();

        bool isIdle = Mathf.Approximately(horizontalInput, 0f) && Mathf.Approximately(verticalInput, 0f);

        if (grounded && isIdle)
        {
            //rb.drag = groundDrag * 5f;
        }
        else if (state == MovementState.walking || state == MovementState.sprinting)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0f;
        }
    }


    private void FixedUpdate()
    {
        MovePlayer();
        ApplyCounterMovement();
        if (grounded && moveDirection.magnitude < 0.1f)
        {
            Vector3 dampedVel = new Vector3(
                Mathf.Lerp(rb.velocity.x, 0, Time.fixedDeltaTime * dampingFactor),
                rb.velocity.y,
                Mathf.Lerp(rb.velocity.z, 0, Time.fixedDeltaTime * dampingFactor)
            );
            rb.velocity = dampedVel;
        }
    }

    #endregion

    private void MyInput()
    {
        Vector2 targetInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        smoothedInput = Vector2.Lerp(smoothedInput, targetInput, Time.deltaTime * inputSmoothSpeed);

        horizontalInput = smoothedInput.x;
        verticalInput = smoothedInput.y;

        if (Input.GetKeyDown(jumpKey))
            jumpBufferCounter = jumpBufferTime;

        if (Input.GetKeyDown(jumpKey) && readyToJump)
        {
            if (coyoteTimeCounter > 0f || (canDoubleJump && jumpCount < 1))
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

    #region Movement States

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
        else if (dashing)
        {
            state = MovementState.dashing;
            desiredMoveSpeed = dashSpeed;
            speedChangeFactor = dashSpeedChangeFactor;
        }
        else if (sliding && grounded)
        {
            if (OnSlope() && rb.velocity.y < 0.1f)
            {
                if (slopeHit.collider.CompareTag("Stairs"))
                {
                    state = MovementState.walking;
                    desiredMoveSpeed = walkSpeed;
                }
                else
                {
                    state = MovementState.sliding;
                    desiredMoveSpeed = slideSpeed;
                }
            }
            else
            {
                desiredMoveSpeed = sprintSpeed;
            }
        }
        else if (grounded)
        {
            if (lastState == MovementState.air)
            {
                PlayLandingSound();
                CameraShake.Instance.ShakeOnce(2f, 2f, 0.1f, 1f);
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

    #endregion

    #region Movement Methods
    #region Main Movement
    private void MovePlayer()
    {
        if (playerClimb.exitingWall)
            return;
        if (state == MovementState.dashing)
            return;

        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        

        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection(moveDirection) * moveSpeed * 20f, ForceMode.Force);

            if (rb.velocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);

            // Prevent sliding when no input
            if (moveDirection.magnitude < 0.1f)
            {
                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            }
        }
        else if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

            // Prevent sliding when no input on flat ground
            if (moveDirection.magnitude < 0.1f)
            {
                rb.velocity = new Vector3(0, rb.velocity.y, 0);
            }
        }
        else if (!grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
            rb.AddForce(Vector3.down * extraAirGravity, ForceMode.Force);
        }

        // turn gravity off while on slope and wallrunning
        if (wallrunning)
            rb.useGravity = !OnSlope();

        if (OnSlope() && grounded && moveDirection == Vector3.zero)
        {
            rb.velocity = Vector3.zero;
        }

        if (!grounded)
        {
            Vector3 airMove = moveDirection.normalized * moveSpeed * airMultiplier;
            Vector3 flatVel = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            Vector3 airForce = airMove - flatVel;
            rb.AddForce(airForce, ForceMode.Force);
            rb.AddForce(Vector3.down * extraAirGravity, ForceMode.Force);
        }

        bool isMoving = moveDirection.magnitude > 0.1f && grounded;
        Debug.LogError(isMoving);
        if (isMoving)
        {
            legAnimator.SetBool("Idle", false);
            legAnimator.SetBool("iswalking", isMoving);
        }
        if (!isMoving)
        {
            legAnimator.SetBool("Idle", true);
            legAnimator.SetBool("iswalking", false);
        }
    }

    private void Jump()
    {
        exitingSlope = true;

        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        coyoteTimeCounter = 0f;
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

        PlayJumpSound();
    }
    #endregion

    #region Movement Tech

    private void ApplyCounterMovement()
    {
        if (!grounded || moveDirection.magnitude > 0.1f)
            return;

        Vector3 flatVel = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        Vector3 counterForce = -flatVel * 10f; // Adjust factor to taste
        rb.AddForce(counterForce, ForceMode.Force);
    }

    private void Airtime()
    {
        if (!grounded)
        {
            airTime += Time.deltaTime;
            airTime = Mathf.Min(airTime, maxAirTime);
        }
        else
        {
            airTime = 0f;
        }

        float normalizedTime = airTime / maxAirTime;
        float offsetY = normalizedTime * maxOffset;

        // Apply offset to arms
        Vector3 targetPos = armsInitialPos + new Vector3(0f, offsetY, 0f);
        arms.localPosition = Vector3.Lerp(arms.localPosition, targetPos, Time.deltaTime * 30f);
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

    private void ResetJump()
    {
        readyToJump = true;
        exitingSlope = false;
    }

    private void HandleMomentum()
    {
        if ((dashing || sliding) && moveDirection.magnitude > 0.1f)
        {
            momentum += momentumBuildRate * Time.deltaTime;
        }
        else if (momentum > 0f)
        {
            momentum -= momentumDecayRate * Time.deltaTime;
        }

        momentum = Mathf.Clamp(momentum, 0f, maxMomentum);
        moveSpeed += momentum * momentumSpeedMultiplier;
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

    public bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            if (slopeHit.collider.CompareTag("Stairs"))
                return false;

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
    #endregion
    #endregion

    #region Feedback

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
        }
        return SurfaceType.Default;
    }

    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Heart")
        {
            handAnimator.SetTrigger("Organ");
            //animator.SetBool("Idle", false);
            walkSpeed = 12;
            jumpForce = 20;
            normalSpeed = true;
            AudioManager.instance.PlaySFXOneShot("Heartbeat", 1f);
            AudioManager.instance.PlayMusic("Background Music", 1f);
            Destroy(other.gameObject);
        }

        if (other.gameObject.tag == "Stomach")
        {
            handAnimator.SetTrigger("Organ");
            handAnimator.SetBool("Idle", false);
            canDoubleJump = true;
            AudioManager.instance.PlaySFXOneShot("Stomach", 1f);
            handAnimator.SetBool("Idle", true);
            lungsPosition.SetActive(true);
            Destroy(other.gameObject);

        }
        if (other.gameObject.tag == "Lungs")
        {
            handAnimator.SetTrigger("Organ");
            handAnimator.SetBool("Idle", false);
            canDash = true;
            AudioManager.instance.PlaySFXOneShot("Lungs", 1f);
            handAnimator.SetBool("Idle", true);
            stomachPosition.SetActive(true);
            Destroy(other.gameObject);

        }
    }
}
