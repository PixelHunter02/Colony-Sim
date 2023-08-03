using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject villager;

    public int numberToSpawn;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < numberToSpawn; i++)
        {
            Instantiate(villager);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
