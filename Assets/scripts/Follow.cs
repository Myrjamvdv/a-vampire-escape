using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public GameObject toFollow;
    public GameObject boundingBoxObject;

    void Update ()
    {
        var boundingBox = boundingBoxObject.GetComponent<Renderer> ().bounds;
        var halfScreenDiagonal = Camera.main.ViewportToWorldPoint (new Vector2 (0.5f, 0.5f))
                                 - Camera.main.ViewportToWorldPoint (new Vector2 (0, 0));
        boundingBox.SetMinMax (boundingBox.min + halfScreenDiagonal, boundingBox.max - halfScreenDiagonal);

        transform.position = toFollow.transform.position;
        transform.position = Vector2.Max (transform.position, boundingBox.min);
        transform.position = Vector2.Min (transform.position, boundingBox.max);
        transform.Translate (10 * Vector3.back);
    }
}
