using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "GameEvents/Listeners/String", fileName = "GameEventListenerString")]
public class GameEventListenerString : MonoBehaviour
{
    public GameEventString Event;
    public UnityEventString Response;

    private void OnEnable() => Event.RegisterListener(this);

    private void OnDisable() => Event.UnRegisterListener(this);

    public void OnEventRaised(string value) => Response?.Invoke(value);
}
[System.Serializable]
public class UnityEventString : UnityEvent<string>{}
