using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{

    [SerializeField] protected BoxCollider2D boxCollider2D;
    public bool inGround = false;
    public float timeSinceGrounded = -Mathf.Infinity;

    void Update()
    {
        if (!inGround)
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
        //if(other.tag == "Ground") {
            this.inGround = true;
            //}    
    } 

    private void OnTriggerExit2D(Collider2D other)
    {
        //if(other.tag == "Ground") {
            this.inGround = false;
            //}    
    }
}
