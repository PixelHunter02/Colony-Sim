using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects",fileName = "BuildingSO",order = 3)]
public class BuildingSO : ScriptableObject
{
    public CraftableSO craftingRecipe;
    public Sprite buttonSprite;
}
