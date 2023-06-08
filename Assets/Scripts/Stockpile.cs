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
    }
}
