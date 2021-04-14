using UnityEngine;

public class PlayerTimerAction
{
    SimulationManager simulationManager;
    float targetTime;
    int nextVideoID;

    public PlayerTimerAction(Action action, SimulationManager simulationManager)
    {
        this.simulationManager = simulationManager;
        targetTime = Time.time + action.getTimer();
        nextVideoID = action.getNextNode();
    }

    public bool CheckActionTime()
    {
        if (Time.time > targetTime){
            simulationManager.GoToNode(nextVideoID);
            return true;
        }
        return false;
    }
}