using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatEnemy : MonoBehaviour
{

    //public Vector2 moveDirection;
    [SerializeField] protected GameObject playerObject;
    Vector2 targetPosition;
    private enum State { Roam, Chase, Charge, Attack };
    [SerializeField] private float chaseDist, roamDist, attackDist, moveSpeed, friction;
    State currentState;
    public Animator animator;
    public Rigidbody2D rigidBody;
    public Transform transform;

    // Start is called before the first frame update
    void Start()
    {
        currentState = State.Roam;
        targetPosition = (Vector2)transform.position + new Vector2(Random.Range(-roamDist, roamDist), Random.Range(-roamDist, roamDist));
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState == State.Roam)
        {
            if (Distance(gameObject, playerObject) < chaseDist)
            {
                currentState = State.Chase;
                Debug.Log("Bat state is now chase");
            }
            targetPosition = transform.position;
            //targetPosition = new Vector3(Mathf.Random(), Mathf.Random(), 0);
        }
        else if (currentState == State.Chase)
        {
            if (Distance(gameObject, playerObject) > roamDist)
            {
                currentState = State.Roam;
                Debug.Log("Bat state is now roam");
            }

            targetPosition = playerObject.transform.position;

        }
        else if (currentState == State.Attack)
        {
            Time attackStartTime;
        }
        else if (currentState == State.Charge)
        {

        }

        Move(targetPosition);

    }

    protected float Distance(GameObject self, GameObject other) {
        Vector3 selfPos = self.transform.position;
        Vector3 otherPos = other.transform.position;

        float selfX = selfPos.x;
        float selfY = selfPos.y;
        float otherX = otherPos.x;
        float otherY = otherPos.y;

        return Mathf.Sqrt((Mathf.Pow(otherX - selfX, 2f)) + (Mathf.Pow(otherY - selfY, 2f)));
    }

    protected void Move(Vector2 targetPos)
    {
        Vector2 currentPosition = transform.position;
        Vector2 direction = (targetPos - currentPosition).normalized;

        if ((targetPos - currentPosition).magnitude > 0.1f)
        {
            rigidBody.velocity = direction * moveSpeed;
        }
        else
        {
            rigidBody.velocity *= (1 - friction);
        }
    }
}
