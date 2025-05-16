using System;
using System.Collections.Generic;
using UnityEngine;
public class PlayerMovement : MonoBehaviour
{


    private Rigidbody2D rigidBody;
    private Transform transform;
    private Vector3 spawnPosition;
    private BoxCollider2D boxCollider;


    [Header("Check Objects")]
    [SerializeField] protected GroundCheck groundCheck;
    [SerializeField] protected WallCheck rightWallCheck;
    [SerializeField] protected WallCheck leftWallCheck;
    [SerializeField] protected WallCheck ceilingCheck;

    [Header("Layers")]
    [SerializeField] protected LayerMask wallLayer;
    [SerializeField] protected LayerMask groundLayer;
    [SerializeField] protected float checkRadiusOffset = 0.1f; // Offset below the collider

    [Header("Movement")]
    [SerializeField] protected float acceleration = 1000f;
    [SerializeField] protected float crawlingSpeed = 0.2f;
    private float maxVertSpeedTemp;
    [SerializeField] protected float terminalVelocity = 300f;
    [SerializeField] protected float maxPlayerSpeed = 12f;
    [SerializeField] protected float frictionCompensation = 2f;
    [SerializeField] float movementCooldownAfterWallJump = 0.1f;

    [Header("Jump")]
    [SerializeField] protected float jumpForce = 5f;
    [SerializeField] protected float wallJumpForce = 5f;
    [SerializeField] protected float ceilingJumpForce = 1f;
    [SerializeField] protected float jumpCooldown = 0.5f;
    [SerializeField] protected float coyoteTime = 0.05f;


    private float lastJumpTime = -Mathf.Infinity;
    private float lastWallJumpTime = -Mathf.Infinity;
    protected Vector2 inputDirection;

    private float gravityScaleStorage = 0f;
    public static PlayerMovement Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        transform = GetComponent<Transform>();
        spawnPosition = transform.position;

        gravityScaleStorage = rigidBody.gravityScale;

        groundCheck.coyoteTime = coyoteTime;
        rightWallCheck.coyoteTime = coyoteTime;
        leftWallCheck.coyoteTime = coyoteTime;
        ceilingCheck.coyoteTime = coyoteTime;
    }

    void Update()
    {
        if (Time.timeScale == 1f)
        {

            inputDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

            if (OnCeiling() || OnWall())
            {
                rigidBody.gravityScale = 0f;
            }
            else
            {
                rigidBody.gravityScale = gravityScaleStorage;
            }

            // Terminal velocity limits
            if (rigidBody.velocity.x > terminalVelocity)
            {
                rigidBody.velocity = new Vector2(terminalVelocity, rigidBody.velocity.y);
            }
            if (rigidBody.velocity.x < -terminalVelocity)
            {
                rigidBody.velocity = new Vector2(-terminalVelocity, rigidBody.velocity.y);
            }
            if (rigidBody.velocity.y > terminalVelocity)
            {
                rigidBody.velocity = new Vector2(rigidBody.velocity.x, terminalVelocity);
            }
            if (rigidBody.velocity.y < -terminalVelocity)
            {
                rigidBody.velocity = new Vector2(rigidBody.velocity.x, -terminalVelocity);
            }


            if (groundCheck.canJump() && Input.GetKeyDown(KeyCode.Space) && Time.time > lastJumpTime + jumpCooldown)
                {
                    Jump(new Vector2(0, 1), jumpForce);
                }

            else if ((leftWallCheck.canJump() || rightWallCheck.canJump()) && Input.GetKeyDown(KeyCode.Space) && Time.time > lastWallJumpTime + jumpCooldown)
            {
                
                // Player sliding against all to the right
                if (rightWallCheck.canJump())
                {
                    if (inputDirection == Vector2.down || inputDirection == Vector2.up)
                    {
                        Jump(new Vector2(-0.2f, inputDirection.y).normalized, jumpForce * 0.5f);
                        lastWallJumpTime = Time.time;
                    }
                    else
                    {
                        Jump(new Vector2(-1f, 2f).normalized, wallJumpForce);
                        lastWallJumpTime = Time.time;
                    }
                }

                // Player sliding against all to the left
                if (leftWallCheck.canJump())
                {
                    if (inputDirection == Vector2.down || inputDirection == Vector2.up)
                    {
                        Jump(new Vector2(0.2f, inputDirection.y).normalized, jumpForce * 0.5f);
                        lastWallJumpTime = Time.time;
                    }
                    else
                    {
                        Jump(new Vector2(1f, 2f).normalized, wallJumpForce);
                        lastWallJumpTime = Time.time;
                    }
                }
            }

            else if (ceilingCheck.canJump() && Input.GetKeyDown(KeyCode.Space) && Time.time > lastWallJumpTime + jumpCooldown)
            {
                //ResetMovement();
                Jump(new Vector2(0, -1), ceilingJumpForce);
                lastWallJumpTime = Time.time;
            }

        }
    }

    void FixedUpdate()
    {
        if (Time.time > lastWallJumpTime + movementCooldownAfterWallJump)
        {
            Move(inputDirection);
        }

    }

    protected void Move(Vector2 moveDirection)
    {
            
        // Ground or air movement
        if (OnGround() || Airborne())
        {
            Vector2 forceToAdd = Vector2.right * moveDirection.x * acceleration;
            
            if ((rigidBody.velocity.x > maxPlayerSpeed && moveDirection.x < 0) ||
                (rigidBody.velocity.x < -maxPlayerSpeed && moveDirection.x > 0) ||
                Mathf.Abs(rigidBody.velocity.x) <= maxPlayerSpeed)
            {
                rigidBody.AddForce(forceToAdd, ForceMode2D.Force);
            }
        }
        
        // Wall movement (only vertical)
        if (OnWall())
        {
            rigidBody.velocity = new Vector2(0, moveDirection.y * crawlingSpeed);
            return; // Avoid conflicting movement logic
        }

        // Ceiling movement (horizontal and vertical)
        if (OnCeiling())
        {
            rigidBody.velocity = new Vector2(moveDirection.x * crawlingSpeed, moveDirection.y * crawlingSpeed);
            return;
        }

        
    }

    protected void ResetMovement()
    {
        rigidBody.velocity = Vector2.zero;
    }

    protected void Jump(Vector2 direction, float force)
    {
        rigidBody.AddForce(direction * force, ForceMode2D.Impulse);
        lastJumpTime = Time.time;
    }

    protected bool Airborne()
    {
        return !(OnWall() || OnCeiling() || OnGround());
    }

    protected bool OnGround()
    {
        return groundCheck.check;
    }

    protected bool OnWall()
    {
        return rightWallCheck.check || leftWallCheck.check;
    }
    protected bool OnCeiling()
    {
        return ceilingCheck.check;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Death")
        {
            Debug.Log("Player hit death barrier");
            transform.position = spawnPosition;
        }
    }
}