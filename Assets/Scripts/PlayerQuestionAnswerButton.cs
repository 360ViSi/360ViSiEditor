using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerQuestionAnswerButton : MonoBehaviour
{
    int id;
    bool isSelected;

    public void Setup(string answer, int id)
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
        GetComponentInChildren<TMP_Text>().text = answer;
        this.id = id;
    }

    void OnClick()
    {
        isSelected = !isSelected;
        GetComponent<Image>().color = isSelected ? Color.green : Color.white;
        PlayerQuestionManager.instance.SelectAnswer(id);
    }
}
