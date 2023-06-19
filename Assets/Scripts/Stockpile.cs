using System.Linq;
using UnityEngine;

public class Stockpile : MonoBehaviour
{
    public Mesh stockPile;

    public Vector3[] vertices = new Vector3[4];
    private int[] _triangles = new int[6];
    public Material stockPileMaterial; 
    
    public delegate void OnCreateStorageCell();
    public static event OnCreateStorageCell OnCreateStorageCellEvent;


    private void Awake() 
    {
        stockPile = new Mesh();
        DrawStockpile();
    }

    public void DrawStockpile()
    {
        if (((vertices[1].x < vertices[0].x) && (vertices[2].z < vertices[0].z))||((vertices[1].x > vertices[0].x) && (vertices[2].z > vertices[0].z)))
        {
            _triangles[0] = 2;
            _triangles[1] = 1;
            _triangles[2] = 0;
            _triangles[3] = 3;
            _triangles[4] = 1;
            _triangles[5] = 2; 
        }
        else
        {
            _triangles[0] = 0;
            _triangles[1] = 1;
            _triangles[2] = 2;
            _triangles[3] = 2;
            _triangles[4] = 1;
            _triangles[5] = 3; 
        }
        
        stockPile.vertices = vertices;
        stockPile.triangles = _triangles;

        gameObject.GetComponent<MeshFilter>().mesh = stockPile;
        GetComponent<Rigidbody>().isKinematic = true;

        gameObject.layer = 8;
        GetComponent<MeshRenderer>().material = stockPileMaterial;
        var meshFilter = GetComponent<MeshFilter>().mesh;
        meshFilter.RecalculateBounds();
        meshFilter.RecalculateNormals();
        
        CreateNewStorageCells();
    }
    
    private void CreateNewStorageCells()
    {
        for (var x = (int)Mathf.Min(vertices[1].x, vertices[0].x); x < (int)Mathf.Max(vertices[1].x, vertices[0].x); x++)
        {
            for (var z = (int)Mathf.Min(vertices[2].z, vertices[0].z); z < (int)Mathf.Max(vertices[2].z, vertices[0].z); z++)
            {
                var storagePosition = new Vector3(x + 0.5f, vertices[0].y, z + 0.5f);
                if(StorageManager.usedSpaces.Contains(storagePosition))
                    continue;
                StorageManager.storageLocations.Add(storagePosition);
            }
        }

        OnCreateStorageCellEvent?.Invoke();
    }
}
