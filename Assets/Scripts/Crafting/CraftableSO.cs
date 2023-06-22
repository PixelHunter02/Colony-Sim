using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Crafting Recipe")]
public class CraftableSO : ScriptableObject
{
    public Resource[] requiredResource;
    public bool instantlyEnterBuildMode;
    public GameObject prefab;
}
