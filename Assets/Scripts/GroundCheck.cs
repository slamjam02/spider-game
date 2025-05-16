using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{

    [SerializeField] protected BoxCollider2D boxCollider2D;
    public bool check = false;
    public float timeSinceGrounded = -Mathf.Infinity;

    public float coyoteTime = 0f;

    void Update()
    {
        if (!check)
        {
            timeSinceGrounded += Time.deltaTime;
        }
        else
        {
            timeSinceGrounded = 0f; // Reset when grounded
        }
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
