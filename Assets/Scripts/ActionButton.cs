using UnityEngine;
using UnityEngine.UI;
public class ActionButton : MonoBehaviour
{
    int nextVideoID;
    [SerializeField] SimulationManager simulationManager;
    [SerializeField] Text buttonText;
    [SerializeField] Button button;

    public void SetAction(string text, int nextVideoID)
    {
        buttonText.text = text;
        this.nextVideoID = nextVideoID;
    }

    public void Activate()
    {
        simulationManager.goToVideo(nextVideoID);
    }
}