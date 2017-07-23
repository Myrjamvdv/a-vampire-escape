using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
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
            Die ();
        }
    }

    private void HandleInput ()
    {
        if (Input.GetKey (KeyCode.Escape) || Input.GetKey (KeyCode.Q)) {
            Debug.Log ("Quitting.");
            Application.Quit ();
        }

        if (Input.GetKey (KeyCode.RightArrow)) {
            MoveRight ();
        } else if (Input.GetKey (KeyCode.LeftArrow)) {
            MoveLeft ();
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
    }

    private void Flip ()
    {
        // Flip whole thing (not just the sprite)
        var scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        // Make him immediately look in the right direction (otherwise the flipping fucks things up for a split sec.)
        var otherState = animator.GetInteger ("state") == FlashlightGun.STATE_FACING_FORWARD
            ? FlashlightGun.STATE_FACING_BACKWARD
            : FlashlightGun.STATE_FACING_FORWARD;
        animator.SetInteger ("state", otherState);
        animator.Play (otherState == FlashlightGun.STATE_FACING_FORWARD ? "lookforward" : "lookback");
    }

    private void Jump ()
    {
        if (isGrounded ()) {
            body.velocity = jumpSpeed * Vector2.up;
        } else {
            Debug.Log ("Can't jump if you're feet aren't touching the ground!");
        }
    }

    private bool isGrounded ()
    {
        var feetRect = new Rect (feet.transform.position.x, feet.transform.position.y, feetWidth, 0.1f);
        return Physics2D.OverlapArea (feetRect.min, feetRect.max, whatIsGround);
    }

    private void Die ()
    {
        Debug.Log ("Game over!");
        SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
    }
}
