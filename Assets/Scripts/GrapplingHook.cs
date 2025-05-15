using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{

    [SerializeField] private float grappleLength;
    [SerializeField] private LayerMask grappleLayer;
    [SerializeField] private LineRenderer rope;

    private Vector3 grapplePoint;
    //private DistanceJoint2D joint;
    private SpringJoint2D joint;
    private Rigidbody2D player;

    void Start(){
        //disables joints and rope since we don't have a grapple point yet
        //joint = gameObject.GetComponent<DistanceJoint2D>();
        joint = gameObject.GetComponent<SpringJoint2D>();
        joint.enabled = false;
        rope.enabled = false;
    }

    void Update(){
        //On Left Click, get click position
        if(Input.GetMouseButtonDown(0)){
            RaycastHit2D hit = Physics2D.Raycast(
                origin: Camera.main.ScreenToWorldPoint(Input.mousePosition),
                direction: Vector2.zero,
                distance: Mathf.Infinity,
                layerMask: grappleLayer
            );

            //If the Click is on a grapplable surface, set positions and enable joint and rope
            if(hit.collider != null){
                grapplePoint = hit.point;
                grapplePoint.z = 0;

                joint.connectedAnchor = grapplePoint;
                joint.enabled = true;
                joint.distance = grappleLength;
                
                rope.SetPosition(0, grapplePoint);
                rope.SetPosition(1, transform.position);
                rope.enabled = true;
            }
        }

        if(Input.GetMouseButtonDown(1)){
            RaycastHit2D hit = Physics2D.Raycast(
                origin: Camera.main.ScreenToWorldPoint(Input.mousePosition),
                direction: Vector2.zero,
                distance: Mathf.Infinity,
                layerMask: grappleLayer
            );

            //If the Click is on a grapplable surface, set positions and enable joint and rope
            if(hit.collider != null){
                grapplePoint = hit.point;
                grapplePoint.z = 0;

                joint.connectedAnchor = grapplePoint;
                joint.enabled = true;
                joint.distance = 10;
                
                rope.SetPosition(0, grapplePoint);
                rope.SetPosition(1, transform.position);
                rope.enabled = true;
            }
        }

        //When Left Click is released, disable joint and rope again
        if(Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1)){
            joint.enabled = false;
            rope.enabled = false;
            // player.AddForce
        }

        //Updates Rope Starting Pos to Track Player position
        if(rope.enabled == true){
            rope.SetPosition(1, transform.position);
        }
    }
}
