using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Interactions
{
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
        /// Workers
        /// </summary>
        [SerializeField] private List<Worker> workers;

        /// <summary>
        /// Stockpile
        /// </summary>
        [SerializeField] private Vector3[] vertices;
        private int[] triangles;
        private bool drawingStockpile;
        private Mesh stockpileMesh;

        private void Awake()
        {
            _playerInputActions = new PlayerInputActions();
            _playerInputActions.Enable();
            foreach (var villager in GameObject.FindGameObjectsWithTag("Villagers"))
            {
                villager.TryGetComponent(out Worker worker);
                workers.Add(worker);
            }

            stockpileMesh = new Mesh();
            stockpileMesh = GetComponent<MeshFilter>().mesh;
            vertices = new Vector3[4];
            triangles = new int[6];
        }

        private void Start()
        {
            _playerInputActions.Player.Select.performed += InteractObject;
            _playerInputActions.Player.Select.performed += DrawStockpile;
            _playerInputActions.Player.Select.canceled += DrawStockpile;
        }

        

        private void InteractObject(InputAction.CallbackContext context)
        {
            // Get the information of the object being clicked
            var ray = cam.ScreenPointToRay(_playerInputActions.UI.Point.ReadValue<Vector2>());
            if (!Physics.Raycast(ray, out var hit, 100)) 
                return;
        
            // Check if the clicked object has the component ObjectManager
            hit.transform.TryGetComponent(out HarvestObjectManager objectManager);
            if (!objectManager) 
                return;
        
            // Run through each worker for an available worker who is of the correct role.
            foreach (var worker in workers.Where(worker => objectManager.harvestableObject.canInteract.Contains(worker.role) && worker.interactingWith == null && objectManager.assignedWorker == null))
            {
                BeginWorking(worker, objectManager);
                worker.StartCoroutine(worker.MoveToJob(worker, objectManager));
                break;
            }
        }

        private void DrawStockpile(InputAction.CallbackContext context)
        {
            // Get the information of the object being clicked
            var ray = cam.ScreenPointToRay(_playerInputActions.UI.Point.ReadValue<Vector2>());
            if (!Physics.Raycast(ray, out var hit, 1000000)) 
                return;
        
            // Check if the clicked object is tagged with Ground
            if (!hit.transform.tag.Equals("Ground"))
                return;

            drawingStockpile = !drawingStockpile;
            var point = new Vector3(Mathf.CeilToInt(hit.point.x), hit.point.y + 0.3f, Mathf.CeilToInt(hit.point.z));
            vertices[0] = point;
            StartCoroutine(DrawStockpileEnumerator());
        }

        private IEnumerator DrawStockpileEnumerator()
        {
            var ray = cam.ScreenPointToRay(_playerInputActions.UI.Point.ReadValue<Vector2>());
            if (!Physics.Raycast(ray, out var hit, 1000000)) 
                yield break;
            
            Debug.Log("running");
            
            var startingPoint = vertices[0];
            var mousePosition = hit.point;
            
            if (!drawingStockpile) 
                yield break;
            vertices[1] = new Vector3(Mathf.CeilToInt(mousePosition.x),startingPoint.y + 0.03f,startingPoint.z);
            vertices[2] = new Vector3(startingPoint.x,startingPoint.y + 0.03f,Mathf.CeilToInt(mousePosition.z));
            vertices[3] = new Vector3(Mathf.CeilToInt(mousePosition.x),startingPoint.y + 0.03f,Mathf.CeilToInt(mousePosition.z));
            if ((vertices[1].x < vertices[0].x && vertices[2].z < vertices[0].z)||(vertices[1].x > vertices[0].x && vertices[2].z > vertices[0].z))
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
        
            stockpileMesh.vertices = vertices;
            stockpileMesh.triangles = triangles;
            yield return new WaitForEndOfFrame();
            StartCoroutine(DrawStockpileEnumerator());
        }

        private static void BeginWorking(Worker worker, HarvestObjectManager harvestObjectManager)
        {
            harvestObjectManager.assignedWorker = worker;
            worker.interactingWith = harvestObjectManager;
        }
        private void ShowVillagerInformation(InputAction.CallbackContext context)
        {
            var ray = cam.ScreenPointToRay(_playerInputActions.UI.Point.ReadValue<Vector2>());
            if (!Physics.Raycast(ray, out var hit, 100))
                return;

            TryGetComponent(out Worker worker);
            if (worker)
            {

            }
        }
    }
}
