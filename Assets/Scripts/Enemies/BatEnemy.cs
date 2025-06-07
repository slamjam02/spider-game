using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatEnemy : MonoBehaviour
{
    private enum State { Roam, Chase, Charge, Attack };
    private State currentState;

    [SerializeField] private GameObject playerObject;

    [Header("Distances")]
    [SerializeField] private float chaseDist = 6f;
    [SerializeField] private float roamDist = 10f;
    [SerializeField] private float attackDist = 2f;

    [Header("Speeds")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float friction = 0.1f;
    [SerializeField] private float chargeLiftSpeed = 3f;
    [SerializeField] private float attackSpeed = 7f;

    [Header("Timers")]
    [SerializeField] private float chargeTime = 1f;
    [SerializeField] private float attackDuration = 0.5f;
    [SerializeField] private float hitCooldown = 4f;

    private float hitCooldownEnd;
    private float chargeStartTime;
    private float attackStartTime;
    private bool beganCharging;
    private Vector2 attackDirection;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 roamTarget;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentState = State.Roam;
        roamTarget = GetRandomRoamPosition();
        beganCharging = false;
    }

    void Update()
    {
        float distToPlayer = Vector2.Distance(transform.position, playerObject.transform.position);

        switch (currentState)
        {
            case State.Roam:
                if (distToPlayer < chaseDist)
                {
                    currentState = State.Chase;
                }
                else if (Vector2.Distance(transform.position, roamTarget) < 0.5f)
                {
                    roamTarget = GetRandomRoamPosition();
                }
                MoveToward(roamTarget, moveSpeed);
                break;

            case State.Chase:
                if (distToPlayer > roamDist)
                {
                    currentState = State.Roam;
                    roamTarget = GetRandomRoamPosition();
                }
                else if (distToPlayer < attackDist)
                {
                    currentState = State.Charge;
                    beganCharging = false;
                }
                else
                {
                    MoveToward(playerObject.transform.position, moveSpeed);
                }
                break;

            case State.Charge:
                if (!beganCharging)
                {
                    chargeStartTime = Time.time;
                    beganCharging = true;
                    Debug.Log("Bat is charging...");
                }

                if (Time.time < chargeStartTime + chargeTime)
                {
                    rb.velocity = Vector2.up * chargeLiftSpeed;
                }
                else
                {
                    attackDirection = (playerObject.transform.position - transform.position).normalized;
                    attackStartTime = Time.time;
                    currentState = State.Attack;
                    beganCharging = false;
                    Debug.Log("Bat is attacking!");
                }
                break;

            case State.Attack:
                rb.velocity = attackDirection * attackSpeed;

                if (Time.time > attackStartTime + attackDuration)
                {
                    currentState = State.Roam;
                    roamTarget = GetRandomRoamPosition();
                }
                break;
        }
    }

    private Vector2 GetRandomRoamPosition()
    {
        Vector2 currentPos = transform.position;
        return currentPos + new Vector2(Random.Range(-roamDist, roamDist), Random.Range(-roamDist, roamDist));
    }

    private void MoveToward(Vector2 target, float speed)
    {
        Vector2 direction = (target - (Vector2)transform.position);
        if (direction.magnitude > 0.1f)
        {
            rb.velocity = direction.normalized * speed;
        }
        else
        {
            rb.velocity *= (1 - friction);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && currentState == State.Attack)
        {
            Debug.Log("Bat hit the player!");
            hitCooldownEnd = Time.time + hitCooldown;
            currentState = State.Roam;
            roamTarget = GetRandomRoamPosition();
        }
    }
}