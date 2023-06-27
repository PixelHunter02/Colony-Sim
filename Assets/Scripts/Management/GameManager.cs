using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    #region SingletonPattern

    private static GameManager _instance;

    public static GameManager Instance { get { return _instance; } }

    #endregion

    public UIManager uiManager;
    public InputManager inputManager;
    public CameraMovement cameraMovement;
    public Interactions interactionManager;
    public SettingsManager settingsManager;
    public StorageManager storageManager;
    [FormerlySerializedAs("workerManager")] public VillagerManager villagerManager;
    public Camera mainCamera;
    
    private void Awake()
    {
        #region Singleton

        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }

        #endregion

        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }
}
