using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BuildStats : MonoBehaviour
{
    public CraftableSO craftingRecipe;
    private GameManager _gameManager;
    public GameObject building;
    public GameObject built;

    private void Awake()
    {
        _gameManager = GameManager.Instance;
    }
}
