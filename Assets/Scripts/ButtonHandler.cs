using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class ButtonHandler : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private List<ActionButton> buttons;

    [SerializeField]
    private SimulationManager simulationManager;
    [SerializeField] VideoPlayer videoPlayer;

    // Update is called once per frame
    void Update()
    {
        var videoPart = simulationManager.getCurrentVideoPart();
        if (videoPart == null)
            return;
        SetupButtons(videoPart);
    }

    public void WhenClicked(int actionID)
    {
        simulationManager.actionSelected(actionID);
    }

    private void SetupButtons(VideoPart currentVideoPart)
    {
        //S NOTE: if the video ends time is divided by 0 so it will result in NaN 
        //-> is this an issue?
        var currentTime = videoPlayer.time / videoPlayer.length;
        if(double.IsNaN(currentTime)) 
            currentTime = 1;

        //S TODO set the times on video already, so no need to LINQ in update
        var arrayOfEnabledActions = currentVideoPart.actions
          .Where(e => e.startTime < currentTime
                      && e.endTime >= currentTime
                      && e.autoEnd == false).ToArray();

        for (int i = 0; i < buttons.Count; i++)
        {
            if (i < arrayOfEnabledActions.Length)
            {
                buttons[i].gameObject.SetActive(true);
                buttons[i].SetAction(arrayOfEnabledActions[i].getActionText(),
                                     arrayOfEnabledActions[i].getNextVideo());
                continue;
            }
            buttons[i].gameObject.SetActive(false);
        }
    }
}