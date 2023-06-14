
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class NewSelections : MonoBehaviour
{
    private Camera cam;
    
    // public List<GameObject> selectedCharacters;
    public string clickState;

    private static NewSelections _instance;
    public static NewSelections Instance {  get { return _instance; } }

    /// <summary>
    /// Button Hold
    /// </summary>
    private bool shiftPressed;
    private bool leftClickHeld;

    /// <summary>
    /// Drag Select
    /// </summary>
    private Vector3Int dragSelectStartingPoint;
    [SerializeField] private Vector3[] verticePoints;
    private int[] triangles;
    private Mesh dragBoxMesh;
    [SerializeField] private Material selectMaterial;
    [SerializeField] private Material stockPileMaterial;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        cam = GameObject.Find("Main Camera").GetComponent<Camera>();                    
        
        
        //Drag Box Setup
        dragBoxMesh = new Mesh();
        dragBoxMesh = GetComponent<MeshFilter>().mesh;
        verticePoints = new Vector3[4];
        triangles = new int[6];
        GetComponent<MeshCollider>().sharedMesh = dragBoxMesh; 

    }
    //
    // private void Update()
    // {
    //     if (leftClickHeld)
    //     {
    //         Ray ray = cam.ScreenPointToRay(Input.mousePosition);
    //         if (Physics.Raycast(ray, out RaycastHit hit, 10000))
    //         {
    //             if (hit.transform.tag.Equals("Ground") && clickState == "Select" && !IsMouseOverUI())
    //             {
    //                 GetComponent<MeshRenderer>().material = selectMaterial;
    //                 DrawBox(hit);
    //                 GetComponent<MeshCollider>().sharedMesh = null;
    //                 GetComponent<MeshCollider>().sharedMesh = dragBoxMesh; 
    //                 
    //             }
    //             else if (hit.transform.tag.Equals("Ground") && clickState == "Stockpile" && !IsMouseOverUI())
    //             {
    //                 GetComponent<MeshRenderer>().material = stockPileMaterial;
    //                 DrawBox(hit);
    //             }
    //         }
    //     }
    // }

    // private void OnTriggerEnter(Collider other)
    // {
    //     if (other.tag.Equals("Villagers") && !selectedCharacters.Contains(other.gameObject))
    //     {
    //         selectedCharacters.Add(other.transform.gameObject);
    //         other.transform.gameObject.GetComponent<Outline>().enabled = true;
    //         other.transform.gameObject.GetComponent<Outline>().OutlineColor = Color.cyan;
    //         other.transform.gameObject.GetComponent<Outline>().OutlineWidth = 10;
    //     }
    // }

    // public void ClickToSelect(InputAction.CallbackContext context)//onclick
    // {
    //     if(context.phase == InputActionPhase.Started)
    //     {
    //         leftClickHeld = true;
    //         Ray ray = cam.ScreenPointToRay(Input.mousePosition);
    //         if (Physics.Raycast(ray, out RaycastHit hit, 100)) 
    //         {
    //             hit.transform.TryGetComponent(out ObjectManager objectManager);
    //             if (hit.transform.tag.Equals("Villagers") && clickState == "Select" && !IsMouseOverUI() && !shiftPressed)
    //             {
    //                 foreach (GameObject select in selectedCharacters)
    //                 {
    //                     if (select.GetComponent<Worker>().workerStates != Worker.WorkerStates.Working)
    //                     {
    //                         select.transform.gameObject.GetComponent<Outline>().enabled = false;
    //                     }
    //                 }
    //                 selectedCharacters.Clear();
    //                 selectedCharacters.Add(hit.transform.gameObject);
    //                 hit.transform.gameObject.GetComponent<Outline>().enabled = true;
    //                 hit.transform.gameObject.GetComponent<Outline>().OutlineColor = Color.cyan;
    //                 hit.transform.gameObject.GetComponent<Outline>().OutlineWidth = 10;
    //             }
    //             
    //             else if (hit.transform.tag.Equals("Villagers") && clickState == "Select" && !IsMouseOverUI() && shiftPressed && !selectedCharacters.Contains(hit.transform.gameObject))
    //             {
    //                 selectedCharacters.Add(hit.transform.gameObject);
    //                 hit.transform.gameObject.GetComponent<Outline>().enabled = true;
    //                 hit.transform.gameObject.GetComponent<Outline>().OutlineColor = Color.cyan;
    //                 hit.transform.gameObject.GetComponent<Outline>().OutlineWidth = 10;
    //             }
    //
    //             else if (hit.transform.tag.Equals("Harvestable") && clickState == "Harvest" && !IsMouseOverUI())
    //             {
    //                 for (int i = 0; i < selectedCharacters.Count; i++)
    //                 {
    //                     selectedCharacters[i].TryGetComponent<Worker>(out var worker);
    //                     if (worker.workerStates == Worker.WorkerStates.Available &&
    //                         objectManager._harvestableObject.harvestType == HarvestableObjectSO.HarvestType.Pickup)
    //                     {
    //                         if (GameObject.Find("Mesh").GetComponent<Stockpile>().maxStorage >
    //                             GameObject.Find("Mesh").GetComponent<Stockpile>().currentStorageTaken)
    //                         {
    //                             hit.transform.position =
    //                                 GameObject.Find("Mesh").GetComponent<Stockpile>().vertices[0];
    //                         }
    //                     }
    //                     else if (worker.workerStates == Worker.WorkerStates.Available && objectManager._harvestableObject.canInteract.Contains(worker.role) && objectManager._harvestableObject.harvestType != HarvestableObjectSO.HarvestType.Pickup)
    //                     {
    //                         worker.workerStates = Worker.WorkerStates.Working;
    //                         worker.WorkerStateManagement(worker.workerStates,hit.transform.gameObject);
    //                         break;
    //                     }
    //                     
    //                 }
    //             }
    //
    //             else if (hit.transform.tag.Equals("Ground") && clickState == "Move" && !IsMouseOverUI())
    //             {
    //                 
    //                 foreach (GameObject select in selectedCharacters)
    //                 {
    //                     if (select.GetComponent<Worker>().workerStates != Worker.WorkerStates.Working)
    //                     {
    //                         select.GetComponent<NavMeshAgent>().SetDestination(hit.point);    
    //                     }
    //                 }  
    //             }
    //             
    //             else if (hit.transform.tag.Equals("Ground") && clickState == "Select" && !IsMouseOverUI())
    //             {
    //                 dragSelectStartingPoint = Vector3Int.CeilToInt(hit.point);
    //             }
    //             
    //             else if (hit.transform.tag.Equals("Ground") && clickState == "Stockpile" && !IsMouseOverUI())
    //             {
    //                 dragSelectStartingPoint = Vector3Int.CeilToInt(hit.point);
    //             }
    //         }
    //     }
    //     else if (context.phase == InputActionPhase.Canceled && clickState == "Stockpile")
    //     {
    //         leftClickHeld = false;   
    //         GenerateStockpile();
    //         dragBoxMesh.Clear();
    //     }
    //     else if (context.phase == InputActionPhase.Canceled )
    //     {
    //         leftClickHeld = false;
    //         dragBoxMesh.Clear();
    //     }
    // }
    // public void Select()
    // {
    //     clickState = "Select";
    // }
    //
    // public void Harvest()
    // {
    //     clickState = "Harvest";
    // }
    //
    // public void Deselect()
    // {
    //     clickState = "Deselect";
    //     foreach (GameObject select in selectedCharacters)
    //     {
    //         select.transform.gameObject.GetComponent<Outline>().enabled = false;
    //     }
    //     selectedCharacters.Clear();
    //     
    // }
    //
    // public void Move()
    // {
    //     clickState = "Move";
    // }
    //
    // public void MakeStockpile()
    // {
    //     clickState = "Stockpile";
    // }
    //
    // private bool IsMouseOverUI()
    // {
    //     return EventSystem.current.IsPointerOverGameObject();
    // }
    //
    // public void MultiSelect(InputAction.CallbackContext context)
    // {
    //     if (context.phase == InputActionPhase.Performed)
    //     {
    //         shiftPressed = true;
    //     }
    //     else if (context.phase == InputActionPhase.Canceled)
    //     {
    //         shiftPressed = false;
    //     }
    // }
    //
    // private void DrawBox(RaycastHit hit)
    // {
    //     verticePoints[0] = new Vector3(dragSelectStartingPoint.x, dragSelectStartingPoint.y + 0.03f,dragSelectStartingPoint.z);
    //     verticePoints[1] = new Vector3(Mathf.CeilToInt(hit.point.x),dragSelectStartingPoint.y + 0.03f,dragSelectStartingPoint.z);
    //     verticePoints[2] = new Vector3(dragSelectStartingPoint.x,dragSelectStartingPoint.y + 0.03f,Mathf.CeilToInt(hit.point.z));
    //     verticePoints[3] = new Vector3(Mathf.CeilToInt(hit.point.x),dragSelectStartingPoint.y + 0.03f,Mathf.CeilToInt(hit.point.z));
    //
    //     if (((verticePoints[1].x < verticePoints[0].x) && (verticePoints[2].z < verticePoints[0].z))||((verticePoints[1].x > verticePoints[0].x) && (verticePoints[2].z > verticePoints[0].z)))
    //     {
    //         triangles[0] = 2;
    //         triangles[1] = 1;
    //         triangles[2] = 0;
    //         triangles[3] = 3;
    //         triangles[4] = 1;
    //         triangles[5] = 2; 
    //     }
    //     else
    //     {
    //         triangles[0] = 0;
    //         triangles[1] = 1;
    //         triangles[2] = 2;
    //         triangles[3] = 2;
    //         triangles[4] = 1;
    //         triangles[5] = 3; 
    //     }
    //     dragBoxMesh.vertices = verticePoints;
    //     dragBoxMesh.triangles = triangles;
    //     
    // }
    //
    // /// <summary>
    // /// Save Information To A Generated Prefab
    // /// </summary>
    // public void GenerateStockpile()
    // {
    //     GameObject meshGO = new GameObject("Mesh", typeof(MeshFilter), typeof(MeshRenderer),typeof(Stockpile));
    //     var dataToAdd = meshGO.GetComponent<Stockpile>() ;
    //     dataToAdd.vertices[0] = verticePoints[0];
    //     dataToAdd.vertices[1] = verticePoints[1];
    //     dataToAdd.vertices[2] = verticePoints[2];
    //     dataToAdd.vertices[3] = verticePoints[3];
    //     dataToAdd.stockPileMaterial = stockPileMaterial;
    //     dataToAdd.DrawBox();
    // }
}
