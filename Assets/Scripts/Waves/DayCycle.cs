using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayCycle : MonoBehaviour
{
    [SerializeField] private float daySpeed;
    [SerializeField] private float nightSpeed;
    [SerializeField] private float speed;
    [SerializeField] private float currentTime;
    [SerializeField] private int dayCount;
    [SerializeField] GameObject monsterWavesManager;

    private void Awake()
    {
        transform.localEulerAngles = Vector3.zero;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DayTime(daySpeed));
    }

    // Update is called once per frame
    //void Update()
    //{
    //    currentTime += 1 * Time.deltaTime;
    //    transform.Rotate(new Vector3(1, 0, 0) * speed * Time.deltaTime);
    //    //transform.r = new Vector3(transform.rotation.x + Time.deltaTime * speed, 0, 0);

    //    // print(transform.localEulerAngles);
    //    if (gameObject.transform.localEulerAngles.x > 180 || gameObject.transform.localEulerAngles.x < 0)
    //    {
    //        print("goodbye light");
    //        gameObject.transform.GetComponent<Light>().enabled = false; 
    //    }
    //    else
    //    {
    //        gameObject.transform.GetComponent<Light>().enabled = true;
    //    }
    //    //https://www.youtube.com/watch?v=VZqqrNShOg0&ab_channel=JTAGames
    //}

    private IEnumerator DayTime(float timeTicks)
    {
        yield return new WaitForSeconds(timeTicks);
        currentTime++;
        //Debug.Log(currentTime);
        // print("DayTime");
        transform.Rotate(new Vector3(timeTicks/4, 0, 0));
        // Debug.Log(gameObject.transform.localEulerAngles.x);
        if (gameObject.transform.localEulerAngles.x > 185)
        {
            gameObject.transform.GetComponent<Light>().enabled = false;
            dayCount++;
            monsterWavesManager.GetComponent<MonsterWaves>().SpawnWave(dayCount);
            StartCoroutine(NightTime(nightSpeed));
        }
        else
        {
            StartCoroutine(DayTime(daySpeed));
        }
    }

    private IEnumerator NightTime(float timeTicks)
    {
        yield return new WaitForSeconds(timeTicks);
        currentTime++;
        //Debug.Log(currentTime);
        // print("NightTime");
        transform.Rotate(new Vector3(timeTicks * 5, 0, 0));

        if (gameObject.transform.localEulerAngles.x < 185)
        {
            gameObject.transform.GetComponent<Light>().enabled = true;
            StartCoroutine(DayTime(daySpeed));
        }
        else
        {
            StartCoroutine(NightTime(nightSpeed));
        }
    }

        //private IEnumerator DayTime(float lengthInSeconds)
        //{

        //    transform.Rotate(new Vector3(10, 0, 0) * speed * Time.deltaTime);
        //    print("time passed");
        //}
}
