using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightGun : MonoBehaviour
{
    public const int STATE_FACING_FORWARD = 0;
    public const int STATE_FACING_BACKWARD = 1;

    public float intensityMultiplier;
    public float bulletSpeed;
    // In bullets / s
    public float fireRate;

    public GameObject flashlight;
    public GameObject bulletPrefab;
    public GameObject armFront;
    public GameObject armBack;

    private Animator animator;
    private float shootTimer;
    private Quaternion armFrontStartRotation;
    private Quaternion armBackStartRotation;

    void Start ()
    {
        animator = GetComponent<Animator> ();
        shootTimer = 0;
        armFrontStartRotation = armFront.transform.rotation;
        armBackStartRotation = armBack.transform.rotation;
    }

    void Update ()
    {
        // Reset shoot timer when auto-fire stops
        if (Input.GetMouseButtonUp (0)) {
            shootTimer = 0;
        }
    }

    void FixedUpdate ()
    {
        HandleInput ();
    }

    void HandleInput ()
    {
        // Gather dinstance and angle of mouse to center of screen
        var basePosition = (Vector2)Camera.main.ViewportToScreenPoint (Camera.main.rect.center);
        var mousePosition = (Vector2)Input.mousePosition;
        var mousePostionRelativeToBase = mousePosition - basePosition;
        var mouseBaseDistance = Vector2.Distance (mousePosition, basePosition);
        var mouseBaseAngle = Mathf.Atan2 (mousePostionRelativeToBase.y, mousePostionRelativeToBase.x);
        var normalizedMouseDirection = mousePostionRelativeToBase / mouseBaseDistance;
        var facingState = CalculateFacingState (mouseBaseAngle);
        var arm = PickArm (facingState);
        var flashlight = arm.GetComponentInChildren<Light> ();
        flashlight.intensity = intensityMultiplier * mouseBaseDistance;

        // Shoot if mouse button is pressed
        if (Input.GetMouseButton (0)) {
            if (shootTimer <= 0) {
                Shoot (normalizedMouseDirection, mouseBaseAngle);
                shootTimer += 1 / fireRate;
            }
            shootTimer -= Time.fixedDeltaTime;
        }

        // Change vampire state according to what way he's looking
        SetVampireLook (facingState);

        // Change vampire arm according to angle
        SetVampireArm (facingState, mouseBaseAngle);
    }

    private void Shoot (Vector2 direction, float angle)
    {
        var position = new Vector2 (flashlight.transform.position.x, flashlight.transform.position.y);
        var rotation = Quaternion.Euler (0, 0, ToDegrees (angle));
        var bullet = (GameObject)Instantiate (bulletPrefab, position, rotation);
        // Let's not collide with ourselves
        Physics2D.IgnoreCollision (bullet.GetComponent<Collider2D> (), GetComponent<Collider2D> ());
        bullet.GetComponent<Rigidbody2D> ().velocity = bulletSpeed * direction;
        Destroy (bullet, 0.5f);
    }

    private int CalculateFacingState (float flashlightAngle)
    {
        var currentDirectionIsRight = Mathf.Abs (flashlightAngle) <= Mathf.PI / 2;
        var lastDirectionWasRight = animator.GetBool ("last-direction-was-right");
        return (lastDirectionWasRight == currentDirectionIsRight) ? STATE_FACING_FORWARD : STATE_FACING_BACKWARD;
    }

    private void SetVampireLook (int facingState)
    {
        animator.Play (facingState == STATE_FACING_FORWARD ? "lookforward" : "lookback");
        animator.SetInteger ("state", facingState);
    }

    private GameObject PickArm (int facingState)
    {
        return facingState == STATE_FACING_FORWARD ? armFront : armBack;
    }

    private void SetVampireArm (int facingState, float angle)
    {
        var zAngle = Mathf.Abs (angle) < Mathf.PI / 2 ? angle : angle + Mathf.PI;
        if (facingState == STATE_FACING_FORWARD) {
            armFront.SetActive (true);
            armBack.SetActive (false);
            armFront.transform.rotation = armFrontStartRotation * Quaternion.Euler (0, 0, ToDegrees (zAngle));
        } else {
            armFront.SetActive (false);
            armBack.SetActive (true);
            armBack.transform.rotation = armBackStartRotation * Quaternion.Euler (0, 0, ToDegrees (zAngle));
        }
    }

    private float ToDegrees (float radians)
    {
        return radians * 180 / Mathf.PI;
    }
}
