using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public WorkerManager workerManager;
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
        
    }
}
