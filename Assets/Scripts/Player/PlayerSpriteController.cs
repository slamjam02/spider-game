using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpriteController : MonoBehaviour
{
    [SerializeField] protected PlayerMovement playerMovement;
    [SerializeField] protected Animator animator;
    private Transform transform;

    private float offset;


    private bool lastFrameOnWall;
    void Start()
    {
        transform = GetComponent<Transform>();
        lastFrameOnWall = false;
        animator = GetComponent<Animator>();
        
        Vector3 localPos = Vector3.zero;
        offset = localPos.y; // Adjust this based on sprite size
    }

    // Update is called once per frame
    void Update()
    {
        AdjustFaceAndRotation();
        AdjustAnchorPosition();
        ControlSprite();
    }

    void ControlSprite()
    {
        if (playerMovement.isWalking)
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }

    void AdjustAnchorPosition()
    {
        Vector3 localPos = Vector3.zero;
        
        if (playerMovement.OnCeiling())
        {
            localPos.y = -offset;
        }
        else if (playerMovement.leftWallCheck.check)
        {
            localPos.x = offset;
        }
        else if (playerMovement.rightWallCheck.check)
        {
            localPos.x = -offset;
        }
        else
        {
            localPos.y = offset; // standing upright
        }

        transform.localPosition = localPos;
    }
    void AdjustFaceAndRotation()
    {
        Vector3 scale = transform.localScale;

        if (playerMovement.OnCeiling() || playerMovement.Airborne() || playerMovement.OnGround())
        {
            transform.rotation = Quaternion.Euler(1, 1, 1);
            if (playerMovement.facingLeft)
            {
                scale.x = -Mathf.Abs(scale.x);
            }
            else
            {
                scale.x = Mathf.Abs(scale.x);
            }
        }

        if (playerMovement.OnCeiling())
        {
            scale.y = -Mathf.Abs(scale.y);
        }
        else
        {
            scale.y = Mathf.Abs(scale.y);
        }

        if (playerMovement.OnWall())
        {
            if (!lastFrameOnWall)
            {
                playerMovement.facingUp = true;
                lastFrameOnWall = true;
            }
        }
        else
        {
            lastFrameOnWall = false;
        }


        if (playerMovement.leftWallCheck.check)
        {
            // Rotate to face left wall (example: 0, -90, 0)
            transform.rotation = Quaternion.Euler(0f, 0f, -90f);
            if (playerMovement.facingUp)
            {
                scale.x = -Mathf.Abs(scale.x);
            }
            else
            {
                scale.x = Mathf.Abs(scale.x);
            }
        }
        else if (playerMovement.rightWallCheck.check)
        {
            // Rotate to face right wall (example: 0, 90, 0)
            transform.rotation = Quaternion.Euler(0f, 0f, 90f);
            if (playerMovement.facingUp)
            {
                scale.x = Mathf.Abs(scale.x);
            }
            else
            {
                scale.x = -Mathf.Abs(scale.x);
            }
        }
        else
        {
            // Default rotation when not on wall
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }

        transform.localScale = scale;
    }
}
