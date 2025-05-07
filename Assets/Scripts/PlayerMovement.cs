using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rigidBody;
    protected Vector2 moveDirection;
    private BoxCollider2D boxCollider;

    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float friction;
    [SerializeField] protected float jumpForce = 5f;
    [SerializeField] protected float jumpCooldown = 0.5f;
    [SerializeField] protected LayerMask groundLayer;
    [SerializeField] protected float groundCheckRadiusOffset = 0.1f; // Offset below the collider

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
            if (IsGrounded() && Input.GetKeyDown(KeyCode.Space) && Time.time > lastJumpTime + jumpCooldown)
            {
                Jump();
                lastJumpTime = Time.time;
            }
        }
    }

    void FixedUpdate()
    {
        moveDirection = new Vector2(Input.GetAxis("Horizontal"), 0);
        Move();
    }

    protected void Move()
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
        Vector2 groundCheckPosition = new Vector2(transform.position.x, transform.position.y - boxCollider.bounds.extents.y - groundCheckRadiusOffset);
        float checkRadius = 0.05f;
        return Physics2D.OverlapCircle(groundCheckPosition, checkRadius, groundLayer);
    }
}