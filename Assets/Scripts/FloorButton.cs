using UnityEngine;

public class FloorButton : MonoBehaviour
{
    int nextVideoID;
    SimulationManager simulationManager;
    MeshRenderer meshRenderer;
    float startTime;
    float endTime;
    bool mouseOver;
    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void SetAction(Action action, SimulationManager simulationManager)
    {
        this.nextVideoID = action.getNextNode();
        this.startTime = action.getStartTime();
        this.endTime = action.getEndTime();
        this.simulationManager = simulationManager;

        transform.position = action.getWorldPosition();
        var oldRotation = transform.rotation;
        transform.LookAt(transform);
        transform.localEulerAngles = new Vector3(oldRotation.x, 0, oldRotation.z);
        transform.localScale = new Vector3(10, .5f, 10);
    }
    private void Update()
    {
        if (mouseOver)
            meshRenderer.material.SetColor("_Color", Color.green);
        else
            meshRenderer.material.SetColor("_Color", Color.white);
    }

    public void Activate() => simulationManager.GoToNode(nextVideoID);
    public void SetActive(double currentTime) => gameObject.SetActive(currentTime > startTime && currentTime <= endTime);
    private void OnMouseDown() => Activate();
    private void OnMouseEnter() => mouseOver = true;
    private void OnMouseExit() => mouseOver = false;
}

