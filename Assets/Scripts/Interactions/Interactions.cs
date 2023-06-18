using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


public class Interactions : MonoBehaviour
{
    /// <summary>
    /// Input Actions
    /// </summary>
    private PlayerInputActions _playerInputActions;

    /// <summary>
    /// Camera
    /// </summary>
    [SerializeField] private Camera cam;

    /// <summary>
    /// Stockpile
    /// </summary>
    [SerializeField]private Vector3[] vertices;
    private int[] _triangles;
    private Vector2[] _uvs; 
    private bool _drawingStockpile;
    private Mesh _stockpileMesh;

    private Outline lastHitOutline;

    private TaskHandler _taskHandler;

    /// <summary>
    /// Material References
    /// </summary>
    [SerializeField] Material stockpileMaterial;
    
    private void Awake()
    {
        
        // Enable Input Actions
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Enable();

        // Assign references for Stockpile information
        _stockpileMesh = new Mesh();
        _stockpileMesh = GetComponent<MeshFilter>().mesh;
        vertices = new Vector3[4];
        _uvs = new Vector2[4];
        _triangles = new int[6];
    }

    private void Start()
    {
        _playerInputActions.Player.Select.performed += Interact;
        _playerInputActions.Player.Select.canceled += Interact;
    }

    private void Update()
    {
        var ray = cam.ScreenPointToRay(_playerInputActions.UI.Point.ReadValue<Vector2>());
        if (!Physics.Raycast(ray, out var hit)) 
            return;
        
        // Clear the reference to the previous outline if not highlighting.
        if (lastHitOutline)
        {
            lastHitOutline.enabled = false;
            lastHitOutline = null;
        }

        // Detect if there is an outline component on the gameobject, If there is enable it
        if (!hit.transform.TryGetComponent(out Outline outline)) 
            return;
        lastHitOutline = outline;
        outline.enabled = true;
    }

    private void Interact(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                Interactable();
                DrawStockpile();
                break;
            case InputActionPhase.Canceled:
                PlaceStockpile();
                break;
        }
    }

    private void Interactable()
    {
        // Get the information of the object being clicked
        var ray = cam.ScreenPointToRay(_playerInputActions.UI.Point.ReadValue<Vector2>());
        if (!Physics.Raycast(ray, out var hit, 100)) 
            return;

        if (hit.transform.TryGetComponent(out IInteractable interactable))
        {
            interactable.Interact();
        }
    }
    
    #region Stockpiles

    private void DrawStockpile()
    {
        // Get the information of the object being clicked
        var ray = cam.ScreenPointToRay(_playerInputActions.UI.Point.ReadValue<Vector2>());
        if (!Physics.Raycast(ray, out var hit, 1000000)) 
            return;
    
        // Check if the clicked object is tagged with Ground
        if (!hit.transform.tag.Equals("Ground"))
            return;

        _drawingStockpile = true;
        var point = new Vector3(Mathf.CeilToInt(hit.point.x), hit.point.y + 0.1f, Mathf.CeilToInt(hit.point.z));
        vertices[0] = point;
        StartCoroutine(CRDrawStockpile());
    }
    
    private IEnumerator CRDrawStockpile()
    {
        var ray = cam.ScreenPointToRay(_playerInputActions.UI.Point.ReadValue<Vector2>());
        if (!Physics.Raycast(ray, out var hit, 1000000)) 
            yield break;
        
        var startingPoint = vertices[0];
        var mousePosition = hit.point;
        
        if (!_drawingStockpile) 
            yield break;
        vertices[1] = new Vector3(Mathf.CeilToInt(mousePosition.x),startingPoint.y,startingPoint.z);
        vertices[2] = new Vector3(startingPoint.x,startingPoint.y,Mathf.CeilToInt(mousePosition.z));
        vertices[3] = new Vector3(Mathf.CeilToInt(mousePosition.x),startingPoint.y,Mathf.CeilToInt(mousePosition.z));

        _uvs[0] = new Vector2(0, 1);
        _uvs[1] = new Vector2(1, 1);
        _uvs[2] = new Vector2(0, 0);
        _uvs[3] = new Vector2(1, 0);

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
    
        _stockpileMesh.vertices = vertices;
        _stockpileMesh.uv = _uvs;
        _stockpileMesh.triangles = _triangles;
        yield return new WaitForEndOfFrame();
        StartCoroutine(CRDrawStockpile());
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
        var meshGo = new GameObject("Stockpile", typeof(MeshFilter), typeof(MeshRenderer),typeof(Stockpile));
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

