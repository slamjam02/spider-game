using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpriteController : MonoBehaviour
{
    [SerializeField] protected PlayerMovement playerMovement;
    private Transform transform;

    private bool lastFrameOnWall;
    void Start()
    {
        transform = GetComponent<Transform>();
        lastFrameOnWall = false;
    }

    // Update is called once per frame
    void Update()
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
