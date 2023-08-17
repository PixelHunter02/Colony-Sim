using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

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
    
    public bool IsOverUI() => EventSystem.current.IsPointerOverGameObject();
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

        HarvestableObject.onHarvestCompletedEvent += level.villagerNavMesh.BuildNavMesh;
        HarvestableObject.onHarvestCompletedEvent += level.monsterNavMesh.BuildNavMesh;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= SceneSetup;
        
        HarvestableObject.onHarvestCompletedEvent -= level.villagerNavMesh.BuildNavMesh;
        HarvestableObject.onHarvestCompletedEvent -= level.monsterNavMesh.BuildNavMesh;
    }

    private void SceneSetup(Scene scene, LoadSceneMode mode)
    {
        if(scene.name.Equals(gameScene) || SceneManager.GetActiveScene().name.Equals("Tablet"))
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