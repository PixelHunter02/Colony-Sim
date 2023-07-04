using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuildingManager : MonoBehaviour
{
    GameManager _gameManager;

    public StoredItemSO currentBuilding;
    
    public static event Action<StoredItemSO> itemBuilt;
    
    private void Awake()
    {
        _gameManager = GameManager.Instance;
    }

    private void OnEnable()
    {
        _gameManager.inputManager.playerInputActions.Player.Select.performed += Building;
    }
    
    private void OnDisable()
    {
        _gameManager.inputManager.playerInputActions.Player.Select.performed -= Building;
    }

    private void Building(InputAction.CallbackContext context)
    {
        var mousePosition = _gameManager.inputManager.GetMouseToWorldPosition();
        if (mousePosition != Vector3.zero && _gameManager.inputManager.InputMode is InputMode.BuildMode &&
            currentBuilding != null)
        {
            var cellPosition = _gameManager.grid.WorldToCell(mousePosition);
            // PlaceBuilding(currentBuilding.prefab, cellPosition);
            Instantiate(currentBuilding.prefab, cellPosition, Quaternion.identity);
            itemBuilt?.Invoke(currentBuilding);
        }
    }
    
    public void PlaceBuilding(GameObject building, Vector3 buildingPosition)
    {
        // Debug.Log("Building Created");
        // Instantiate(building, buildingPosition, Quaternion.identity);
    }
}
