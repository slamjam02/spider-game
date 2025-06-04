using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom : MonoBehaviour
{
    public float bounce = 10f;

    public Transform transform;

    void Start()
    {
        //transform = GetComponent(Transform);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collider Entered!");

        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                //rb.velocity = new Vector2(rb.velocity.x, bounce);
                rb.AddForce(new Vector2(0f, bounce), ForceMode2D.Impulse);

            }
        }
    }

    protected void OnTriggerExit2D(Collider2D other)
    {
        
    }
}

