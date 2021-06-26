using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComBineChildren : MonoBehaviour
{
    void Start()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length - 1];

        for (int i = 0; i < meshFilters.Length - 1; i++)
        {
            combine[i].mesh = meshFilters[i + 1].sharedMesh;
            combine[i].transform = meshFilters[i + 1].transform.localToWorldMatrix;
            meshFilters[i + 1].gameObject.SetActive(false);
        }
        transform.GetComponent<MeshFilter>().mesh = new Mesh();
        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
        transform.gameObject.SetActive(true);
    }
}
