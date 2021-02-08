using UnityEngine;
using UnityEngine.UI;
public class ActionButton : MonoBehaviour
{
    int nextVideoID;
    [SerializeField] SimulationManager simulationManager;
    [SerializeField] Text buttonText;
    [SerializeField] Button button;
    float startTime;
    float endTime;

    public void SetAction(Action action)
    {
        buttonText.text = action.getActionText();
        this.nextVideoID = action.getNextVideo();
        this.startTime = action.getStartTime();
        this.endTime = action.getEndTime();
    }

    public void Activate() => simulationManager.goToVideo(nextVideoID);

    public void SetActive(double currentTime)
    {
        //S LATER - button.enabled to hide button when it's not usable
        button.interactable = currentTime > startTime && currentTime <= endTime;
    }
}