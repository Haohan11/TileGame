using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    Bounds bounds;

    private void Start()
    {
        bounds.size = gameObject.GetComponent<MeshFilter>().mesh.bounds.size;
        bounds.size = new Vector3(0.9f,0.9f,0.3f);
        gameObject.GetComponent<MeshFilter>().mesh.bounds = bounds;

    }
}
