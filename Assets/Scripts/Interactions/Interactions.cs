using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class Interactions : MonoBehaviour
{
    // private PlayerInputActions _playerInputActions;

    #region Stockpile Variables

    [SerializeField] private Vector3[] vertices;
    private int[] _triangles;
    private Vector2[] _uvs;
    private bool _drawingStockpile;
    private Mesh _stockpileMesh;
    [SerializeField] Material stockpileMaterial;

    #endregion

    private Outline _lastHitOutline;

    private static Villager previouslySelected;

    private static bool isOverUI;

    private GameManager _gameManager;

    public LayerMask ground;
    
    private void Awake()
    {
        InitializeStockpiles();

        _gameManager = GameManager.Instance;
    }

    private void Start()
    {
        _gameManager.inputManager.playerInputActions.Player.Select.performed += ClickContext;
        _gameManager.inputManager.playerInputActions.Player.Select.canceled += ClickContext;
    }

    private void Update()
    {
        OutlineInteractable();
    }

    private void OutlineInteractable()
    {
        var ray = _gameManager.mainCamera.ScreenPointToRay(_gameManager.inputManager.playerInputActions.UI.Point.ReadValue<Vector2>());
        if (!Physics.Raycast(ray, out var hit))
            return;

        // Clear the reference to the previous outline if not highlighting.
        if (_lastHitOutline)
        {
            _lastHitOutline.enabled = false;
            _lastHitOutline = null;
        }

        // Detect if there is an outline component on the gameobject, If there is enable it
        if (!hit.transform.TryGetComponent(out Outline outline))
            return;
        _lastHitOutline = outline;
        outline.enabled = true;
    }

    private void ClickContext(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                Interactable();
                BeginDrawStockpile();
                break;
            case InputActionPhase.Canceled:
                PlaceStockpile();
                break;
        }
    }

    private void Interactable()
    {
        // Get the information of the object being clicked
        var ray = _gameManager.mainCamera.ScreenPointToRay(_gameManager.inputManager.playerInputActions.UI.Point.ReadValue<Vector2>());
        if (!Physics.Raycast(ray, out var hit, 100))
            return;

        if (hit.transform.TryGetComponent(out IInteractable interactable) && !isOverUI)
        {
            interactable.OnInteraction();
        }
    }

    public static void SetNewSelectedVillager(Villager villager)
    {
        //Check if Previously Selected = null
        if (!previouslySelected)
        {
            //Assign Previously Selected If previously Selected is null
            previouslySelected = villager;
            SetAllChildLayers(villager.gameObject, 6);
            villager.transform.GetChild(0).Find("PortraitCamera").gameObject.SetActive(true);
        }
        // If Previously Selected Is not null
        else
        {
            // set the layer of previously selected to be 0
            SetAllChildLayers(previouslySelected.gameObject, 0);
            previouslySelected.transform.GetChild(0).Find("PortraitCamera").gameObject.SetActive(false);
            previouslySelected = villager;
            SetAllChildLayers(previouslySelected.gameObject, 6);
            previouslySelected.transform.GetChild(0).Find("PortraitCamera").gameObject.SetActive(true);
        }
    }

    private static void SetAllChildLayers(GameObject root, int layer)
    {
        var children = root.GetComponentsInChildren<Transform>(includeInactive: true);
        foreach (var child in children)
        {
            child.gameObject.layer = layer;
        }
    }

    #region Stockpiles

    private void InitializeStockpiles()
    {
        _stockpileMesh = new Mesh();
        _stockpileMesh = GetComponent<MeshFilter>().mesh;
        vertices = new Vector3[4];
        AssignUVs();
        _triangles = new int[6];
    }

    private void BeginDrawStockpile()
    {
        // Get the information of the object being clicked
        var ray = _gameManager.mainCamera.ScreenPointToRay(_gameManager.inputManager.playerInputActions.UI.Point.ReadValue<Vector2>());
        if (!Physics.Raycast(ray, out var hit, 1000,ground) || isOverUI ||!_gameManager.level.stockpileMode) 
            return;
    
        // Check if the clicked object is tagged with Ground
        if (!hit.transform.tag.Equals("Ground"))
            return;

        _drawingStockpile = true;
        var point = new Vector3(Mathf.FloorToInt(hit.point.x), hit.point.y + 0.1f, Mathf.FloorToInt(hit.point.z));
        //var point = grid.WorldToCell(hit.transform.position);
        vertices[0] = point;
        StartCoroutine(DrawStockpileCR());
    }
    
    private IEnumerator DrawStockpileCR()
    {
        var ray = _gameManager.mainCamera.ScreenPointToRay(_gameManager.inputManager.playerInputActions.UI.Point.ReadValue<Vector2>());
        if (!Physics.Raycast(ray, out var hit, 1000)) 
            yield break;
        
        var startingPoint = vertices[0];
        var mousePosition = hit.point;
        
        if (!_drawingStockpile) 
            yield break;
        
        var xDistance = startingPoint.x - mousePosition.x;
        if (xDistance > 5)
        {
            mousePosition.x = startingPoint.x - 5;
        }
        else if (xDistance < -5)
        {
            mousePosition.x = startingPoint.x + 5;
        }
        
        var zDistance = startingPoint.z - mousePosition.z;
        if (zDistance > 5)
        {
            mousePosition.z = startingPoint.z - 5;
        }
        else if (zDistance < -5)
        {
            mousePosition.z = startingPoint.z + 5;
        }
        
        vertices[1] = new Vector3(Mathf.FloorToInt(mousePosition.x),startingPoint.y,startingPoint.z);
        vertices[2] = new Vector3(startingPoint.x,startingPoint.y,Mathf.FloorToInt(mousePosition.z));
        vertices[3] = new Vector3(Mathf.FloorToInt(mousePosition.x),startingPoint.y,Mathf.FloorToInt(mousePosition.z));

        
        
        DrawTriangles();
        
        GetComponent<MeshFilter>().mesh.RecalculateBounds();
        GetComponent<MeshFilter>().mesh.RecalculateNormals();
        
        //Assign Mesh Information
        _stockpileMesh.vertices = vertices;
        _stockpileMesh.uv = _uvs;
        _stockpileMesh.triangles = _triangles;
        
        yield return new WaitForEndOfFrame();
        
        //Update Drawing
        StartCoroutine(DrawStockpileCR());
    }

    private void AssignUVs()
    {
        _uvs = new Vector2[4];
        _uvs[0] = new Vector2(0, 1);
        _uvs[1] = new Vector2(1, 1);
        _uvs[2] = new Vector2(0, 0);
        _uvs[3] = new Vector2(1, 0);
    }
    
    private void DrawTriangles()
    {
        if ((vertices[1].x < vertices[0].x && vertices[2].z < vertices[0].z)||(vertices[1].x > vertices[0].x && vertices[2].z > vertices[0].z))
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
    }
    
    private void PlaceStockpile()
    {
        if (!_drawingStockpile) 
            return;
        if (Mathf.CeilToInt(vertices[0].x) == Mathf.CeilToInt(vertices[1].x) || Mathf.CeilToInt(vertices[0].z) == Mathf.CeilToInt(vertices[2].z))
        {
            _drawingStockpile = false;
            return;
        }
        
        _drawingStockpile = false;
        GenerateStockpile();
        _stockpileMesh.Clear();
    }
    
    /// <summary>
    /// Save Information To A Generated Prefab
    /// </summary>
    private void GenerateStockpile()
    {
        var meshGo = new GameObject("Stockpile", typeof(MeshFilter), typeof(MeshRenderer), typeof(Stockpile));
        var dataToAdd = meshGo.GetComponent<Stockpile>() ;
        dataToAdd.vertices[0] = vertices[0];
        dataToAdd.vertices[1] = vertices[1];
        dataToAdd.vertices[2] = vertices[2];
        dataToAdd.vertices[3] = vertices[3];
        dataToAdd.stockPileMaterial = stockpileMaterial;
        dataToAdd.DrawStockpile();
    }

    #endregion
    
    
}

