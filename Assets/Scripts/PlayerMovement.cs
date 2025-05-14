using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{


    private Rigidbody2D rigidBody;
    private BoxCollider2D boxCollider;

    [Header("Layers")]
    [SerializeField] protected LayerMask wallLayer;
    [SerializeField] protected LayerMask groundLayer;
    [SerializeField] protected float checkRadiusOffset = 0.1f; // Offset below the collider

    [Header("Movement")]
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float friction;

    [Header("Jump")]
    [SerializeField] protected float jumpForce = 5f;
    [SerializeField] protected float jumpCooldown = 0.5f;


    private float lastJumpTime = -Mathf.Infinity;
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
    }

    void Update()
    {
        if (Time.timeScale == 1f)
        {

            // Get the center position of the collider in world space
            Vector3 center = boxCollider.transform.TransformPoint(boxCollider.offset);
            // Get the size of the collider in world space
            Vector3 size = boxCollider.transform.TransformVector(boxCollider.size);
            // Get edge positions of box collider
            float leftX = center.x - (size.x / 2f);
            float rightX = center.x + (size.x / 2f);
            float topY = center.y + (size.y / 2f);
            float bottomY = center.y - (size.y / 2f);

            if (IsGrounded() && Input.GetKeyDown(KeyCode.Space) && Time.time > lastJumpTime + jumpCooldown)
            {
                Jump();
                lastJumpTime = Time.time;
            }
            
        }
    }

    void FixedUpdate()
    {
        Vector2 moveDirection = new Vector2(Input.GetAxis("Horizontal"), 0);
        Move(moveDirection);
    }

    protected void Move(Vector2 moveDirection)
    {
        


        float targetVelocityX = moveDirection.x * moveSpeed;
        float currentVelocityX = rigidBody.velocity.x;

        float velocityChange = targetVelocityX - currentVelocityX;
        float force = velocityChange / Time.fixedDeltaTime;

        rigidBody.AddForce(Vector2.right * force, ForceMode2D.Force);

        if (Mathf.Abs(moveDirection.x) < 0.01f)
        {
            rigidBody.velocity = new Vector2(rigidBody.velocity.x * (1 - friction * Time.fixedDeltaTime), rigidBody.velocity.y);
        }
        

    }


    
    protected void Jump()
    {
        rigidBody.velocity = new Vector2(rigidBody.velocity.x, jumpForce);
    }


    protected bool IsGrounded()
    {
        Vector2 groundCheckPosition = new Vector2(transform.position.x, transform.position.y - boxCollider.bounds.extents.y - checkRadiusOffset);
        float checkRadius = 0.05f;
        return Physics2D.OverlapCircle(groundCheckPosition, checkRadius, groundLayer);
    }

    // This function is unfinished, it doesn't really do anything yet
    protected bool IsAgainstWallSide()
    {
        Vector2 wallCheckPosition = new Vector2(transform.position.x - boxCollider.bounds.extents.x - checkRadiusOffset, transform.position.y);
        float checkRadius = 0.05f;
        return Physics2D.OverlapCircle(wallCheckPosition, checkRadius, wallLayer); 
    }
}