using System;
using System.Collections.Generic;
using UnityEngine;
public class PlayerMovement : MonoBehaviour
{


    private Rigidbody2D rigidBody;
    private Transform transform;
    private BoxCollider2D boxCollider;
    [SerializeField] protected GroundCheck groundCheck;
    [SerializeField] protected WallCheck wallCheck;

    [Header("Layers")]
    [SerializeField] protected LayerMask wallLayer;
    [SerializeField] protected LayerMask groundLayer;
    [SerializeField] protected float checkRadiusOffset = 0.1f; // Offset below the collider

    [Header("Movement")]
    [SerializeField] protected float acceleration = 2f;
    [SerializeField] protected float maxSpeed = 2f;
    [SerializeField] protected float friction;
    [SerializeField] protected float slideSpeed = 0.1f;
    [SerializeField] float movementCooldownAfterWallJump = 0.1f;

    

    [Header("Jump")]
    [SerializeField] protected float jumpForce = 5f;
    [SerializeField] protected float wallJumpForce = 5f;
    [SerializeField] protected float jumpCooldown = 0.5f;
    [SerializeField] protected float coyoteTime = 0.05f;


    private float lastJumpTime = -Mathf.Infinity;
    private float lastWallJumpTime = -Mathf.Infinity;
    
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
    }

    void Update()
    {
        if (Time.timeScale == 1f)
        {
            if ((groundCheck.timeSinceGrounded < coyoteTime) && Input.GetKeyDown(KeyCode.Space) && Time.time > lastJumpTime + jumpCooldown)
            {
                Jump(new Vector2(0, 1), jumpForce);
                lastJumpTime = Time.time;
            }

            if (IsAgainstWallSide() && Input.GetKeyDown(KeyCode.Space) && Time.time > lastJumpTime + jumpCooldown){
                // Player sliding against all to the right
                if(Input.GetAxis("Horizontal") > 0){
                    ResetMovement();
                    Jump(new Vector2(-1, 1).normalized, wallJumpForce);
                    lastWallJumpTime = Time.time;
                }

                // Player sliding against all to the left
                if(Input.GetAxis("Horizontal") < 0) {
                    ResetMovement();
                    Jump(new Vector2(1, 1).normalized, wallJumpForce);
                    lastWallJumpTime = Time.time;
                }
            }

            if (rigidBody.velocity.x > maxSpeed) {
            rigidBody.velocity = new Vector2(maxSpeed, rigidBody.velocity.y);
            }
            if (rigidBody.velocity.x < -maxSpeed) {
                rigidBody.velocity = new Vector2(-maxSpeed, rigidBody.velocity.y);
            }
            
            
        } else {

        }
    }

    void FixedUpdate()
    {
        Vector2 moveDirection = new Vector2(Input.GetAxis("Horizontal"), 0);

        if(IsAgainstWallSide()){
            SlideDownWall();
        }

        if(Time.time > lastWallJumpTime + movementCooldownAfterWallJump){
            Move(moveDirection);
        }

    }

    protected void SlideDownWall() {
        transform.Translate(0f,-slideSpeed,0f);
    }

    protected void Move(Vector2 moveDirection)
    {

        rigidBody.AddForce(Vector2.right * moveDirection * acceleration, ForceMode2D.Force);

    }

    protected void ResetMovement() {
        rigidBody.velocity = Vector2.zero;
    }

    protected void Jump(Vector2 direction, float force)
    {
        rigidBody.AddForce(direction * force, ForceMode2D.Impulse);
    }


    protected bool IsAgainstWallSide()
    {
        return wallCheck.inWall;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Death") {
            
        }  
    } 
}