using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Cinemachine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Level : MonoBehaviour
{
    #region Game

    private GameManager _gameManager = GameManager.Instance;
    
    /// <summary>
    /// Menus
    /// </summary>
    [SerializeField] private GameObject craftingMenu;

    //Cameras
    [SerializeField] private CinemachineVirtualCamera mainVCamera;
    [SerializeField] private CinemachineVirtualCamera buildVCamera;

    private GameState gameState;
    public Material gridMaterial;

    public GameState GameState
    {
        get => gameState;
        set
        {
            gameState = value;
            switch (gameState)
            {
                case GameState.Playing:
                    Time.timeScale = 1;
                    CloseAllUI();
                    toolbar.SetActive(true);
                    mainVCamera.gameObject.SetActive(true);
                    buildVCamera.gameObject.SetActive(false);
                    break;
                case GameState.Paused:
                    CloseAllUI();
                    Time.timeScale = 0;
                    toolbar.SetActive(true);
                    break;
                case GameState.Crafting:
                    CloseAllUI();
                    toolbar.SetActive(true);
                    OpenCrafting();
                    break;
                case GameState.VillagerManagement:
                    CloseAllUI();
                    toolbar.SetActive(true);
                    _gameManager.uiManager.OpenVillagerMenu();
                    break;
                case GameState.Inventory:
                    CloseAllUI();
                    ShowInventory();
                    toolbar.SetActive(true);
                    if (tutorialManager.TutorialStage == TutorialStage.InventoryTutorial)
                    {
                        tutorialManager.TutorialStage = TutorialStage.CraftingTutorial;
                    }
                    break;
                case GameState.DoubleSpeed:
                    break;
                case GameState.TrippleSpeed:
                    break;
                case GameState.Building:
                    CloseAllUI();
                    buildingToolbar.SetActive(true);
                    toolbar.SetActive(false);
                    AddBuildingsToBuildingToolbar();
                    mainVCamera.gameObject.SetActive(false);
                    buildVCamera.gameObject.SetActive(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    [SerializeField] private Transform buildContentHolder;
    [SerializeField] private GameObject buildButtonTemplate;

    [SerializeField] private GameObject _infoUI;
    private TMP_Text _villagerName;
    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private GameObject villagerSelectUI;
    
    [SerializeField] private GameObject roleAssignmentUI;

    private static Dictionary<Villager, string> villagerLog;
    [SerializeField] private TMP_Text villagerLogTMP;
    public Transform inventoryMenu;
    
    public bool stockpileMode;

    [SerializeField] private GameObject craftingButtonTemplate;
    [SerializeField] private GameObject craftingContainer;
    private List<StoredItemSO> craftingButtons;

    private Villager lastSelected;

    [SerializeField] private GameObject buildingToolbar;
    [SerializeField] private GameObject toolbar;
    #endregion

    private Dictionary<StoredItemSO, GameObject> _buildingToolbarButtons;

    public Tutorial tutorialManager;

    public Camera Camera;

    public GameObject followObject;

    [SerializeField] private TMP_Text craftingDescription;
    [SerializeField] private Button addToQueueButton;
    [SerializeField] private Image craftingImage;

    public GameObject queueItemTemplate;
    public Transform queueItemContainer;
    
    public GameObject villageHeart;

    private float currentExpBoost = 1f;

    public float ExpBoostAmount
    {
        get => currentExpBoost;
        set
        {
            Debug.Log("Modified EXP Boost");
            currentExpBoost = value;
        }
    }
    private void Awake()
    {
        _villagerName = GameObject.Find("SelectedVillagerName").GetComponent<TMP_Text>();
        _infoUI.SetActive(false);
        CloseAllUI();
        toolbar.SetActive(true);
        villagerLog = new Dictionary<Villager, string>();
        craftingButtons = new List<StoredItemSO>();

        List<Villager> tempVillagers = new List<Villager>();
        foreach (var villager in VillagerManager.GetVillagers())
        {
            _gameManager.villagerManager.SpawnVillager(villager, out var villagerToReturn);
            tempVillagers.Add(villagerToReturn);
        }
        VillagerManager.villagers.Clear();

        foreach (var villager in tempVillagers)
        {
            VillagerManager.AddVillagerToList(villager);
        }

        _buildingToolbarButtons = new Dictionary<StoredItemSO, GameObject>();
        tutorialManager.TutorialStage = TutorialStage.VillagerStatsTutorial;
        gridMaterial.SetFloat("_Alpha", 0);

    }

    private void OnEnable()
    {
            AddToVillagerLogAction += ShowVillagerInformationOnUpdate;
    }

    private void OnDisable()
    {
            AddToVillagerLogAction -= ShowVillagerInformationOnUpdate;
    }

    private void Start()
    {
        _gameManager.inputManager.playerInputActions.Player.Escape.started += Pause;
    }
    
    private void Pause(InputAction.CallbackContext context)
    {
        if (_gameManager.inputManager.EscapePressed() && GameState != GameState.Playing)
        {
            GameState = GameState.Playing;
        }
        else if (_gameManager.inputManager.EscapePressed())
        {
            GameState = GameState.Paused;
        }
    }

    // On Villager Click
    public void ShowVillagerInformationOnClick(Villager villager)
    {
        if (lastSelected == villager && _infoUI.activeSelf)
        {
            _infoUI.SetActive(false);
        }
        else if (!lastSelected == villager && _infoUI.activeSelf)
        {
            lastSelected = villager;
            var storedLog = villagerLog.GetValueOrDefault(villager, String.Empty);
            villagerLogTMP.text = storedLog;
            _infoUI.SetActive(false);
            GameObject.Find("VillagerPortrait").GetComponent<RawImage>().texture = villager._portraitRenderTexture;
            _villagerName.text = villager.VillagerName;
        }
        else
        {
            lastSelected = villager;
            var storedLog = villagerLog.GetValueOrDefault(villager, String.Empty);
            villagerLogTMP.text = storedLog;
            _infoUI.SetActive(true);
            GameObject.Find("VillagerPortrait").GetComponent<RawImage>().texture = villager._portraitRenderTexture;
            _villagerName.text = villager.VillagerName;
        }

    }

    public void ShowVillagerInformationOnUpdate(Villager villager)
    {
        if (lastSelected == villager && _infoUI.activeSelf)
        {
            lastSelected = villager;
            var storedLog = villagerLog.GetValueOrDefault(villager, String.Empty);
            villagerLogTMP.text = storedLog;
            GameObject.Find("VillagerPortrait").GetComponent<RawImage>().texture = villager._portraitRenderTexture;
            _villagerName.text = villager.VillagerName;
            _gameManager.uiManager.SetVillagerStatsUI(villager);
        }
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
        if (GameState == GameState.Building)
        {
            GameState = GameState.Playing;
            gridMaterial.SetFloat("_Alpha", 0);
        }
        else
        {
            GameState = GameState.Building;
            gridMaterial.SetFloat("_Alpha", 1);
        }
    }


    public static event Action<Villager> AddToVillagerLogAction;
    public static void AddToVillagerLog(Villager villager, string newLog)
    {
        var storedLog = villagerLog.GetValueOrDefault(villager, String.Empty);
        StringBuilder log = new StringBuilder(storedLog);
        log.Append(newLog);
        log.Append(Environment.NewLine);
        villagerLog[villager] = log.ToString();
        AddToVillagerLogAction?.Invoke(villager);
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

    public void CloseAllUI()
    {
        GameState = GameState.Playing;
        _infoUI.SetActive(false);
        inventoryUI.SetActive(false);
        villagerSelectUI.SetActive(false);
        craftingContainer.transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(1500, 790);
        craftingMenu.SetActive(false);
        buildingToolbar.SetActive(false);
        villageHeart.GetComponent<VillageHeart>().villagerHeartMenu.SetActive(false);
    }

    public void StockpileModeEnabled()
    {
        stockpileMode = !stockpileMode;
        if (stockpileMode)
        {
            gridMaterial.SetFloat("_Alpha", 1);
        }
        else
        {
            gridMaterial.SetFloat("_Alpha", 0);
        }
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
        for (int i = 0; i < _gameManager.craftingManager.CraftingRecipes.Length; i++)
        {
            if (craftingButtons.Contains(_gameManager.craftingManager.CraftingRecipes[i]))
            {
                continue;
            }
            if(_gameManager.level.villageHeart.GetComponent<VillageHeart>().Level < _gameManager.craftingManager.CraftingRecipes[i].levelUnlocked)
            {
                return;
            }

            var button = Instantiate(craftingButtonTemplate, craftingContainer.transform);
            var image = button.transform.GetChild(0).GetComponent<Image>();
            image.sprite = _gameManager.craftingManager.CraftingRecipes[i].uiSprite;
            var buttonRef = button.GetComponent<ButtonReference>();
            buttonRef.recipeReference = _gameManager.craftingManager.CraftingRecipes[i];
            craftingButtons.Add(buttonRef.recipeReference);
            button.GetComponent<Button>().onClick.RemoveAllListeners();
            button.GetComponent<Button>().onClick.AddListener(() => ShowCraftingInformation(buttonRef.recipeReference));
        }
    }

    private void ShowCraftingInformation(StoredItemSO craftingRecipe)
    {
        craftingDescription.text = craftingRecipe.itemDescrition;
        craftingImage.sprite = craftingRecipe.uiSprite;
        craftingContainer.transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(775, 790);
        craftingImage.transform.GetChild(0).GetComponent<TMP_Text>().text = craftingRecipe.objectName;
        addToQueueButton.onClick.RemoveAllListeners();
        addToQueueButton.onClick.AddListener(() => AddToCraftingQueue(craftingRecipe));
    }

    private void AddToCraftingQueue(StoredItemSO craftingRecipe)
    {
        
        var queuedItem = Instantiate(queueItemTemplate, queueItemContainer);
        queuedItem.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = craftingRecipe.uiSprite;
        var craftingItem = _gameManager.craftingManager.BeginCrafting(craftingRecipe);
        _gameManager.craftingManager.craftingQueueDictionary.Add(craftingItem,queuedItem);
        _gameManager.craftingManager.craftingQueue.Enqueue(craftingItem);
    }

    private void AddBuildingsToBuildingToolbar()
    {
        foreach (var building in _gameManager.buildingManager.buildings)
        {
            if (_buildingToolbarButtons.ContainsKey(building))
            {
                continue;
            }
            var newButton = Instantiate(buildButtonTemplate, buildContentHolder);
            newButton.GetComponent<Button>().onClick.AddListener(() => _gameManager.buildingManager.currentBuilding = building);
            newButton.GetComponent<Button>().onClick.AddListener(() => _gameManager.buildingManager.PreviewSetup(_gameManager.buildingManager.currentBuilding.prefab));
            _buildingToolbarButtons.TryAdd(building,newButton);
        }
    }
}
