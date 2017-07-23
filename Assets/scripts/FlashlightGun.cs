using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightGun : MonoBehaviour
{
    public const int STATE_IDLE_FORWARD = 3;
    public const int STATE_WALKING_FORWARD = 0;
    public const int STATE_IDLE_BACKWARD = 4;
    public const int STATE_WALKING_BACKWARD = 1;
    public const int STATE_DED = 2;

    public float intensityMultiplier;
    public float bulletSpeed;
    // In bullets / s
    public float fireRate;

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
        if (animator.GetInteger ("state") != STATE_DED) {
            HandleInput ();
        }
    }

    public void Die ()
    {
        armFront.SetActive (false);
        armBack.SetActive (false);
    }

    private void HandleInput ()
    {
        // Gather dinstance and angle of mouse to center of screen
        var basePosition = (Vector2)Camera.main.ViewportToScreenPoint (Camera.main.rect.center);
        var mousePosition = (Vector2)Input.mousePosition;
        var mousePostionRelativeToBase = mousePosition - basePosition;
        var mouseBaseDistance = Vector2.Distance (mousePosition, basePosition);
        var mouseBaseAngle = Mathf.Atan2 (mousePostionRelativeToBase.y, mousePostionRelativeToBase.x);
        var normalizedMouseDirection = mousePostionRelativeToBase / mouseBaseDistance;
        var looksForwards = CalculateFacingState (mouseBaseAngle);
        var arm = PickArm (looksForwards);
        var flashlight = arm.GetComponentInChildren<Light> ();
        flashlight.intensity = intensityMultiplier * mouseBaseDistance;

        // Shoot if mouse button is pressed
        if (Input.GetMouseButton (0)) {
            if (shootTimer <= 0) {
                Shoot (arm.transform.position, normalizedMouseDirection, mouseBaseAngle);
                shootTimer += 1 / fireRate;
            }
            shootTimer -= Time.fixedDeltaTime;
        }

        // Change vampire state according to what way he's looking
        SetVampireLook (looksForwards);

        // Change vampire arm according to angle
        SetVampireArm (looksForwards, mouseBaseAngle);
    }

    private void Shoot (Vector3 basePosition, Vector2 direction, float angle)
    {
        var position = new Vector2 (basePosition.x, basePosition.y);
        var rotation = Quaternion.Euler (0, 0, ToDegrees (angle));
        var bullet = (GameObject)Instantiate (bulletPrefab, position, rotation);
        // Let's not collide with ourselves
        Physics2D.IgnoreCollision (bullet.GetComponent<Collider2D> (), GetComponent<Collider2D> ());
        bullet.GetComponent<Rigidbody2D> ().velocity = bulletSpeed * direction;
        Destroy (bullet, 0.5f);
    }

    private bool CalculateFacingState (float flashlightAngle)
    {
        var currentDirectionIsRight = Mathf.Abs (flashlightAngle) <= Mathf.PI / 2;
        var lastDirectionWasRight = animator.GetBool ("last-direction-was-right");
        return lastDirectionWasRight == currentDirectionIsRight;
    }

    private void SetVampireLook (bool looksForwards)
    {
        var prevState = animator.GetInteger ("state");
        if (prevState == STATE_DED) {
            return;
        }
        var isIdle = prevState == STATE_IDLE_FORWARD || prevState == STATE_IDLE_BACKWARD;
        var newState = looksForwards
            ? (isIdle ? STATE_IDLE_FORWARD : STATE_WALKING_FORWARD)
            : (isIdle ? STATE_IDLE_BACKWARD : STATE_WALKING_BACKWARD);
        animator.SetInteger ("state", newState);
    }

    private GameObject PickArm (bool looksForwards)
    {
        return looksForwards ? armFront : armBack;
    }

    private void SetVampireArm (bool looksForwards, float angle)
    {
        var zAngle = Mathf.Abs (angle) < Mathf.PI / 2 ? angle : angle + Mathf.PI;
        if (looksForwards) {
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
