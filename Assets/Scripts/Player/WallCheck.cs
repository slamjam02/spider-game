using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCheck : ColliderCheck
{
    protected void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Wall")
        {
            this.check = true;
            this.onMovingPlatform = true;
            this.attached = other;
        }

    }

    protected void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Wall")
        {
            this.check = false;
            this.onMovingPlatform = false;
            this.attached = null;
        }
    }
}