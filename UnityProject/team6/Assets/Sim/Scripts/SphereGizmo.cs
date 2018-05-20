using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereGizmo : MonoBehaviour {

    public float size;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(transform.position, 0.1f + size);
    }
}
