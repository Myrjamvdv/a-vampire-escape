using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightGun : MonoBehaviour
{
    private const float INTENSITY_MULTIPLIER = .25f;
    private const float BULLET_SPEED = 20;
    // In bullets / s
    private const float FIRE_RATE = 5;

    public GameObject flashlight;
    public GameObject bulletPrefab;

    private float shootTimer;

    void Start ()
    {
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
        flashlight.transform.rotation = Quaternion.Euler (-toDegrees (mouseBaseAngle), 90, 0);

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
    }

    private void Shoot (Vector2 direction, float angle)
    {
        var position = new Vector2 (flashlight.transform.position.x, flashlight.transform.position.y);
        var rotation = Quaternion.Euler (0, 0, toDegrees (angle));
        var bullet = (GameObject)Instantiate (bulletPrefab, position, rotation);
        // Let's not collide with ourselves
        Physics2D.IgnoreCollision (bullet.GetComponent<Collider2D> (), GetComponent<Collider2D> ());
        bullet.GetComponent<Rigidbody2D> ().velocity = BULLET_SPEED * direction;
    }

    private float toDegrees (float radians)
    {
        return radians * 180 / Mathf.PI;
    }
}
