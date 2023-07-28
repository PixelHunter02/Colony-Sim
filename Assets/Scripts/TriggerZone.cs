using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerZone : MonoBehaviour
{
    public List<GameObject> objInTriggerZone;

    private void OnTriggerEnter(Collider other)
    {
        objInTriggerZone.Add(other.gameObject);
    }
    private void OnTriggerExit(Collider other)
    {
        objInTriggerZone.Remove(other.gameObject);
    }

    private void Update()
    {
        List<GameObject> temp = new List<GameObject>();
        foreach(GameObject obj in objInTriggerZone)
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
                objInTriggerZone.Remove(obj);
            }
        }
    }
}
