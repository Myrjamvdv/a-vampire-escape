using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightGun : MonoBehaviour
{
    private const float INTENSITY_MULTIPLIER = .25f;
    private const float BULLET_SPEED = 20;
    // In bullets / s
    private const float FIRE_RATE = 5;

    public const int STATE_FACING_FORWARD = 0;
    public const int STATE_FACING_BACKWARD = 1;

    public GameObject flashlight;
    public GameObject bulletPrefab;

    private Animator animator;
    private float shootTimer;

    void Start ()
    {
        animator = GetComponent<Animator> ();
        shootTimer = 0;
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

        // Set flashlight angle
        flashlight.transform.rotation = Quaternion.Euler (-ToDegrees (mouseBaseAngle), 90, 0);

        // Set flashlight intensity
        flashlight.GetComponent<Light> ().intensity = INTENSITY_MULTIPLIER * mouseBaseDistance;

        // Shoot if mouse button is pressed
        if (Input.GetMouseButton (0)) {
            if (shootTimer <= 0) {
                Shoot (normalizedMouseDirection, mouseBaseAngle);
                shootTimer += 1 / FIRE_RATE;
            }
            shootTimer -= Time.fixedDeltaTime;
        }

        // Change vampire state according to what way he's looking
        SetVampireLook (mouseBaseAngle);
    }

    private void Shoot (Vector2 direction, float angle)
    {
        var position = new Vector2 (flashlight.transform.position.x, flashlight.transform.position.y);
        var rotation = Quaternion.Euler (0, 0, ToDegrees (angle));
        var bullet = (GameObject)Instantiate (bulletPrefab, position, rotation);
        // Let's not collide with ourselves
        Physics2D.IgnoreCollision (bullet.GetComponent<Collider2D> (), GetComponent<Collider2D> ());
        bullet.GetComponent<Rigidbody2D> ().velocity = BULLET_SPEED * direction;
        Destroy (bullet, 0.5f);
    }

    private void SetVampireLook (float flashlightAngle)
    {
        var currentDirectionIsRight = Mathf.Abs (flashlightAngle) <= Mathf.PI / 2;
        var lastDirectionWasRight = animator.GetBool ("last-direction-was-right");
        var newState = (lastDirectionWasRight == currentDirectionIsRight) ? STATE_FACING_FORWARD : STATE_FACING_BACKWARD;
        animator.SetInteger ("state", newState);
    }

    private float ToDegrees (float radians)
    {
        return radians * 180 / Mathf.PI;
    }
}
