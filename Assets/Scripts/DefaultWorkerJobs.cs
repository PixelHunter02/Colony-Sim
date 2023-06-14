using System;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class DefaultWorkerJobs : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Image taskImage;
    [SerializeField] private GameObject canvas;
    private Animator _animator;

    public void BeginHarvest(HarvestObjectManager harvestObjectManager)
    {
        StartCoroutine(TaskTimer(harvestObjectManager));
        taskImage.sprite = harvestObjectManager.harvestableObject.taskSprite;
    }


    private IEnumerator TaskTimer(HarvestObjectManager harvestObjectManager)
    {
        canvas.SetActive(true);
        
        var timer = 0f;
        var timeToCount = harvestObjectManager.harvestableObject.timeToHarvest;
        while (timer < timeToCount)
        {
            timer += Time.deltaTime;
            slider.value = timer / timeToCount;
            yield return null;
        }
        
        taskImage.sprite = harvestObjectManager.harvestableObject.taskCompleteSprite;
        StartCoroutine(harvestObjectManager.SpawnHarvestDrops());
        yield return new WaitForSeconds(3f);
        canvas.SetActive(false);
    }
}
