using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayCycle : MonoBehaviour
{
    [SerializeField] private float dayInterval;
    [SerializeField] private float nightInterval;
    [SerializeField] private float speed;
    [SerializeField] private int dayCount;
    [SerializeField] GameObject monsterWavesManager;

    private void Awake()
    {
        transform.localEulerAngles = Vector3.zero;
        monsterWavesManager = GameManager.Instance.monsterWaves.gameObject;
    }

    void Start()
    {
        StartCoroutine(DayTime(dayInterval));
        monsterWavesManager.GetComponent<MonsterWaves>().SpawnDayMonsters(3);
    }

    private IEnumerator DayTime(float timeTicks)
    {
        yield return new WaitForSeconds(timeTicks);
        transform.Rotate(new Vector3((timeTicks/4) * speed, 0, 0));

        if (gameObject.transform.localEulerAngles.x > 185)
        {
            gameObject.transform.GetComponent<Light>().enabled = false;
            dayCount++;
            monsterWavesManager.GetComponent<MonsterWaves>().SpawnWave(dayCount);
            GameEvents.current.NightTimeStart();
            StartCoroutine(NightTime(nightInterval));

        }
        else
        {
            GameEvents.current.NightTimeEnd();
            StartCoroutine(DayTime(dayInterval));
        }
    }

    private IEnumerator NightTime(float timeTicks)
    {
        yield return new WaitForSeconds(timeTicks);
        transform.Rotate(new Vector3((timeTicks * 2) * speed, 0, 0));

        if (gameObject.transform.localEulerAngles.x < 185)
        {
            gameObject.transform.GetComponent<Light>().enabled = true;
            StartCoroutine(DayTime(dayInterval));
        }
        else
        {
            StartCoroutine(NightTime(nightInterval));
        }
    }
}
