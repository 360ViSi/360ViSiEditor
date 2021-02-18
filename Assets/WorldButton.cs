using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WorldButton : MonoBehaviour
{
    int nextVideoID;
    SimulationManager simulationManager;
    Image image;
    float startTime;
    float endTime;
    bool mouseOver;
    GameObject tooltipPanel;
    TMP_Text tmpText;

    public bool MouseOver { get => mouseOver; set => mouseOver = value; }

    private void Start()
    {
        GetComponent<Canvas>().worldCamera = Camera.main;
        image = GetComponentInChildren<Image>();
        var childImage = transform.GetChild(0).gameObject.AddComponent<WorldButtonImage>();
        childImage.Button = this;
    }

    public void SetAction(Action action, SimulationManager simulationManager)
    {
        this.nextVideoID = action.getNextVideo();
        this.startTime = action.getStartTime();
        this.endTime = action.getEndTime();
        this.simulationManager = simulationManager;

        tooltipPanel = transform.GetChild(1).gameObject;
        tmpText = GetComponentInChildren<TMP_Text>();
        tmpText.text = action.getActionText();
        
        transform.position = action.getWorldPosition();
        transform.LookAt(simulationManager.transform);
        //world canvas size quirk - might remove flipping later
        //transform.localScale = new Vector3(-.01f, .01f, 1);
    }
    private void Update()
    {
        if (mouseOver)
        {
            image.color = Color.green;
            tooltipPanel.SetActive(true);
        }
        else
        {
            image.color = Color.white;
            tooltipPanel.SetActive(false);
        }
    }

    public void Activate() => simulationManager.goToVideo(nextVideoID);
    public void SetActive(double currentTime) => gameObject.SetActive(currentTime > startTime && currentTime <= endTime);
}
