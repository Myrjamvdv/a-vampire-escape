using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerCollidesWith : MonoBehaviour
{
    public string layerName;
    public LayerMask layersToCollideWith;

    void Start ()
    {
        var layer = LayerMask.NameToLayer (layerName);
        Physics2D.SetLayerCollisionMask (layer, layersToCollideWith);
    }
}
