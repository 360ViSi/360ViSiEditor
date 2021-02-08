﻿using System;
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
    private List<ActionButton> screenButtons = new List<ActionButton>();
    private List<WorldButton> worldButtons = new List<WorldButton>();

    [SerializeField]
    private SimulationManager simulationManager;
    [SerializeField] VideoPlayer videoPlayer;
    [SerializeField] GameObject worldButtonPrefab;

    private void Update()
    {
        var currentTime = videoPlayer.time / videoPlayer.length;
        if (double.IsNaN(currentTime))
            currentTime = 1;

        foreach (var item in screenButtons)
            item.SetActive(currentTime);

        foreach (var item in worldButtons)
            item.SetActive(currentTime);
    }
    public void SetupActions()
    {
        foreach (var item in worldButtons)
            Destroy(item.gameObject);
            
        worldButtons.Clear();
        var currentVideoPart = simulationManager.getCurrentVideoPart();

        var buttonActions = currentVideoPart.actions
          .Where(e => e.autoEnd == false
                      && e.actionType == ActionType.ScreenButton).ToArray();

        var worldActions = currentVideoPart.actions
          .Where(e => e.autoEnd == false
                      && e.actionType == ActionType.WorldButton).ToArray();

        for (int i = 0; i < screenButtons.Count; i++)
        {
            if (i < buttonActions.Length)
            {
                screenButtons[i].gameObject.SetActive(true);
                screenButtons[i].SetAction(buttonActions[i]);
                continue;
            }
            screenButtons[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < worldActions.Length; i++)
        {
            var go = Instantiate(worldButtonPrefab);
            var worldButton = go.GetComponent<WorldButton>();
            worldButton.SetAction(worldActions[i], simulationManager);
            worldButtons.Add(worldButton);
        }
    }
}