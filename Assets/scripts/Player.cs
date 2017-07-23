using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public float restartDelay;
    public float moveSpeed;
    public float jumpSpeed;
    public float feetWidth;

    public GameObject feet;
    public LayerMask whatIsGround;

    private Rigidbody2D body;
    private Animator animator;

    void Start ()
    {
        body = GetComponent<Rigidbody2D> ();
        animator = GetComponent<Animator> ();
    }

    void FixedUpdate ()
    {
        HandleInput ();
    }

    void OnCollisionEnter2D (Collision2D collision)
    {
        if (collision.gameObject.tag == "deadlyOnTouch") {
            gameObject.SendMessage ("Die");
        }
    }

    private void HandleInput ()
    {
        if (Input.GetKey (KeyCode.Escape) || Input.GetKey (KeyCode.Q)) {
            Debug.Log ("Quitting.");
            Application.Quit ();
        }

        if (animator.GetInteger ("state") == FlashlightGun.STATE_DED) {
            return;
        }

        // Hero inputs from here on

        if (Input.GetKey (KeyCode.RightArrow)) {
            MoveRight ();
        } else if (Input.GetKey (KeyCode.LeftArrow)) {
            MoveLeft ();
        } else {
            var oldState = animator.GetInteger ("state");
            var forwards = oldState == FlashlightGun.STATE_IDLE_FORWARD || oldState == FlashlightGun.STATE_WALKING_FORWARD;
            animator.SetInteger ("state", forwards ? FlashlightGun.STATE_IDLE_FORWARD : FlashlightGun.STATE_IDLE_BACKWARD);
        }

        if (Input.GetKey (KeyCode.UpArrow)) {
            Jump ();
        }
    }

    private void MoveLeft ()
    {
        Move (Vector2.left);
        if (animator.GetBool ("last-direction-was-right")) {
            Flip ();
        }
        animator.SetBool ("last-direction-was-right", false);
    }

    private void MoveRight ()
    {
        Move (Vector2.right);
        if (!animator.GetBool ("last-direction-was-right")) {
            Flip ();
        }
        animator.SetBool ("last-direction-was-right", true);
    }

    private void Move (Vector2 translationVector)
    {
        transform.Translate (moveSpeed * translationVector);

        var oldState = animator.GetInteger ("state");
        var forwards = oldState == FlashlightGun.STATE_IDLE_FORWARD || oldState == FlashlightGun.STATE_WALKING_FORWARD;
        animator.SetInteger ("state", forwards ? FlashlightGun.STATE_WALKING_FORWARD : FlashlightGun.STATE_WALKING_BACKWARD);
    }

    private void Flip ()
    {
        // Flip whole thing (not just the sprite)
        var scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        // Make him immediately look in the right direction (otherwise the flipping fucks things up for a split sec.)
        var oldState = animator.GetInteger ("state");
        var forwards = oldState == FlashlightGun.STATE_IDLE_FORWARD || oldState == FlashlightGun.STATE_WALKING_FORWARD;
        var idle = oldState == FlashlightGun.STATE_IDLE_FORWARD || oldState == FlashlightGun.STATE_IDLE_BACKWARD;
        var otherState = forwards
            ? (idle ? FlashlightGun.STATE_IDLE_BACKWARD : FlashlightGun.STATE_WALKING_BACKWARD)
            : (idle ? FlashlightGun.STATE_IDLE_FORWARD : FlashlightGun.STATE_WALKING_FORWARD);
        animator.SetInteger ("state", otherState);
        var stateName = (idle ? "vampidle" : "look") + (forwards ? "back" : "forward");
        animator.Play (stateName);
    }

    private void Jump ()
    {
        if (isGrounded ()) {
            body.velocity = jumpSpeed * Vector2.up;
        }
    }

    private bool isGrounded ()
    {
        var feetRect = new Rect (feet.transform.position.x, feet.transform.position.y, feetWidth, 0.1f);
        return Physics2D.OverlapArea (feetRect.min, feetRect.max, whatIsGround);
    }

    public void Die ()
    {
        Debug.Log ("Game over!");
        animator.SetInteger ("state", FlashlightGun.STATE_DED);
        StartCoroutine (RestartAfterDelay ());     
    }

    IEnumerator RestartAfterDelay ()
    {
        yield return new WaitForSeconds (restartDelay);
        SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
    }
}
