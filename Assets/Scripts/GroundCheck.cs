using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{

    [SerializeField] protected BoxCollider2D boxCollider2D;
    public bool inGround;

    // Start is called before the first frame update
    void Start()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        this.inGround = true;
    } 

    private void OnTriggerExit2D(Collider2D other)
    {
        //if(collider.)
        this.inGround = false;
    }
}
