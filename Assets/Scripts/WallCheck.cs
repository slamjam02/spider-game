using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCheck : MonoBehaviour
{

    [SerializeField] protected BoxCollider2D boxCollider2D;
    public bool check = false;

    private void OnTriggerStay2D(Collider2D other)
    {
        if(other.tag == "Wall") {this.check = true;}
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.tag == "Wall") {this.check = false;}
    }
}
