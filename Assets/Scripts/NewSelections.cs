
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class NewSelections : MonoBehaviour
{
    private Camera cam;
    
    public List<GameObject> characters;
    public List<GameObject> selectedCharacters;
    public string clickState;

    private static NewSelections _instance;
    public static NewSelections Instance {  get { return _instance; } }

    private bool shiftPressed;
    private bool leftClickHeld;

    /// <summary>
    /// Drag Select
    /// </summary>
    private Vector3Int dragSelectStartingPoint;
    [SerializeField] private Vector3[] verticePoints;
    private int[] triangles;
    private Mesh dragBoxMesh;

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
        
        //Drag Box Building
        dragBoxMesh = new Mesh();
        dragBoxMesh = GetComponent<MeshFilter>().mesh;
        verticePoints = new Vector3[4];
        triangles = new int[6];
    }

    private void Update()
    {
        if (leftClickHeld)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100))
            {
                if (hit.transform.tag.Equals("Ground") && clickState == "Select" && !IsMouseOverUI())
                {
                    verticePoints[1] = new Vector3(Mathf.CeilToInt(hit.point.x),dragSelectStartingPoint.y +0.1f,dragSelectStartingPoint.z);
                    verticePoints[2] = new Vector3(dragSelectStartingPoint.x,dragSelectStartingPoint.y+0.1f,Mathf.CeilToInt(hit.point.z));
                    verticePoints[3] = new Vector3(Mathf.CeilToInt(hit.point.x),dragSelectStartingPoint.y+0.1f,Mathf.CeilToInt(hit.point.z));

                    if (((verticePoints[1].x < verticePoints[0].x) && (verticePoints[2].z < verticePoints[0].z))||((verticePoints[1].x > verticePoints[0].x) && (verticePoints[2].z > verticePoints[0].z)))
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
                    
                    dragBoxMesh.vertices = verticePoints;
                    dragBoxMesh.triangles = triangles;
                }
            }
        }
    }

    public void ClickToSelect(InputAction.CallbackContext context)//onclick
    {
        if(context.phase == InputActionPhase.Started)
        {
            leftClickHeld = true;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100)) 
            {
                hit.transform.TryGetComponent(out HarvestableObjects harvestableObjects);
                if (hit.transform.tag.Equals("Villagers") && clickState == "Select" && !IsMouseOverUI() && !shiftPressed)
                {
                    foreach (GameObject select in selectedCharacters)
                    {
                        if (select.GetComponent<Worker>().workerStates != Worker.WorkerStates.Working)
                        {
                            select.transform.gameObject.GetComponent<Outline>().enabled = false;
                        }
                    }
                    selectedCharacters.Clear();
                    selectedCharacters.Add(hit.transform.gameObject);
                    hit.transform.gameObject.GetComponent<Outline>().enabled = true;
                    hit.transform.gameObject.GetComponent<Outline>().OutlineColor = Color.cyan;
                    hit.transform.gameObject.GetComponent<Outline>().OutlineWidth = 10;
                }
                
                else if (hit.transform.tag.Equals("Villagers") && clickState == "Select" && !IsMouseOverUI() && shiftPressed && !selectedCharacters.Contains(hit.transform.gameObject))
                {
                    selectedCharacters.Add(hit.transform.gameObject);
                    hit.transform.gameObject.GetComponent<Outline>().enabled = true;
                    hit.transform.gameObject.GetComponent<Outline>().OutlineColor = Color.cyan;
                    hit.transform.gameObject.GetComponent<Outline>().OutlineWidth = 10;
                }

                else if (hit.transform.tag.Equals("Harvestable") && clickState == "Harvest" && !IsMouseOverUI())
                {
                    for (int i = 0; i < selectedCharacters.Count; i++)
                    {
                        selectedCharacters[i].TryGetComponent<Worker>(out var worker);
                        if (worker.workerStates == Worker.WorkerStates.Available && harvestableObjects.canInteract.Contains(worker.role))
                        {
                            worker.workerStates = Worker.WorkerStates.Working;
                            worker.WorkerStateManagement(worker.workerStates,hit.transform.gameObject);
                            break;
                        }
                    }
                }

                else if (hit.transform.tag.Equals("Ground") && clickState == "Move" && !IsMouseOverUI())
                {
                    
                    foreach (GameObject select in selectedCharacters)
                    {
                        if (select.GetComponent<Worker>().workerStates != Worker.WorkerStates.Working)
                        {
                            select.GetComponent<NavMeshAgent>().SetDestination(hit.point);    
                        }
                    }  
                }
                
                else if (hit.transform.tag.Equals("Ground") && clickState == "Select" && !IsMouseOverUI())
                {
                    dragSelectStartingPoint = Vector3Int.CeilToInt(hit.point);
                    verticePoints[0] = new Vector3(dragSelectStartingPoint.x, dragSelectStartingPoint.y+0.1f,dragSelectStartingPoint.z);
                }
            }
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            leftClickHeld = false;
            dragBoxMesh.Clear();
        }
    }
    public void Select()
    {
        clickState = "Select";
    }
    
    public void Harvest()
    {
        clickState = "Harvest";
    }
    
    public void Deselect()
    {
        clickState = "Deselect";
        foreach (GameObject select in selectedCharacters)
        {
            select.transform.gameObject.GetComponent<Outline>().enabled = false;
        }
        selectedCharacters.Clear();
    }
    
    public void Move()
    {
        clickState = "Move";
    }
    
    private bool IsMouseOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }
    
    public void MultiSelect(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            shiftPressed = true;
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            shiftPressed = false;
        }
    }

    
}
