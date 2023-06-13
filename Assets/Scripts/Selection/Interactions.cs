using UnityEngine;
using UnityEngine.InputSystem;

public class Interactions : MonoBehaviour
{
    
    private PlayerInputActions _playerInputActions;
    [SerializeField] private NewSelections selectionManager;
    [SerializeField] private Camera cam;

    private void Awake()
    {
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Enable();
    }

    private void Start()
    {
        _playerInputActions.Player.Select.performed += InteractObject;
    }

    private void InteractObject(InputAction.CallbackContext context)
    {
        var ray = cam.ScreenPointToRay(_playerInputActions.UI.Point.ReadValue<Vector2>());
        if (Physics.Raycast(ray, out var hit, 100))
        {
            hit.transform.TryGetComponent(out ObjectManager objectManager);
            if (objectManager)
            {
                foreach (var worker in selectionManager.selectedCharacters)
                {
                    worker.TryGetComponent(out Worker workerBrain);
                    if (objectManager._harvestableObject.canInteract.Contains(workerBrain.role))
                    {
                        BeginWorking(workerBrain, objectManager);
                    }
                }
            }
        }
    }

    private void BeginWorking(Worker worker, ObjectManager objectManager)
    {
        objectManager.assignedWorker = worker;
    }
}
