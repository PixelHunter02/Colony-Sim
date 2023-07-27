using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public string gameScene = "New Scene";
    public string mainMenu = "Main Menu";

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
            else if (_instance == null)
            {
                GameObject go = new GameObject("GameManager", typeof(GameManager));
            }

            return _instance;
        }
    }

    #endregion

    #region Game

    public InputManager inputManager;
    public Interactions interactionManager;
    public SettingsManager settingsManager;
    public StorageManager storageManager;
    public VillagerManager villagerManager;
    public Camera mainCamera;
    public BuildingManager buildingManager;
    public CraftingManager craftingManager;
    public TaskHandler taskHandler;
    public MonsterWaves monsterWaves;
    public Level level;
    public UIManager uiManager;
    
    public Grid grid;
    
    private GameManager _gameManager;
    
    private TMP_Text _villagerName;
    private static Dictionary<Villager, string> villagerLog;

    private TMP_Text roleSelectionTMPText;

    public bool IsOverUI() => EventSystem.current.IsPointerOverGameObject();

    // public bool stockpileMode;

    private List<CraftableSO> craftingButtons;

    private Villager lastSelected;
    #endregion

    #region MainMenu

    private GameObject mainMenuCanvas;
    private GameObject characterCreatorCanvas;

    [SerializeField] private GameObject characterCreationTemplate;
    [SerializeField] private Transform characterCreationContainer;

    #endregion


    private void Awake()
    {
        DontDestroyOnLoad(this);
    
        #region Singleton
    
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    
        #endregion
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += SceneSetup;
    }
    
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= SceneSetup;
    }
   
    public void GenerateStats()
    {
        for (int i = 0; i < characterCreationContainer.childCount; i++)
        {
            Destroy(characterCreationContainer.GetChild(i).gameObject);
        }
        
        foreach (var value in FindObjectsOfType(typeof(Villager)))
        {
            var villager = value.GetComponent<Villager>();
            villagerManager.GenerateNewVillagerStats(villager);
            
            var template = Instantiate(characterCreationTemplate, characterCreationContainer);
            var portrait = template.transform.GetChild(0);
            portrait.GetComponent<RawImage>().texture = value.GetComponent<Villager>()._portraitRenderTexture;
            var nameText = portrait.Find("Name").GetComponent<TMP_InputField>();
            nameText.text = villager.VillagerName;
            nameText.onEndEdit.AddListener(villager.EditName);
            portrait.Find("Strength").GetComponent<TMP_Text>().text = $"Strength: {villager.Strength}";
            portrait.Find("Craft").GetComponent<TMP_Text>().text = $"Craft: {villager.Craft}";
            portrait.Find("Magic").GetComponent<TMP_Text>().text = $"Magic: {villager.Magic}";
        }
    }

    public void NewGame()
    {
        if (SceneManager.GetActiveScene().name.Equals(mainMenu))
        {
            mainMenuCanvas.SetActive(false);
            characterCreatorCanvas.SetActive(true);
            GenerateStats();
        }
    }

    public void ContinueToGame()
    {
        if (SceneManager.GetActiveScene().name.Equals(mainMenu))
        {
            foreach (var villager in GameObject.FindGameObjectsWithTag("Villagers"))
            {
                // Debug.Log(villager);
                VillagerManager.AddVillagerToList(villager.GetComponent<Villager>());
            }

            // Debug.Log("Changed");
            SceneManager.LoadScene(gameScene);
        }
    }

    private void SceneSetup(Scene scene, LoadSceneMode mode)
    {
        if (scene.name.Equals(mainMenu))
        {
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            mainMenuCanvas = GameObject.Find("MainMenu");
            characterCreatorCanvas = GameObject.Find("Character Creation");
            mainMenuCanvas.SetActive(true);
            characterCreatorCanvas.SetActive(false);
        }
        if (scene.name.Equals("Main Menu 2"))
        {
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            mainMenuCanvas = GameObject.Find("MainMenu");
            characterCreatorCanvas = GameObject.Find("Character Creation");
            mainMenuCanvas.SetActive(true);
            characterCreatorCanvas.SetActive(false);
        }
        else if(scene.name.Equals(gameScene))
        {
            level = GameObject.Find("LocalSettings").GetComponent<Level>();
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        }
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
    TrippleSpeed,
    Building,
}

public enum ButtonType
{
    Craft,
    Build,
}