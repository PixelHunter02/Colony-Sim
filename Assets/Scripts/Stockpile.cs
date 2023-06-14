using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Stockpile : MonoBehaviour
{
    [FormerlySerializedAs("mesh")] public Mesh stockPile;

    public Vector3[] vertices = new Vector3[4];
    private int[] triangles = new int[6];
    public Material stockPileMaterial; 
    public Vector2 dimensions;
    public int maxStorage;
    public int currentStorageTaken;

    private void Awake()
    {
        stockPile = new Mesh();
        DrawBox();
        
    }

    public void DrawBox()
    {
        if (((vertices[1].x < vertices[0].x) && (vertices[2].z < vertices[0].z))||((vertices[1].x > vertices[0].x) && (vertices[2].z > vertices[0].z)))
        {
            triangles[0] = 2;
            triangles[1] = 1;
            triangles[2] = 0;
            triangles[3] = 3;
            triangles[4] = 1;
            triangles[5] = 2; 
        }
        else
        {
            triangles[0] = 0;
            triangles[1] = 1;
            triangles[2] = 2;
            triangles[3] = 2;
            triangles[4] = 1;
            triangles[5] = 3; 
        }
        
        stockPile.vertices = vertices;
        stockPile.triangles = triangles;
        gameObject.GetComponent<MeshFilter>().mesh = stockPile;
        GetComponent<MeshRenderer>().material = stockPileMaterial;
        dimensions = new Vector2(Mathf.Max(vertices[1].x, vertices[0].x) - Mathf.Min(vertices[1].x, vertices[0].x),
            Mathf.Max(vertices[2].z, vertices[0].z) - Mathf.Min(vertices[2].z, vertices[0].z));
        maxStorage = Mathf.CeilToInt(dimensions.x * dimensions.y);

        GetComponent<MeshFilter>().mesh.RecalculateBounds();
        GetComponent<MeshFilter>().mesh.RecalculateNormals();
    }
}
