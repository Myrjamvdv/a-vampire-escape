using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightGun : MonoBehaviour
{
    private const float INTENSITY_MULTIPLIER = 10f;

    public GameObject flashlight;

    void FixedUpdate ()
    {
        HandleInput ();
    }

    void HandleInput ()
    {
        // Gather data
        var basePosition = transform.position;
        var mousePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
        var mousePostionRelativeToBase = mousePosition - basePosition;
        var mouseBaseDistance = Vector2.Distance (mousePosition, transform.position);
        var mouseBaseAngle = Mathf.Atan2 (mousePostionRelativeToBase.y, mousePostionRelativeToBase.x);

        // Set flashlight angle
        flashlight.transform.rotation = Quaternion.Euler (-mouseBaseAngle * 180 / Mathf.PI, 90, 0);

        // Set flashlight intensity
        flashlight.GetComponent<Light> ().intensity = INTENSITY_MULTIPLIER * mouseBaseDistance;
    }
}
