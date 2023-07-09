using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class BuildingManager : MonoBehaviour
{
    GameManager _gameManager;

    public StoredItemSO currentBuilding;
    
    public static event Action<StoredItemSO> itemBuilt;

    private int currentBuildingRotationModifier;

    private GameObject previewGO;

    [SerializeField] private Material placmentMaterial;
    
    private void Awake()
    {
        _gameManager = GameManager.Instance;
    }

    private void OnEnable()
    {
        _gameManager.inputManager.playerInputActions.Player.Select.performed += Building;
        _gameManager.inputManager.playerInputActions.Player.RotateBuilding.performed += RotateBuilding;
    }
    
    private void OnDisable()
    {
        _gameManager.inputManager.playerInputActions.Player.Select.performed -= Building;
        _gameManager.inputManager.playerInputActions.Player.RotateBuilding.performed -= RotateBuilding;
    }

    private void Update()
    {
        MovePreview(previewGO);
    }

    public void PreviewSetup(GameObject go)
    {
        previewGO = Instantiate(go);
        previewGO.GetComponentInChildren<MeshCollider>().enabled = false;
        previewGO.GetComponentInChildren<NavMeshObstacle>().enabled = false;
        previewGO.GetComponentInChildren<Renderer>().material = placmentMaterial;
    }

    public void MovePreview(GameObject previewObj)
    {
        if (previewObj != null)
        {
            if (_gameManager.inputManager.InputMode is not InputMode.BuildMode)
            {
                Destroy(previewGO);
            }
            
            var mousePosition = _gameManager.inputManager.GetMouseToWorldPosition();
            Vector3Int cellPosition = new Vector3Int();
            if (mousePosition != Vector3.zero && _gameManager.inputManager.InputMode is InputMode.BuildMode &&
                currentBuilding != null)
            {
                cellPosition = _gameManager.grid.WorldToCell(mousePosition);
            }

            previewObj.transform.position = cellPosition;
            previewGO.transform.eulerAngles = new Vector3(0, 90 * currentBuildingRotationModifier, 0);
        }
    }

    private void Building(InputAction.CallbackContext context)
    {
        var mousePosition = _gameManager.inputManager.GetMouseToWorldPosition();
        if (mousePosition != Vector3.zero && _gameManager.inputManager.InputMode is InputMode.BuildMode &&
            currentBuilding != null)
        {

            var cellPosition = _gameManager.grid.WorldToCell(mousePosition);
            // PlaceBuilding(currentBuilding.prefab, cellPosition);
            var placedItem = Instantiate(currentBuilding.prefab, cellPosition, Quaternion.identity);
            placedItem.transform.eulerAngles = new Vector3(0, 90 * currentBuildingRotationModifier, 0);
            itemBuilt?.Invoke(currentBuilding);
        }
    }

    public void RotateBuilding(InputAction.CallbackContext context)
    {
        if (context.ReadValue<float>() > 0)
        {
            currentBuildingRotationModifier++;
            if (currentBuildingRotationModifier > 3)
            {
                currentBuildingRotationModifier = 0;
            }
        }
        else
        {
            currentBuildingRotationModifier--;
            if (currentBuildingRotationModifier < 0)
            {
                currentBuildingRotationModifier = 3;
            }
        }
    }
}
