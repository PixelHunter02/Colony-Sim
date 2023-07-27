using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterWaves : MonoBehaviour
{
    [SerializeField] float time;
    [SerializeField] GameObject[] prefabs;

    [SerializeField] float minX;
    [SerializeField] float minZ;
    [SerializeField] float maxX;
    [SerializeField] float maxZ;

    public void SpawnWave(int day)
    {
        for (var i = 1; i < day * 2; i++)
        {
            //will need to be changed to avoid spawning near characters and base
            Instantiate(prefabs[0], new Vector3(Random.Range(minX,maxX), -2, Random.Range(minZ, maxZ)), Quaternion.identity);
        }
    }
    public void SpawnDayMonsters(int day)
    {
        Instantiate(prefabs[1], new Vector3(Random.Range(minX, maxX), -2, Random.Range(minZ, maxZ)), Quaternion.identity);

    }
}
