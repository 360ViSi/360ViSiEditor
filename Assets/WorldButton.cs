using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class WorldButton : MonoBehaviour
{
    int nextVideoID;
    SimulationManager simulationManager;
    Image image;
    float startTime;
    float endTime;
    bool mouseOver;

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

        transform.position = action.getWorldPosition();
        transform.LookAt(transform);
        transform.localScale = new Vector3(-.01f, .01f, 1);
    }
    private void Update()
    {
        if (mouseOver)
            image.color = Color.green;
        else
            image.color = Color.red;
    }

    public void Activate() => simulationManager.goToVideo(nextVideoID);
    public void SetActive(double currentTime) => gameObject.SetActive(currentTime > startTime && currentTime <= endTime);
}
