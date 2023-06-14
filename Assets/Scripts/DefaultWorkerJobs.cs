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

    public void ChopTrees(HarvestObjectManager harvestObjectManager)
    {
        StartCoroutine(TaskTimer(5,harvestObjectManager));
        taskImage.sprite = harvestObjectManager.harvestableObject.taskSprite;
    }

    public void Forage()
    {
        
    }

    public void PickUpObject()
    {
        
    }

    private IEnumerator TaskTimer(float timeToCount, HarvestObjectManager harvestObjectManager)
    {
        canvas.SetActive(true);
        
        var timer = 0f;
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
