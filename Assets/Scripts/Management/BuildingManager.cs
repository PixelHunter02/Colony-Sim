using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class BuildingManager : MonoBehaviour
{
    GameManager _gameManager;

    public StoredItemSO currentBuilding;
    
    public static event Action<StoredItemSO> itemBuilt;

    private int currentBuildingRotationModifier;

    private GameObject previewGO;

    [SerializeField] private Material placmentMaterial;

    public StoredItemSO[] buildings;
    
    private void Awake()
    {
        _gameManager = GameManager.Instance;
    }

    private void OnEnable()
    {
        _gameManager.inputManager.playerInputActions.Player.Select.performed += PlaceBuilding;
        _gameManager.inputManager.playerInputActions.Player.RotateBuilding.performed += RotateBuilding; 
    }
    
    private void OnDisable()
    {

        _gameManager.inputManager.playerInputActions.Player.Select.performed -= PlaceBuilding;
        _gameManager.inputManager.playerInputActions.Player.RotateBuilding.performed -= RotateBuilding;
    }

    private void Update()
    {
        if(SceneManager.GetActiveScene().name.Equals("GameScene"))
            MovePreview(previewGO);
    }

    public void PreviewSetup(GameObject go)
    {
        previewGO = Instantiate(go);
        if (previewGO.transform.childCount > 0)
        {
            previewGO.GetComponentInChildren<MeshCollider>().enabled = false;
            previewGO.GetComponentInChildren<NavMeshObstacle>().enabled = false;
            previewGO.GetComponentInChildren<Renderer>().material = placmentMaterial;
        }
        else
        {
            previewGO.GetComponent<MeshCollider>().enabled = false;
            previewGO.GetComponent<NavMeshObstacle>().enabled = false;
            previewGO.GetComponent<Renderer>().material = placmentMaterial;
        }
    }

    public void MovePreview(GameObject previewObj)
    {
        if (previewObj != null)
        {
            // if (_gameManager.level.GameState is not GameState.Building)
            // {
            //     Destroy(previewGO);
            // }
            
            var mousePosition = _gameManager.inputManager.GetMouseToWorldPosition();
            Vector3Int cellPosition = new Vector3Int();
            Vector3 vec3 = new Vector3();
            if (mousePosition != Vector3.zero && currentBuilding != null)
            {
                previewGO.SetActive(true);
                cellPosition = _gameManager.grid.WorldToCell(mousePosition);
                vec3 = cellPosition;
                vec3.y = -1.5f;
            }
            else
            {
                previewGO.SetActive(false);
            }

            previewObj.transform.position = vec3;
            previewGO.transform.eulerAngles = new Vector3(0, 90 * currentBuildingRotationModifier, 0);
        }
    }

    public static event Action TutorialStageNine;

    private void PlaceBuilding(InputAction.CallbackContext context)
    {
        var mousePosition = _gameManager.inputManager.GetMouseToWorldPosition();
        if (mousePosition != Vector3.zero && currentBuilding != null)
        {
            var cellPosition = _gameManager.grid.WorldToCell(mousePosition);
            Vector3 vec3 = cellPosition;
            vec3.y = -1.5f;
            var placedItem = Instantiate(currentBuilding.prefab, vec3, Quaternion.identity);

            placedItem.transform.eulerAngles = new Vector3(0, 90 * currentBuildingRotationModifier, 0);
            itemBuilt?.Invoke(currentBuilding);
            placedItem.GetComponent<BuildStats>().enabled = true;
            // _gameManager.taskHandler.queuedTasks.Add(_gameManager.taskHandler.TaskToAssign(placedItem.GetComponent<BuildStats>()));
            Coroutine cr = StartCoroutine(_gameManager.taskHandler.TaskToAssign(placedItem.GetComponent<BuildStats>()));
            _gameManager.taskHandler.queuedTasks.Enqueue(cr);
            TutorialStageNine?.Invoke();
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

    public void AssignBuilding(StoredItemSO building)
    {
        currentBuilding = building;
        PreviewSetup(building.prefab);
    }
}
