using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class HarvestObjectManager : MonoBehaviour
{
    public HarvestableObjectSO harvestableObject;
    public Worker assignedWorker;

    public IEnumerator CRSpawnHarvestDrops()
    {
        const float pushIntensity = 0.5f;
        const float timeBetweenSpawns = 0.3f;
        var min = harvestableObject.minDropAmount;
        var max = harvestableObject.maxDropAmount;

        var numberToSpawn = Random.Range(min, max);
        for (var i = 0; i <= numberToSpawn; i++)
        {
            var drop = Instantiate(harvestableObject.prefabToSpawn, transform.position, Quaternion.identity);
            var x = Random.Range(-180, 180);
            var z = Random.Range(-180, 180);
            var pushDirection = new Vector3(x, 50, z);
            drop.TryGetComponent(out Rigidbody rb);
            rb.AddForce(pushDirection*pushIntensity);
            yield return new WaitForSeconds(timeBetweenSpawns);
        }
    }
}
