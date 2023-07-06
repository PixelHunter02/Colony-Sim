using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region SingletonPattern

    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null && FindObjectOfType<GameManager>(true))
            {
                _instance = FindObjectOfType<GameManager>(true);
            }
            else if(_instance == null)
            {
                GameObject go = new GameObject("GameManager", typeof(GameManager));
            }
            return _instance;
        }
    }

    #endregion

    public UIManager uiManager;
    public InputManager inputManager;
    public CameraMovement cameraMovement;
    public Interactions interactionManager;
    public SettingsManager settingsManager;
    public StorageManager storageManager;
    public VillagerManager villagerManager;
    public Camera mainCamera;
    public BuildingManager buildingManager;
    public CraftingManager craftingManager;

    public Grid grid;

    /// <summary>
    /// Menus
    /// </summary>
    [SerializeField] private GameObject craftingMenu;

    private GameState gameState;

    public GameState GameState
    {
        get => gameState;
        set
        {
            gameState = value;
            switch (gameState)
            {
                case GameState.Playing:
                    CloseAllUI();
                    break;
                case GameState.Paused:
                    Time.timeScale = 0;
                    break;
                case GameState.Crafting:
                    CloseAllUI();
                    OpenCrafting();
                    break;
                case GameState.VillagerManagement:
                    CloseAllUI();
                    ShowVillagerSelectUI();
                    break;
                case GameState.Inventory:
                    CloseAllUI();
                    ShowInventory();
                    break;
                case GameState.DoubleSpeed:
                    break;
                case GameState.TrippleSpeed:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
    
    private GameManager _gameManager;
    
    [SerializeField] private GameObject _infoUI;
    private TMP_Text _villagerName;
    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private GameObject villagerManagementUI;
    [SerializeField] private GameObject villagerSelectUI;

    [SerializeField] private GameObject villagerManagementCell;
    [SerializeField] private Transform villagerManagementContainer;

    [SerializeField] private Button[] roleButtons;
    [SerializeField] private GameObject roleAssignmentUI;

    private static Dictionary<Villager, string> villagerLog;
    [SerializeField] private TMP_Text villagerLogTMP;

    private TMP_Text roleSelectionTMPText;
    
    public bool IsOverUI() => EventSystem.current.IsPointerOverGameObject();

    public bool stockpileMode;

    [SerializeField]private GameObject craftingButtonTemplate;
    [SerializeField]private GameObject craftingContainer;
    private List<CraftableSO> craftingButtons;
    
    private void Awake()
    {
        #region Singleton

        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        } else {
            _instance = this;
        }

        #endregion

        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _villagerName = GameObject.Find("SelectedVillagerName").GetComponent<TMP_Text>();
        roleSelectionTMPText = GameObject.Find("RoleText").GetComponent<TMP_Text>();
        villagerLog = new Dictionary<Villager, string>();
        craftingButtons = new List<CraftableSO>();
        _infoUI.SetActive(false);
        CloseAllUI();
    }
    
    private void Start()
    {
        UpdateVillagerManagementUI();
    }

    private void Update()
    {
        if (_gameManager.inputManager.EscapePressed())
        {
            CloseAllUI();
        }
    }

    // On Villager Click
    public void ShowVillagerInformation(Villager villager)
    {
        var storedLog = villagerLog.GetValueOrDefault(villager, String.Empty);
        villagerLogTMP.text = storedLog;
        _infoUI.SetActive(true);
        _villagerName.text = villager.VillagerName;
    }

    // Button Set In Inspector
    private void ShowVillagerSelectUI()
    {
        villagerSelectUI.SetActive(true);
    }

    public void VillagerUI()
    {
        if (GameState is GameState.VillagerManagement)
        {
            GameState = GameState.Playing;
        }
        else
        {
            GameState = GameState.VillagerManagement;
        }
    }
    
    public void BuildMode()
    {
        if (inputManager.InputMode == InputMode.BuildMode)
        {
            inputManager.InputMode = InputMode.DefaultMode;
        }
        else
        {
            inputManager.InputMode = InputMode.BuildMode;
        }
    }

    private void UpdateVillagerManagementUI()
    {
        for (int i = 0; i < VillagerManager.GetVillagers().Count; i++)
        {
            var villager = VillagerManager.GetVillagers()[i];
            var cell = Instantiate(villagerManagementCell, villagerManagementContainer);
            cell.transform.Find("Label").Find("Name").GetComponent<TMP_Text>().text = villager.VillagerName;
            var button = cell.transform.Find("Button").GetComponent<Button>();
            button.GetComponent<ButtonReference>().workerReference = villager;
            button.onClick.AddListener(() => OpenRoleManagementUI(button.GetComponent<ButtonReference>().workerReference));
             Debug.Log(button.GetComponent<ButtonReference>().workerReference);
            cell.SetActive(true);
        }
    }

    private void OpenRoleManagementUI(Villager villager)
    {
        foreach (var button in roleButtons)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => SetVillagerRole(villager));
        }
        CloseAllUI();
        roleSelectionTMPText.text = "Select Role For: \n" + villager.VillagerName;
        villagerManagementUI.SetActive(true);
        roleAssignmentUI.SetActive(true);
    }

    private void SetVillagerRole(Villager villager)
    {
        string originalRole = villager.CurrentRole.ToString();
        Enum.TryParse(EventSystem.current.currentSelectedGameObject.name, out Roles role);
        villager.CurrentRole = role;
        AddToVillagerLog(villager,villager.VillagerName + " has changed from " + originalRole + " to " + role);
        GameState = GameState.Playing;
    }
    
    public static void AddToVillagerLog(Villager villager, string newLog)
    {
        var storedLog = GameManager.villagerLog.GetValueOrDefault(villager, String.Empty);
        StringBuilder log = new StringBuilder(storedLog);
        log.Append(newLog);
        log.Append(Environment.NewLine);
        GameManager.villagerLog[villager] = log.ToString();
        // TODO Add To Villager Log In Scene When Built
    }

    // Button Set In Inspector
    private void ShowInventory()
    {
        inventoryUI.SetActive(true);
    }

    public void Inventory()
    {
        if (GameState is GameState.Inventory)
        {
            GameState = GameState.Playing;
        }
        else
        {
            GameState = GameState.Inventory;
        }
    }

    private void CloseAllUI()
    {
        _infoUI.SetActive(false);
        inventoryUI.SetActive(false);
        roleAssignmentUI.SetActive(false);
        villagerSelectUI.SetActive(false);
        craftingMenu.SetActive(false);
    }

    public void StockpileModeEnabled()
    {
        stockpileMode = !stockpileMode;
        _gameManager.inputManager.InputMode = InputMode.Stockpile;
    }

    // Set Mode to Crafting
    public void Crafting()
    {
        if (GameState is GameState.Crafting)
        {
            GameState = GameState.Playing;
        }
        else
        {
            GameState = GameState.Crafting;
        }
    }

    private void OpenCrafting()
    {
        craftingMenu.SetActive(true);
        for (int i = 0; i < craftingManager.CraftingRecipes.Length; i++)
        {
            if (craftingButtons.Contains(craftingManager.CraftingRecipes[i]))
            {
                continue;
            }
            var button = Instantiate(craftingButtonTemplate, craftingContainer.transform);
            var image = button.transform.GetChild(0).GetComponent<Image>();
            image.sprite = craftingManager.CraftingRecipes[i].sprite;
            var buttonRef = button.GetComponent<ButtonReference>();
            buttonRef.recipeReference = craftingManager.CraftingRecipes[i];
            craftingButtons.Add(buttonRef.recipeReference);
            button.GetComponent<Button>().onClick.AddListener(() => StartCoroutine(craftingManager.BeginCrafting(buttonRef.recipeReference)));
        }
    }

    public static bool TryGetVillagerByRole(Roles role, out Villager value)
    {
        Villager villagerToReturn = null;
        foreach (var villager in VillagerManager.GetVillagers())
        {
            if (villagerToReturn)
            {
                if (villager.CurrentRole == role)
                {
                    Debug.Log("Found Worker");
                    if (villagerToReturn.TasksToQueue.Count > villager.TasksToQueue.Count ||
                        villager.CurrentState is VillagerStates.Idle)
                    {
                        villagerToReturn = villager;
                        Debug.Log(villager.TasksToQueue.Count + " : " + villager.VillagerName);
                        value = villagerToReturn;
                    }
                }
            }
            else
            {
                if (villager.CurrentRole == role)
                {
                    villagerToReturn = villager;
                    value = villagerToReturn; 
                }
            }
        }
        value = villagerToReturn;
        return villagerToReturn;
        
        
// // Run through each worker for an available worker who is of the correct role.
//         foreach (var worker in VillagerManager.GetVillagers())
//         {
//             if (!harvestableObject.canInteract.Contains(worker.CurrentRole) || assignedVillager != null) 
//                 continue;
//             if (workerToAssign)
//             {
//                 if (workerToAssign.TasksToQueue.Count > worker.TasksToQueue.Count || worker.CurrentState is VillagerStates.Idle)
//                 {
//                     workerToAssign = worker;
//                     Debug.Log(worker.TasksToQueue.Count + " : " + worker.VillagerName);
//                 }
//             }
//             else
//             {
//                 workerToAssign = worker;
//                 Debug.Log(worker.TasksToQueue.Count + " : " + worker.VillagerName);
//
//             }
//         }
    }
}

public enum GameState
{
    Playing,
    Paused,
    Crafting,
    VillagerManagement,
    Inventory,
    DoubleSpeed,
    TrippleSpeed
}
