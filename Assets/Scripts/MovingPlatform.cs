using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Platform Properties")]
    [SerializeField] protected float moveSpeed = 1f;
    [SerializeField] protected float moveDistance = 2f;
    [SerializeField] protected float directionInDegrees = 0f;

    protected Vector2 direction;
    protected Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;

        float angleInRadians = directionInDegrees * Mathf.Deg2Rad;
        direction = new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians)).normalized;
    }

    void FixedUpdate()
    {
        // Oscillate back and forth using sine wave
        float oscillation = Mathf.Sin(Time.time * moveSpeed) * moveDistance;
        Vector3 targetOffset = direction * oscillation;
        Vector3 targetPosition = startPosition + targetOffset;

        // Optional lerp to smooth out movement
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSpeed);
    }

}