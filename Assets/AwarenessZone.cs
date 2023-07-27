using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AwarenessZone : MonoBehaviour
{
    public List<GameObject> objInAwarenessZone;

    private void OnTriggerEnter(Collider other)
    {
        objInAwarenessZone.Add(other.gameObject);
    }
    private void OnTriggerExit(Collider other)
    {
        objInAwarenessZone.Remove(other.gameObject);
    }
    private void Update()
    {
        List<GameObject> temp = new List<GameObject>();
        foreach (GameObject obj in objInAwarenessZone)
        {
            if (!obj)
            {
                temp.Add(obj);
            }
        }

        foreach (GameObject obj in temp)
        {
            if (!obj)
            {
                objInAwarenessZone.Remove(obj);
            }
        }
    }
}
