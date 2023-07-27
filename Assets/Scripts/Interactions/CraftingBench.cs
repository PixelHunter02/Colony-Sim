using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingBench : MonoBehaviour
{
    private GameManager _gameManager;
    private void Awake()
    {
        _gameManager = GameManager.Instance;
    }

    public void OnInteraction()
    {
        ShowCraftingMenu();
    }

    private void ShowCraftingMenu()
    {
        _gameManager.level.GameState = GameState.Crafting;
    }
}
