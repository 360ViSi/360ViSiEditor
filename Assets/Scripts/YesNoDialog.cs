using System;
using UnityEngine;
using TMPro;

public class YesNoDialog : MonoBehaviour
{

    Action<bool> dialogEvent;
    [SerializeField] TMP_Text questionText;    
    [SerializeField] GameObject dialogPanel;    

    internal void Initialize(Action<bool> dialogEvent, string text)
    {
        this.dialogEvent = dialogEvent;
        questionText.text = text;
        dialogPanel.SetActive(true);
    }

    public void Answer(bool value)
    {
        if(value) 
            dialogEvent?.Invoke(value);
        dialogPanel.SetActive(false);
    }
}