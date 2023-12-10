using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class NewTutorial : MonoBehaviour
{
    private bool stageOneRun;
    private bool stageTwoRun;
    private bool stageThreeRun;
    private bool stageFourRun;
    private bool stageFiveRun;
    private bool stageSixRun;
    private bool stageSevenRun;
    private bool stageEightRun;
    private bool stageNineRun;
    
    public static event Action TutorialStageTwo;
    public static event Action TutorialStageThree;

    public GameObject mainCameraView;
    public GameObject trappedCameraView;
    public GameObject buildCameraView;
    
    private void OnEnable()
    {
        
        Level.tutorialStageOne += TutorialStage1;
        
        TutorialStageTwo += TutorialStage2;
        
        TutorialStageThree += TutorialStage3;
        
        Interactions.TutorialStageFour += TutorialStage4;
        
        UIToolkitManager.TutorialStageFive += TutorialStage5;
        
        CraftingManager.TutorialStageSix += TutorialStage6;

        UIToolkitManager.TutorialStageSeven += TutorialStage7;
        
        HarvestableObject.TutorialStageEight += TutorialStage8;
        
        BuildingManager.TutorialStageNine += TutorialStage9;
    }

    private void OnDisable()
    {
        Level.tutorialStageOne -= TutorialStage1;
        
        TutorialStageTwo -= TutorialStage2;

        TutorialStageThree -= TutorialStage3;
        
        Interactions.TutorialStageFour -= TutorialStage4;

        UIToolkitManager.TutorialStageFive -= TutorialStage5;
  
        CraftingManager.TutorialStageSix -= TutorialStage6;
        
        UIToolkitManager.TutorialStageSeven -= TutorialStage7;
        
        HarvestableObject.TutorialStageEight -= TutorialStage8;

        BuildingManager.TutorialStageNine -= TutorialStage9;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)||Input.GetKeyDown(KeyCode.W)||Input.GetKeyDown(KeyCode.S)||Input.GetKeyDown(KeyCode.D))
        {
            TutorialStageThree?.Invoke();
        }
    }

    private void TutorialStage1()
    {
        if (stageOneRun)
        {
            return;
        }

        stageOneRun = true;
        var villager = VillagerManager.GetVillagers()[0];
        var trappedVillager = VillagerManager.GetVillagers()[1];
        Level.AddToVillagerLog(villager, $"Quick! Village Leader! The bridge has broken and {trappedVillager.VillagerStats.VillagerName} is stuck across the water! They're going to need your help to get back home!");
        StartCoroutine(TutorialEnumerator());
    }
    
    private IEnumerator TutorialEnumerator()
    {
        yield return new WaitForSeconds(3f);
        mainCameraView.SetActive(false);
        buildCameraView.SetActive(false);
        trappedCameraView.SetActive(true);
        yield return new WaitForSeconds(3f);
        mainCameraView.SetActive(true);
        trappedCameraView.SetActive(false);
        yield return new WaitForSeconds(3f);
        TutorialStageTwo?.Invoke();
    }
    
    private void TutorialStage2()
    {
        if (stageTwoRun || !stageOneRun)
        {
            return;
        }
        
        stageTwoRun = true;
        var trappedVillager = VillagerManager.GetVillagers()[1];
        Level.AddToVillagerLog(trappedVillager, "Help! You can use the WASD keys to move the camera over to me! You will also need to rotate the camera by pressing the right mouse button and dragging.");
    }
    
    private void TutorialStage3()
    {
        if (stageThreeRun || !stageTwoRun)
        {
            return;
        }
        
        stageThreeRun = true;
        var trappedVillager = VillagerManager.GetVillagers()[1];
        Level.AddToVillagerLog(trappedVillager,"I saw some logs on the ground, but I'm going to need somewhere to put it, can you designate a stockpile nearby by selecting the mine cart and clicking and dragging on the ground?");
    }
    
    private void TutorialStage4()
    {
        if (stageFourRun || !stageThreeRun)
        {
            return;
        }
        
        stageFourRun = true;
        var trappedVillager = VillagerManager.GetVillagers()[1];
        Level.AddToVillagerLog(trappedVillager,"Thanks Leader! Ill get straight to work on collecting them. You can keep an eye on the resources we have in storage by clicking on the chest.");
    }
    
    private void TutorialStage5()
    {
        if (stageFiveRun || !stageFourRun)
        {
            return;
        }
        
        stageFiveRun = true;
        var trappedVillager = VillagerManager.GetVillagers()[1];
        Level.AddToVillagerLog(trappedVillager,"Sadly I dont think we have enough wood to fix the bridge, Im going to need you to help me craft an axe. Can you click on the Anvil Icon on the left of the toolbar and click the axe Icon? ");
    }
    
    private void TutorialStage6()
    {
        if (stageSixRun || !stageFiveRun)
        {
            return;
        }
        
        stageSixRun = true;
        var trappedVillager = VillagerManager.GetVillagers()[1];
        Level.AddToVillagerLog(trappedVillager,"Awesome! You're a natural at this! You can assign me the lumberjack role by clicking on the Scroll Icon and finding my Portrait. Then click the drop down and select lumberjack");
    }
    
    private void TutorialStage7()
    {
        if (stageSevenRun || !stageSixRun)
        {
            return;
        }
        
        stageSevenRun = true;
        var trappedVillager = VillagerManager.GetVillagers()[1];
        Level.AddToVillagerLog(trappedVillager,"Alright, next up im going to need to cut down some trees, you can get me to do this by clicking on the tree when it is highlighted.");
    }
    
    private void TutorialStage8()
    {
        if (stageEightRun || !stageSevenRun)
        {
            return;
        }
        
        stageEightRun = true;
        var trappedVillager = VillagerManager.GetVillagers()[1];
        Level.AddToVillagerLog(trappedVillager,"Whew, we are almost out of here! The last step we need to do is build the bridge, this can be done by selecting the hammer icon and selecting the bridge icon in the tool bar. Once you do that just place it where the old bridge was and Ill get to work!");
    }
    
    private void TutorialStage9()
    {
        if (stageNineRun || !stageEightRun)
        {
            return;
        }
        
        stageNineRun = true;
        var trappedVillager = VillagerManager.GetVillagers()[1];
        Level.AddToVillagerLog(trappedVillager,"Thanks so much for helping me get back to our village! As a token of my thanks I was hoping to show you a little secret, this colony was brought together by the village heart, completing tasks and ensuring the villagers are safe and happy powers it up and allows us to expand our colony, it appears to be ready to grow now. I would love for you to do the honours, click on the village heart and level it up.");
    }

}
