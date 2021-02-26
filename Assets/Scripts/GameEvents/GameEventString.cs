using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GameEvents/String", fileName = "GameEventString")]

public class GameEventString : ScriptableObject
{
    private List<GameEventListenerString> listeners = new List<GameEventListenerString>();

    public void Raise(string value)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
            listeners[i].OnEventRaised(value);
    }

    public void RegisterListener(GameEventListenerString listener) => listeners.Add(listener);

    public void UnRegisterListener(GameEventListenerString listener) => listeners.Remove(listener);
}