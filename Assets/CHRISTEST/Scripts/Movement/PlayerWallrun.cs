using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallrun : MonoBehaviour
{
    #region Variables
    [Header("Wallrunning")]
    public LayerMask whatIsWall;
    public LayerMask whatIsGround;
    public float wallRunForce = 600f;
    public float wallJumpUpForce = 8f;
    public float wallJumpSideForce = 8f;
    public float wallClimbSpeed = 8f;
    public float maxWallRunTime = 1.5f;
    private float wallRunTimer;

    [Header("Camera")]
    public float wallRunFov = 100f;
    public float normalFov = 90f;
    private float currentTilt = 0f;

    [Header("Input")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode upwardsRunKey = KeyCode.LeftShift;
    public KeyCode downwardsRunKey = KeyCode.LeftControl;

    private bool upwardsRunning;
    private bool downwardsRunning;
    private float horizontalInput;
    private float verticalInput;

    [Header("Detection")]
    public float wallCheckDistance = 0.6f;
    public float minJumpHeight = 1.5f;
    private RaycastHit leftWallhit;
    private RaycastHit rightWallhit;
    private bool wallLeft;
    private bool wallRight;

    [Header("Exiting")]
    public float exitWallTime = 0.3f;
    private float exitWallTimer;
    private bool exitingWall;

    [Header("Gravity")]
    public bool useGravity = true;
    public float gravityCounterForce = 20f;
    public float fallControlForce = 400f;

    [Header("References")]
    public Transform orientation;
    public PlayerCamera cam;
    private PlayerMovement pm;
    private Rigidbody rb;
    #endregion

    #region Start/Update

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        CheckForWall();
        HandleStateMachine();
    }

    private void FixedUpdate()
    {
        if (pm.wallrunning)
            WallRunningMovement();
    }

    #endregion

    #region Wallrunning Methods

    private void CheckForWall()
    {
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallhit, wallCheckDistance, whatIsWall);
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallhit, wallCheckDistance, whatIsWall);
    }

    private bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, whatIsGround);
    }

    private void HandleStateMachine()
    {
        // Get inputs
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        upwardsRunning = Input.GetKey(upwardsRunKey);
        downwardsRunning = Input.GetKey(downwardsRunKey);

        // Start wallrun
        if ((wallLeft || wallRight) && verticalInput > 0 && AboveGround() && !exitingWall)
        {
            if (!pm.wallrunning)
                StartWallRun();

            if (wallRunTimer > 0)
                wallRunTimer -= Time.deltaTime;
            else
                ExitWallRun();

            if (Input.GetKeyDown(jumpKey))
                WallJump();
        }
        else if (exitingWall)
        {
            if (pm.wallrunning)
                StopWallRun();

            exitWallTimer -= Time.deltaTime;
            if (exitWallTimer <= 0)
                exitingWall = false;
        }
        else
        {
            if (pm.wallrunning)
                StopWallRun();
        }
    }

    private void StartWallRun()
    {
        cam.DoFov(wallRunFov);

        currentTilt = wallLeft ? -5f : wallRight ? 5f : 0f;
        cam.DoTilt(currentTilt);

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.drag = 0f; // Prevent artificial friction
        pm.wallrunning = true;
        wallRunTimer = maxWallRunTime;
    }

    private void WallRunningMovement()
    {
        rb.useGravity = useGravity;

        Vector3 wallNormal = wallRight ? rightWallhit.normal : leftWallhit.normal;
        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if (Vector3.Dot(orientation.forward, wallForward) < Vector3.Dot(orientation.forward, -wallForward))
            wallForward = -wallForward;

        rb.AddForce(wallForward * wallRunForce * Time.fixedDeltaTime, ForceMode.Force);

        // Climb up/down
        if (upwardsRunning)
            rb.velocity = new Vector3(rb.velocity.x, wallClimbSpeed, rb.velocity.z);
        else if (downwardsRunning)
            rb.velocity = new Vector3(rb.velocity.x, -wallClimbSpeed, rb.velocity.z);

        // Stick to wall
        bool pushingAway = (wallLeft && horizontalInput > 0) || (wallRight && horizontalInput < 0);
        if (!pushingAway)
            rb.AddForce(-wallNormal * 200, ForceMode.Force);

        // Counteract gravity
        if (useGravity)
            rb.AddForce(Vector3.up * fallControlForce, ForceMode.Force);
    }

    private void StopWallRun()
    {
        pm.wallrunning = false;
        cam.DoFov(normalFov);
        cam.DoTilt(0f);

        rb.useGravity = true;
        rb.drag = 1f; // Restore natural air drag

        currentTilt = 0f;
    }

    private void ExitWallRun()
    {
        exitingWall = true;
        exitWallTimer = exitWallTime;
    }

    private void WallJump()
    {
        ExitWallRun();

        Vector3 wallNormal = wallRight ? rightWallhit.normal : leftWallhit.normal;
        Vector3 wallForward = Vector3.Cross(wallNormal, Vector3.up);

        if (Vector3.Dot(orientation.forward, wallForward) < Vector3.Dot(orientation.forward, -wallForward))
            wallForward = -wallForward;

        // Optional: Slight upward bias to maintain vertical control
        Vector3 jumpDirection = (wallForward + Vector3.up * 0.5f).normalized;

        // Reset vertical velocity before jump
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // Add jump force in desired direction
        rb.AddForce(jumpDirection * wallJumpSideForce + Vector3.up * wallJumpUpForce, ForceMode.Impulse);

        // Add small wall pushback to ensure clearance from surface
        rb.AddForce(wallNormal * 2f, ForceMode.Impulse);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, orientation.right * wallCheckDistance);
        Gizmos.DrawRay(transform.position, -orientation.right * wallCheckDistance);
    }
    #endregion
}
