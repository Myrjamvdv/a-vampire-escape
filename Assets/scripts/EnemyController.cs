using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // In world units / s, I think?
    private const float IDLE_SPEED = 1.2f;
    private const int AMOUNT_OF_GOALS = 2;
    private const float GOAL_APPROXIMATION_MARGIN = 1f;

    private const float ATTACK_RANGE = 7;
    private const float ATTACK_SPEED = 2.5f;

    private const int STATE_IDLE = 0;
    private const int STATE_ATTACKING = 1;
    private const int STATE_DED = 2;

    public GameObject idleCollider;
    public GameObject attackingCollider;
    public GameObject vampire;

    private Animator animator;
    private Vector2[] goals;
    private int currentGoalIdx;
    private int state;
    private bool oldDirectionWasRight;

    void Start ()
    {
        animator = GetComponent<Animator> ();
        goals = new Vector2[AMOUNT_OF_GOALS];
        goals [0] = transform.position;
        goals [1] = transform.position + 5 * (Vector3)Vector2.right;
        currentGoalIdx = 1;
        state = STATE_IDLE;
        oldDirectionWasRight = true;
    }

    void FixedUpdate ()
    {
        UpdateState (FindAppropriateState ());
        var oldPosition = transform.position;
        DoAppropriateAction ();
        UpdateAnimation (oldPosition);
    }

    void OnCollisionEnter2D (Collision2D collision)
    {
        if (collision.gameObject == vampire) {
            vampire.SendMessage ("Die");
        }
    }

    public void Die ()
    {
        Debug.Log ("Nice job! You killed him!");
        UpdateState (STATE_DED);
    }

    private int FindAppropriateState ()
    {
        if (state == STATE_DED) {
            return STATE_DED;
        } else if (IsVampireWithinRange ()) {
            return STATE_ATTACKING;
        } else {
            return STATE_IDLE;
        }
    }

    private void DoAppropriateAction ()
    {
        if (state == STATE_IDLE) {
            WalkAround ();
        } else if (state == STATE_ATTACKING) {
            Attack ();
        } // Don't do anything if you're ded
    }

    private void WalkAround ()
    {
        transform.position = Vector2.MoveTowards (transform.position, goals [currentGoalIdx], IDLE_SPEED * Time.fixedDeltaTime);
        // If we're approximately there, move to the next goal
        if (Vector2.Distance (transform.position, goals [currentGoalIdx]) < GOAL_APPROXIMATION_MARGIN) {
            currentGoalIdx = (currentGoalIdx + 1) % AMOUNT_OF_GOALS;
        }
    }

    private void Attack ()
    {
        transform.position = Vector2.MoveTowards (transform.position, vampire.transform.position, ATTACK_SPEED * Time.fixedDeltaTime);
    }

    private void UpdateState (int newState)
    {
        state = newState;
        animator.SetInteger ("state", state);
        if (state == STATE_IDLE) {
            idleCollider.SetActive (true);
            attackingCollider.SetActive (false);
        } else if (state == STATE_ATTACKING) {
            idleCollider.SetActive (false);
            attackingCollider.SetActive (true);
        } else if (state == STATE_DED) {
            idleCollider.SetActive (false);
            attackingCollider.SetActive (false);
            GetComponent<Rigidbody2D> ().isKinematic = true;
        }
    }

    private bool IsVampireWithinRange ()
    {
        return Vector2.Distance (transform.position, vampire.transform.position) < ATTACK_RANGE;
    }

    private void UpdateAnimation (Vector2 oldPosition)
    {
        if (transform.position.x == oldPosition.x) {
            return;
        }

        var directionIsRight = transform.position.x > oldPosition.x;
        if (directionIsRight != oldDirectionWasRight) {
            Flip ();
        }
        oldDirectionWasRight = directionIsRight;
    }

    private void Flip ()
    {
        var scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
