using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private const float MOVE_SPEED = 0.1f;
    private const float JUMP_SPEED = 1400f;
    private const float FEET_RADIUS = 0.01f;

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

    void HandleInput ()
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
            body.AddForce (JUMP_SPEED * Vector2.up);
        }
    }

    private bool isGrounded ()
    {
        return Physics2D.OverlapCircle (feet.transform.position, FEET_RADIUS, whatIsGround);
    }
}
