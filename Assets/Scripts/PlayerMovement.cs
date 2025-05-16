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
    [SerializeField] protected float walkingSpeed = 12f;
    private float maxVertSpeedTemp;
    [SerializeField] protected float maxHoriSpeed = 12f;
    [SerializeField] protected float maxVertSpeed = 12f;
    [SerializeField] protected float frictionCompensation = 2f;
    [SerializeField] protected float slideSpeed = 0.1f;
    [SerializeField] float movementCooldownAfterWallJump = 0.1f;

    [Header("Jump")]
    [SerializeField] protected float jumpForce = 5f;
    [SerializeField] protected float wallJumpForce = 5f;
    [SerializeField] protected float ceilingJumpForce = 1f;

    [SerializeField] protected float jumpCooldown = 0.5f;
    [SerializeField] protected float coyoteTime = 0.05f;

    private float lastJumpTime = -Mathf.Infinity;
    private float lastWallJumpTime = -Mathf.Infinity;
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
            if (OnCeiling() || OnWall())
            {
                rigidBody.gravityScale = 0f;
            }
            else
            {
                rigidBody.gravityScale = gravityScaleStorage;
            }

            // Speed checks
            if (rigidBody.velocity.x > maxHoriSpeed)
            {
                rigidBody.velocity = new Vector2(maxHoriSpeed, rigidBody.velocity.y);
            }
            if (rigidBody.velocity.x < -maxHoriSpeed)
            {
                rigidBody.velocity = new Vector2(-maxHoriSpeed, rigidBody.velocity.y);
            }
            if (rigidBody.velocity.y > maxVertSpeed)
            {
                rigidBody.velocity = new Vector2(rigidBody.velocity.x, maxVertSpeed);
            }
            if (rigidBody.velocity.y < -maxVertSpeed)
            {
                rigidBody.velocity = new Vector2(rigidBody.velocity.x, -maxVertSpeed);
            }

            if (groundCheck.canJump() && Input.GetKeyDown(KeyCode.Space) && Time.time > lastJumpTime + jumpCooldown)
            {
                //ResetMovement();
                Jump(new Vector2(0, 1), jumpForce);
                lastJumpTime = Time.time;
            }

            if ((leftWallCheck.canJump() || rightWallCheck.canJump()) && Input.GetKeyDown(KeyCode.Space) && Time.time > lastWallJumpTime + jumpCooldown)
            {
                // Player sliding against all to the right
                if (rightWallCheck.canJump())
                {
                    ResetMovement();
                    Jump(new Vector2(-1, 1).normalized, wallJumpForce);
                    lastWallJumpTime = Time.time;
                }

                // Player sliding against all to the left
                if (leftWallCheck.canJump())
                {
                    ResetMovement();
                    Jump(new Vector2(1, 1).normalized, wallJumpForce);
                    lastWallJumpTime = Time.time;
                }
            }

            if (ceilingCheck.canJump() && Input.GetKeyDown(KeyCode.Space) && Time.time > lastWallJumpTime + jumpCooldown)
            {
                //ResetMovement();
                Jump(new Vector2(0, -1), ceilingJumpForce);
                lastWallJumpTime = Time.time;
            }

        }
    }

    void FixedUpdate()
    {


        // if (OnWall())
        // {
        //     SlideDownWall();
        // }

        Vector2 moveDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
        if (Time.time > lastWallJumpTime + movementCooldownAfterWallJump)
        {
            Move(moveDirection);
        }

    }

    protected void SlideDownWall()
    {
        transform.Translate(0f, -slideSpeed, 0f);
    }

    protected void Move(Vector2 moveDirection)
    {
        if (OnGround())
        {
            //rigidBody.AddForce(Vector2.up * frictionCompensation, ForceMode2D.Force);    // Compensate for friction
            rigidBody.AddForce(Vector2.right * moveDirection * acceleration, ForceMode2D.Force);
        }
        if (OnWall() || OnCeiling())
        {
            rigidBody.MovePosition(new Vector2(rigidBody.position.x + (moveDirection.x * walkingSpeed), rigidBody.position.y + (moveDirection.y * walkingSpeed)));
        }
        if (Airborne())
        {
            rigidBody.AddForce(Vector2.right * moveDirection * acceleration, ForceMode2D.Force);
        }
    }

    protected void ResetMovement()
    {
        rigidBody.velocity = Vector2.zero;
    }

    protected void Jump(Vector2 direction, float force)
    {
        rigidBody.AddForce(direction * force, ForceMode2D.Impulse);
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