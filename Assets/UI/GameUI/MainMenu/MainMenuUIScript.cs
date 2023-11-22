using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.UI;
using UnityEngine.UIElements;

public class MainMenuUIScript : MonoBehaviour
{
    UIDocument uiDocument;
    VisualElement root;

    Button playButton;
    Button settingsButton;
    Button exitButton;

    private void Awake()
    {
        uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;

        playButton = root.Q<Button>("PlayButton");
        settingsButton = root.Q<Button>("SettingsButton");
        exitButton = root.Q<Button>("ExitButton");

        playButton.RegisterCallback<ClickEvent>(OnPlayButtonPress);
        settingsButton.RegisterCallback<ClickEvent>(OnSettingsButtonPress);
        exitButton.RegisterCallback<ClickEvent>(OnExitButtonPress);
    }

    private void OnPlayButtonPress(ClickEvent evt)
    {
        print("start button pressed");
    }
    private void OnSettingsButtonPress(ClickEvent evt)
    {
        print("settings button pressed");
    }
    private void OnExitButtonPress(ClickEvent evt)
    {
        print("exit button pressed");
        Application.Quit();
    }
}
