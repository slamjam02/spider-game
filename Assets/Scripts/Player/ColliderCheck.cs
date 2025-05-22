using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderCheck : MonoBehaviour
{

    [SerializeField] protected BoxCollider2D boxCollider2D;
    public bool check = false;
    public bool onMovingPlatform;
    public Collider2D attached;
    public float timeSinceGrounded = -Mathf.Infinity;

    public float coyoteTime = 0f;

    public bool canJump()
    {
        return timeSinceGrounded < coyoteTime;
    }

    void Start()
    {
        timeSinceGrounded = coyoteTime;
    }

    void FixedUpdate()
    {
        if (!check)
            timeSinceGrounded += Time.fixedDeltaTime;
        else
            timeSinceGrounded = 0f;
    }


    protected void OnTriggerStay2D(Collider2D other)
    {
        this.check = true;
        this.onMovingPlatform = true;
        this.attached = other;

    }

    protected void OnTriggerExit2D(Collider2D other)
    {
        this.check = false;
        this.onMovingPlatform = false;
        this.attached = null;
    }
}