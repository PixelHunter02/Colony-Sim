using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BuildStats : MonoBehaviour
{
    public StoredItemSO craftingRecipe;
    private GameManager _gameManager;
    public GameObject building;
    public GameObject built;
    public Transform[] buildPoints;

    private void Awake()
    {
        _gameManager = GameManager.Instance;
    }
}
