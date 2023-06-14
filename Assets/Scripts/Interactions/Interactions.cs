using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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
        [SerializeField]private Vector3[] vertices;
        private int[] _triangles;
        private Vector2[] _uvs; 
        private bool _drawingStockpile;
        private Mesh _stockpileMesh;

        private void Awake()
        {
            _playerInputActions = new PlayerInputActions();
            _playerInputActions.Enable();
            foreach (var villager in GameObject.FindGameObjectsWithTag("Villagers"))
            {
                villager.TryGetComponent(out Worker worker);
                workers.Add(worker);
            }

            _stockpileMesh = new Mesh();
            _stockpileMesh = GetComponent<MeshFilter>().mesh;
            vertices = new Vector3[4];
            _uvs = new Vector2[4];
            _triangles = new int[6];
        }

        private void Start()
        {
            _playerInputActions.Player.Select.performed += InteractObject;
            _playerInputActions.Player.Select.performed += DrawStockpile;
            _playerInputActions.Player.Select.performed += ShowVillagerInformation;
            _playerInputActions.Player.Select.canceled += PlaceStockpile;
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

            _drawingStockpile = true;
            var point = new Vector3(Mathf.CeilToInt(hit.point.x), hit.point.y + 0.1f, Mathf.CeilToInt(hit.point.z));
            vertices[0] = point;
            StartCoroutine(DrawStockpileEnumerator());
        }

        private IEnumerator DrawStockpileEnumerator()
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
            // if (Mathf.CeilToInt(vertices[0].x) == Mathf.CeilToInt(vertices[1].x))
            // {
            //     vertices[1].x -= 1;
            //     vertices[3].x -= 1;
            // }
            // if (Mathf.CeilToInt(vertices[0].z) == Mathf.CeilToInt(vertices[2].z))
            // {
            //     vertices[0].z -= 1;
            //     vertices[1].z -= 1;
            // }
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
            StartCoroutine(DrawStockpileEnumerator());
        }

        private void PlaceStockpile(InputAction.CallbackContext context)
        {
            _drawingStockpile = false;
            GenerateStockpile();
            _stockpileMesh.Clear();
        }
        
        private static void BeginWorking(Worker worker, HarvestObjectManager harvestObjectManager)
        {
            harvestObjectManager.assignedWorker = worker;
            worker.interactingWith = harvestObjectManager;
        }
        private void ShowVillagerInformation(InputAction.CallbackContext context)
        {
            print("ShowVillagerInformation");
            var ray = cam.ScreenPointToRay(_playerInputActions.UI.Point.ReadValue<Vector2>());
            if (!Physics.Raycast(ray, out var hit, 100))
                return;

            hit.transform.TryGetComponent(out Worker worker);
            print(worker);
            if (worker)
            {
                //setting values for info ui of villagers
                var infoUI = worker.gameObject.transform.GetChild(0).GetChild(1).gameObject;
                infoUI.SetActive(true);
                infoUI.transform.Find("Name").TryGetComponent(out TMP_Text name);
                infoUI.transform.Find("Job").TryGetComponent(out TMP_Text job);
                name.text = worker.name;
                job.text = worker.role.ToString();
            }
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
            dataToAdd.DrawBox();
        }
    }
}
