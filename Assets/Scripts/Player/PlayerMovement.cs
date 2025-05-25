using System;
using System.Collections.Generic;
using UnityEngine;
public class PlayerMovement : MonoBehaviour
{


    private Rigidbody2D rigidBody;
    private Transform transform;
    private Vector3 spawnPosition;
    public Collider2D attachedMovingObject;

    [Header("Check Objects")]
    [SerializeField] public GroundCheck groundCheck;
    [SerializeField] public WallCheck rightWallCheck;
    [SerializeField] public WallCheck leftWallCheck;
    [SerializeField] public WallCheck ceilingCheck;

    [Header("Layers")]
    [SerializeField] protected LayerMask wallLayer;
    [SerializeField] protected LayerMask groundLayer;
    // [SerializeField] protected float checkRadiusOffset = 0.1f; // Offset below the collider

    [Header("Movement")]
    [SerializeField] protected float acceleration = 1000f;
    [SerializeField] protected float maxPlayerSpeed = 12f;
    [SerializeField] protected float crawlingSpeed = 0.2f;
    private float maxVertSpeedTemp;
    [SerializeField] protected float terminalVelocity = 300f;
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

    [Header("Current Stats")]
    [SerializeField] protected Vector2 inputDirection;
    [SerializeField] protected Vector2 currentVelocity;
    public bool facingLeft;
    public bool facingUp;
    public bool isWalking;
    public bool hasJumped;

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
        transform = GetComponent<Transform>();
        spawnPosition = transform.position;

        gravityScaleStorage = rigidBody.gravityScale;

        groundCheck.coyoteTime = coyoteTime;
        rightWallCheck.coyoteTime = coyoteTime;
        leftWallCheck.coyoteTime = coyoteTime;
        ceilingCheck.coyoteTime = coyoteTime;

        facingLeft = false;
        facingUp = true;
        isWalking = false;
    }

    void Update()
    {
        if (Time.timeScale > 0f)
        {
            hasJumped = false;

            inputDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
            currentVelocity = rigidBody.velocity;

            if ((rigidBody.velocity.x != 0f || rigidBody.velocity.y != 0f) && !Airborne())
            {
                isWalking = true;
            }
            else
            {
                isWalking = false;
            }

            if (inputDirection.x > 0)
            {
                facingLeft = false;
            }
            if (inputDirection.x < 0)
            {
                facingLeft = true;
            }
            // if (inputDirection.y > 0)
            // {
            //     facingUp = true;
            // }
            // if (inputDirection.y < 0)
            // {
            //     facingUp = false;
            // }


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


            if (ceilingCheck.canJump() && Input.GetKeyDown(KeyCode.Space) && Time.time > lastWallJumpTime + jumpCooldown)
            {
                //ResetMovement();
                Jump(new Vector2(0, -1), ceilingJumpForce);
                
                lastWallJumpTime = Time.time;
            }


            else if ((leftWallCheck.canJump() || rightWallCheck.canJump()) && Input.GetKeyDown(KeyCode.Space) && Time.time > lastWallJumpTime + jumpCooldown)
            {
                // Player sliding against all to the right
                if (rightWallCheck.canJump())
                {
                    if (inputDirection.x < 0)
                    {
                        Jump(new Vector2(-1f, 2f).normalized, wallJumpForce);
                        lastWallJumpTime = Time.time;
                    }
                    else
                    {
                        Jump(new Vector2(-0.5f, 2f).normalized, wallJumpForce);
                        lastWallJumpTime = Time.time;
                    }

                    // if (inputDirection == Vector2.down || inputDirection == Vector2.up)
                    // {
                    //     Jump(new Vector2(-0.2f, inputDirection.y).normalized, jumpForce * 0.5f);
                    //     lastWallJumpTime = Time.time;
                    // }
                    // else
                    // {
                    //     Jump(new Vector2(-1f, 2f).normalized, wallJumpForce);
                    //     lastWallJumpTime = Time.time;
                    // }
                }

                // Player sliding against all to the left
                if (leftWallCheck.canJump())
                {
                    if (inputDirection.x > 0)
                    {
                        Jump(new Vector2(1f, 2f).normalized, wallJumpForce);
                        lastWallJumpTime = Time.time;
                    }
                    else
                    {
                        Jump(new Vector2(0.5f, 2f).normalized, wallJumpForce);
                        lastWallJumpTime = Time.time;
                    }
                    // if (inputDirection == Vector2.down || inputDirection == Vector2.up)
                    // {
                    //     Jump(new Vector2(0.2f, inputDirection.y).normalized, jumpForce * 0.5f);
                    //     lastWallJumpTime = Time.time;
                    // }
                    // else
                    // {
                    //     Jump(new Vector2(1f, 2f).normalized, wallJumpForce);
                    //     lastWallJumpTime = Time.time;
                    // }
                }
            }



            else if (groundCheck.canJump() && Input.GetKeyDown(KeyCode.Space) && Time.time > lastJumpTime + jumpCooldown)
            {
                Jump(new Vector2(0, 1), jumpForce);
            }

        }
    }

    void FixedUpdate()
    {
        if (Time.time > lastWallJumpTime + movementCooldownAfterWallJump)
        {
            Move(inputDirection);
        }
        checkOnMovingObject();

    }

    protected void Move(Vector2 moveDirection)
    {
        
        // Wall movement (only vertical)
        if (OnWall())
        {
            float verticalInput = moveDirection.y;

            // Allow horizontal direction to assist vertical wall movement
            if (leftWallCheck.check)
            {
                if (moveDirection.x < 0 || moveDirection.y > 0) // left or up
                {
                    verticalInput = 1;
                    facingUp = true;
                }
                else if (moveDirection.x > 0 || moveDirection.y < 0) // right or down
                {
                    verticalInput = -1;
                    facingUp = false;
                }
            }
            else // right wall
            {
                if (moveDirection.x > 0 || moveDirection.y > 0) // right or up
                {
                    verticalInput = 1;
                    facingUp = true;
                }
                else if (moveDirection.x < 0 || moveDirection.y < 0) // left or down
                {
                    verticalInput = -1;
                    facingUp = false;
                }
            }

            if (facingUp || !OnGround())
            {
                rigidBody.velocity = new Vector2(0, verticalInput * crawlingSpeed);
            }
        }

        // Ceiling movement (horizontal and vertical)
        if (OnCeiling())
        {
            rigidBody.velocity = new Vector2(moveDirection.x * crawlingSpeed, moveDirection.y * crawlingSpeed);
        }
        
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
        
    }

    protected void checkOnMovingObject()
    {
        if (groundCheck.onMovingPlatform)
        {
            attachedMovingObject = groundCheck.attached;
        }
        else if (leftWallCheck.onMovingPlatform)
        {
            attachedMovingObject = leftWallCheck.attached;

        }
        else if (rightWallCheck.onMovingPlatform)
        {
            attachedMovingObject = rightWallCheck.attached;

        }
        else if (ceilingCheck.onMovingPlatform)
        {
            attachedMovingObject = ceilingCheck.attached;

        }
        else
        {
            attachedMovingObject = null;
        }

        if (attachedMovingObject != null)
        {
            this.transform.SetParent(attachedMovingObject.transform);
        }
        else
        {
            this.transform.SetParent(null);
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
        hasJumped = true;
    }

    public bool Airborne()
    {
        return !(OnWall() || OnCeiling() || OnGround());
    }

    public bool OnGround()
    {
        return groundCheck.check;
    }

    public bool OnWall()
    {
        return rightWallCheck.check || leftWallCheck.check;
    }
    public bool OnCeiling()
    {
        return ceilingCheck.check;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Death")
        {
            transform.position = spawnPosition;
        }
    }
}