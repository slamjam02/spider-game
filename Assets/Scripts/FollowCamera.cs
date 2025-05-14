using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] protected GameObject followObject;
    [SerializeField] protected float followRadius = 5f;
    [SerializeField] protected float smoothTime = 0.15f; // Adjust for the delay and smoothness
    [SerializeField] protected float maxSpeed = 100f;    // Limit the maximum camera speed

    private Transform cameraTransform;
    private Transform followTransform;
    private Vector3 offset;
    private Vector3 currentVelocity; // Store the camera's current velocity

    void Start()
    {
        cameraTransform = GetComponent<Transform>();
        if (followObject != null)
        {
            followTransform = followObject.GetComponent<Transform>();
            offset = cameraTransform.position - followTransform.position;
        }
        else
        {
            Debug.LogError("Follow Object is not assigned in the Inspector!");
            enabled = false;
        }
        currentVelocity = Vector3.zero; // Initialize velocity to zero
    }

    void LateUpdate()
    {
        if (followTransform == null) return;

        Vector3 targetPosition = followTransform.position + offset;
        float distance = Vector3.Distance(cameraTransform.position, followTransform.position);

        if (distance > followRadius)
        {
            // Smoothly move towards the target position with velocity
            cameraTransform.position = Vector3.SmoothDamp(cameraTransform.position, targetPosition, ref currentVelocity, smoothTime, maxSpeed);
        }
        else
        {
            // Directly maintain the offset when within the radius
            cameraTransform.position = targetPosition;
            currentVelocity = Vector3.zero; // Reset velocity when within radius
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (followObject != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(followObject.transform.position, followRadius);
        }
    }
}