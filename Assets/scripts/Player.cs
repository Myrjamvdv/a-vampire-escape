using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    private const float MOVE_SPEED = 0.1f;
    private const float JUMP_SPEED = 17f;
    private const float FEET_WIDTH = 0.40f;

    public GameObject feet;
    public LayerMask whatIsGround;

    private Rigidbody2D body;

    void Start ()
    {
        body = GetComponent<Rigidbody2D> ();
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
    }

    private void MoveRight ()
    {
        Move (Vector2.right);
    }

    private void Move (Vector2 translationVector)
    {
        transform.Translate (MOVE_SPEED * translationVector);
    }

    private void Jump ()
    {
        if (isGrounded ()) {
            body.velocity = JUMP_SPEED * Vector2.up;
        } else {
            Debug.Log ("Can't jump if you're feet aren't touching the ground!");
        }
    }

    private bool isGrounded ()
    {
//        var colliderBounds = GetComponent<BoxCollider2D> ().bounds;
//        var feet = Rect.MinMaxRect (colliderBounds.min.x + 0.1f, colliderBounds.min.y, colliderBounds.max.x - 0.1f, colliderBounds.min.y + 0.1f);
        var feetRect = new Rect (feet.transform.position.x, feet.transform.position.y, FEET_WIDTH, 0.1f);
//        Debug.DrawLine (colliderBounds.min, colliderBounds.max);
        Debug.DrawLine (feetRect.min, feetRect.max);
        return Physics2D.OverlapArea (feetRect.min, feetRect.max, whatIsGround);
    }

    private void Die ()
    {
        Debug.Log ("Game over!");
        SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
    }
}
