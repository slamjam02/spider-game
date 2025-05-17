using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCheck : MonoBehaviour
{

    [SerializeField] protected BoxCollider2D boxCollider2D;
    public bool check = false;
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


    private void OnTriggerStay2D(Collider2D other)
    {
        this.check = true;
    } 

    private void OnTriggerExit2D(Collider2D other)
    {
        this.check = false;
    }
}
