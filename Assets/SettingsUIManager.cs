using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

public class SettingsUIManager : MonoBehaviour
{
    // Core
    public static SettingsUIManager Instance;
    private UIDocument uiDocument;
    private VisualElement root;
    private GameManager _gameManager;

    private UIToolkitManager _uiToolkitManager;
    
    // Audio
    private Slider masterVolumeSlider;
    private Slider villagerSlider;
    private Slider backgroundMusicSldier;
    private Slider uiVolumeSlider;
    public AudioMixer audioMixer;
    
    //Camera Controls
    private Slider mousePanSpeedSlider;
    private Slider keyboardPanSpeedSlider;    
    private Slider rotationSpeedSlider;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
        
        if (!_gameManager)
        {
            _gameManager = GameManager.Instance;
        }
    }

    private void Start()
    {
        //Core References
        uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;
        root.Q("CloseButton").Q<Button>().RegisterCallback<ClickEvent>(evt =>
        {
            root.AddToClassList("SettingsPanelDown");
        });
        
        SetUpVolumeSettings();
        SetUpCameraSettings();
        
        root.AddToClassList("SettingsPanelDown");
    }
    
    // SetupVolume
    public void SetUpVolumeSettings()
    {
        // Volume
        masterVolumeSlider = root.Q<Slider>("MasterVolumeSlider");
        audioMixer.GetFloat("Master", out float masterValue);
        masterVolumeSlider.SetValueWithoutNotify(Mathf.Pow(10, (masterValue / 20)));
        masterVolumeSlider.RegisterCallback<ChangeEvent<float>>(evt =>
        {
            audioMixer.SetFloat("Master", Mathf.Log10(evt.newValue)*20);
        });
        
        backgroundMusicSldier = root.Q<Slider>("MusicVolumeSlider");
        audioMixer.GetFloat("Music", out float musicValue);
        backgroundMusicSldier.SetValueWithoutNotify(Mathf.Pow(10, (musicValue / 20)));
        backgroundMusicSldier.RegisterCallback<ChangeEvent<float>>(evt =>
        {
            audioMixer.SetFloat("Music", Mathf.Log10(evt.newValue)*20);
        });
        
        villagerSlider = root.Q<Slider>("VillagerVolumeSlider");
        audioMixer.GetFloat("Villager", out float villagerValue);
        villagerSlider.SetValueWithoutNotify(Mathf.Pow(10, (villagerValue / 20)));
        villagerSlider.RegisterCallback<ChangeEvent<float>>(evt =>
        {
            audioMixer.SetFloat("Villager", Mathf.Log10(evt.newValue)*20);
        });
        
        uiVolumeSlider = root.Q<Slider>("UIVolumeSlider");
        audioMixer.GetFloat("UI", out float uiValue);
        uiVolumeSlider.SetValueWithoutNotify(Mathf.Pow(10, (uiValue / 20)));
        uiVolumeSlider.RegisterCallback<ChangeEvent<float>>(evt =>
        {
            audioMixer.SetFloat("UI", Mathf.Log10(evt.newValue)*20);
        });
    }

    // Setup Camera Settings
    public void SetUpCameraSettings()
    {
        mousePanSpeedSlider = root.Q<Slider>("MousePanSpeedSlider");
        keyboardPanSpeedSlider = root.Q<Slider>("KeyboardMoveSpeedSlider");
        rotationSpeedSlider = root.Q<Slider>("CameraRotateSlider");
        
        mousePanSpeedSlider.SetValueWithoutNotify(SettingsManager.mousePanSpeedModifier);
        mousePanSpeedSlider.RegisterCallback<ChangeEvent<float>>(evt =>
        {
            SettingsManager.mousePanSpeedModifier = evt.newValue;
        });
        
        keyboardPanSpeedSlider.SetValueWithoutNotify(SettingsManager.keyboardMoveSpeedModifier);
        keyboardPanSpeedSlider.RegisterCallback<ChangeEvent<float>>(evt =>
        {
            SettingsManager.keyboardMoveSpeedModifier = evt.newValue;
        });
        
        rotationSpeedSlider.SetValueWithoutNotify(SettingsManager.rotationSpeedModifier);
        rotationSpeedSlider.RegisterCallback<ChangeEvent<float>>(evt =>
        {
            SettingsManager.rotationSpeedModifier = evt.newValue;
        });
    }
    public void OpenSettingsUI()
    {
        root.AddToClassList("SettingsPanelUp");
        root.RemoveFromClassList("SettingsPanelDown");
    } 
    
    public void CloseSettingsUI()
    {
        root.AddToClassList("SettingsPanelDown");
        root.RemoveFromClassList("SettingsPanelUp");
    } 
}
