using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        //print(other);
        //print(GetComponentInParent<MonsterAI>().target);
        if (other.gameObject == GetComponentInParent<MonsterAI>().target.gameObject)
        {
            print("inRange");
            GetComponentInParent<MonsterAI>().inRange = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other == GetComponentInParent<MonsterAI>().target)
        {
            print("outRange");
            GetComponentInParent<MonsterAI>().inRange = false;
        }
    }
}
