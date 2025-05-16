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
    [SerializeField] protected float jumpCooldown = 0.5f;
    [SerializeField] protected float coyoteTime = 0.05f;


    private float lastJumpTime = -Mathf.Infinity;
    private float lastWallJumpTime = -Mathf.Infinity;
    
    public static PlayerMovement Instance;

    public enum PlayerState {Idle, Run, Jump, Swing}
    public PlayerState currentState;

    private void OnEnterRunState() { 

    }
    private void UpdateRunState() { }
    private void OnExitRunState() { }

    private void OnEnterJumpState() { 
        maxVertSpeedTemp = maxVertSpeed;
        maxVertSpeed = Mathf.Infinity;

    }
    private void UpdateJumpState() { }
    private void OnExitJumpState() { 
        maxVertSpeed = maxVertSpeedTemp;
    }

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

        currentState = PlayerState.Idle;
    }

    void Update()
    {
        if (Time.timeScale == 1f)
        {
            if (Mathf.Abs(rigidBody.velocity.x) > 0.1f && groundCheck.inGround) {
                currentState = PlayerState.Run;
            }




            if (rigidBody.velocity.x > maxHoriSpeed) {
            rigidBody.velocity = new Vector2(maxHoriSpeed, rigidBody.velocity.y);
            }
            if (rigidBody.velocity.x < -maxHoriSpeed) {
                rigidBody.velocity = new Vector2(-maxHoriSpeed, rigidBody.velocity.y);
            }
            if (rigidBody.velocity.y > maxVertSpeed) {
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, maxVertSpeed);
            }
            if (rigidBody.velocity.y < -maxVertSpeed) {
                rigidBody.velocity = new Vector2(rigidBody.velocity.x, -maxVertSpeed);
            }
        
            if ((groundCheck.timeSinceGrounded < coyoteTime) && Input.GetKeyDown(KeyCode.Space) && Time.time > lastJumpTime + jumpCooldown)
            {
                Jump(new Vector2(0, 1), jumpForce);
                lastJumpTime = Time.time;
            }

            if (IsAgainstWallSide() && Input.GetKeyDown(KeyCode.Space) && Time.time > lastWallJumpTime + jumpCooldown){
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

            

            
            
            
        } else {

        }
    }

    void FixedUpdate()
    {
        

        if(IsAgainstWallSide()){
            SlideDownWall();
        }

        Vector2 moveDirection = new Vector2(Input.GetAxis("Horizontal"), 0);
        if(Time.time > lastWallJumpTime + movementCooldownAfterWallJump){
            Move(moveDirection);
        }

    }

    protected void SlideDownWall() {
        transform.Translate(0f,-slideSpeed,0f);
    }

    protected void Move(Vector2 moveDirection)
    {
        // if(rigidBody.velocity.x > walkingSpeed){
        //     if(moveDirection == Vector2.right){
        //         return;
        //     }
        // }
        // if(rigidBody.velocity.x < -walkingSpeed){
        //     if(moveDirection == Vector2.left){
        //         return;
        //     }
        // }

        if(groundCheck.inGround){
            // Compensate for friction
            rigidBody.AddForce(Vector2.up * frictionCompensation, ForceMode2D.Force);
            rigidBody.AddForce(Vector2.right * moveDirection * acceleration, ForceMode2D.Force);
        } else {
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
        currentState = PlayerState.Jump;
    }


    protected bool IsAgainstWallSide()
    {
        return rightWallCheck.check || leftWallCheck.check;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Death") {
            Debug.Log("Player hit death barrier");
            transform.position = spawnPosition;
        }  
    } 
}