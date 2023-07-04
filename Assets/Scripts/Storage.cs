using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vector3 = System.Numerics.Vector3;

public class Storage : MonoBehaviour
{
    private static HashSet<Vector3Int> storageLocation;
    private static HashSet<Vector3Int> usedSpaces;
    

    // Assign Storage Location
    private void AssignStorage(StoredItemSO itemToStore, Vector3Int location)
    {
        storageLocation.Remove(location);
        usedSpaces.Add(location);
    }
    
    // Add To Storage
    
    // Remove From Storage
}
 