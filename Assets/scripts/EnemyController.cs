using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // In world units / s, I think?
    private const float SPEED = 1.2f;
    private const int AMOUNT_OF_GOALS = 2;
    private const float GOAL_APPROXIMATION_MARGIN = 1f;

    private Vector2[] goals;
    private int currentGoalIdx;

    void Start ()
    {
        goals = new Vector2[AMOUNT_OF_GOALS];
        goals [0] = transform.position;
        goals [1] = transform.position + 5 * (Vector3)Vector2.right;
        currentGoalIdx = 1;
    }

    void FixedUpdate ()
    {
        WalkAround ();
    }

    void WalkAround ()
    {
        transform.position = Vector2.MoveTowards (transform.position, goals [currentGoalIdx], SPEED * Time.fixedDeltaTime);
        if (Vector2.Distance (transform.position, goals [currentGoalIdx]) < GOAL_APPROXIMATION_MARGIN) {
            currentGoalIdx = (currentGoalIdx + 1) % AMOUNT_OF_GOALS;
        }
    }
}
