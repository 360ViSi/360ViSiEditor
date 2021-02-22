using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WorldButton : MonoBehaviour
{
    int nextVideoID;
    SimulationManager simulationManager;
    [SerializeField] SO_Icons iconDb;
    [SerializeField] Image image;
    float startTime;
    float endTime;
    bool mouseOver;
    [SerializeField] GameObject tooltipPanel;
    [SerializeField] TMP_Text tmpText;

    public bool MouseOver { get => mouseOver; set => mouseOver = value; }

    private void Start()
    {
        GetComponent<Canvas>().worldCamera = Camera.main;
        image.gameObject.GetComponent<WorldButtonImage>().Button = this; 
    }

    public void SetAction(Action action, SimulationManager simulationManager)
    {
        this.nextVideoID = action.getNextVideo();
        this.startTime = action.getStartTime();
        this.endTime = action.getEndTime();
        this.simulationManager = simulationManager;

        tmpText.text = action.getActionText();
        image.sprite = iconDb.GetIconSprite(action.getIconName());
        transform.position = action.getWorldPosition();
        transform.LookAt(simulationManager.transform);
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