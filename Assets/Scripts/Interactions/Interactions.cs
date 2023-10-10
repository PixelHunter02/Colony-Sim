using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class Interactions : MonoBehaviour
{
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
        if(SceneManager.GetActiveScene().name.Equals("New Scene"))
            OutlineInteractable();
    }

    private void OutlineInteractable()
    {
        var ray = _gameManager.mainCamera.ScreenPointToRay(_gameManager.inputManager.playerInputActions.UI.Point.ReadValue<Vector2>());
        Debug.Log(ray.GetPoint(10));
        if (!Physics.Raycast(ray, out var hit))
            return;
        Debug.Log(hit.transform.name);

        // Clear the reference to the previous outline if not highlighting.
        if (_lastHitOutline)
        {
            _lastHitOutline.enabled = false;
            _lastHitOutline = null;
        }

        // Detect if there is an outline component on the gameobject, If there is enable it
        if (!hit.transform.TryGetComponent(out Outline outline))
            return;

        if (hit.transform.TryGetComponent(out IInteractable interactable) && interactable.CanInteract())
        {
            _lastHitOutline = outline;
            outline.enabled = true;
        }
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
        var ray = _gameManager.level.Camera.ScreenPointToRay(_gameManager.inputManager.playerInputActions.UI.Point.ReadValue<Vector2>());
        if (!Physics.Raycast(ray, out var hit, 1000))
            return;
        Debug.Log(hit.transform.name);
        if (hit.transform.TryGetComponent(out IInteractable interactable) && !isOverUI)
        {
            // Debug.Log(hit.transform.name);
            interactable.OnInteraction();
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
        var ray = _gameManager.level.Camera.ScreenPointToRay(_gameManager.inputManager.playerInputActions.UI.Point.ReadValue<Vector2>());
        if (!Physics.Raycast(ray, out var hit, 100,ground) || isOverUI ||!_gameManager.level.stockpileMode) 
            return;

        _drawingStockpile = true;
        var point = new Vector3(Mathf.FloorToInt(hit.point.x), hit.point.y + 0.1f, Mathf.FloorToInt(hit.point.z));
        vertices[0] = point;
        StartCoroutine(DrawStockpileCR());
    }
    
    private IEnumerator DrawStockpileCR()
    {
        var ray = _gameManager.level.Camera.ScreenPointToRay(_gameManager.inputManager.playerInputActions.UI.Point.ReadValue<Vector2>());
        if (!Physics.Raycast(ray, out var hit, 100,ground)) 
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
     
        if (_gameManager.level.tutorialManager.TutorialStage == TutorialStage.StockpileTutorial)
        {
            _gameManager.level.tutorialManager.TutorialStage = TutorialStage.InventoryTutorial;
        }

        _gameManager.level.stockpileMode = false;
        _drawingStockpile = false;
        GenerateStockpile();
        _stockpileMesh.Clear();
        _gameManager.level.gridMaterial.SetFloat("_Alpha", 0);
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

