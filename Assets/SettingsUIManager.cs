using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

public class SettingsUIManager : MonoBehaviour
{
    // Core
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

    private void Awake()
    {
        if (!_gameManager)
        {
            _gameManager = GameManager.Instance;
        }
    }

    private void Start()
    {
        uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;
        root.Q("CloseButton").Q<Button>().RegisterCallback<ClickEvent>(evt =>
        {
            root.AddToClassList("SettingsPanelDown");
        });
        
        // Volume
        masterVolumeSlider = root.Q<Slider>("MasterVolumeSlider");
        masterVolumeSlider.RegisterCallback<ChangeEvent<float>>(evt =>
        {
            audioMixer.SetFloat("Master", Mathf.Log10(evt.newValue)*20);
        });
        
        backgroundMusicSldier = root.Q<Slider>("MusicVolumeSlider");
        backgroundMusicSldier.RegisterCallback<ChangeEvent<float>>(evt =>
        {
            audioMixer.SetFloat("Music", Mathf.Log10(evt.newValue)*20);
        });
        
        villagerSlider = root.Q<Slider>("VillagerVolumeSlider");
        villagerSlider.RegisterCallback<ChangeEvent<float>>(evt =>
        {
            audioMixer.SetFloat("Villager", Mathf.Log10(evt.newValue)*20);
        });
        
        uiVolumeSlider = root.Q<Slider>("UIVolumeSlider");
        uiVolumeSlider.RegisterCallback<ChangeEvent<float>>(evt =>
        {
            audioMixer.SetFloat("UI", Mathf.Log10(evt.newValue)*20);
        });
    }
}
