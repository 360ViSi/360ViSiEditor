using System.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System;

public class QuizCreator_OLD : MonoBehaviour
{
    [SerializeField] Transform layoutTransform;
    [SerializeField] GameObject answerPrefab;
    [SerializeField] TMP_InputField questionTitleInput;
    [SerializeField] TMP_InputField questionInput;
    List<TMP_InputField> answerInputs = new List<TMP_InputField>();
    List<TMP_InputField> outPortInputs = new List<TMP_InputField>();
    List<TMP_InputField> scoreInputs = new List<TMP_InputField>();
    List<Toggle> correctToggles = new List<Toggle>();
    [SerializeField] Toggle multichoiceToggle;
    [SerializeField] ImageSetter imageSetter;
    [SerializeField] GameObject imagePanel;

    // Start is called before the first frame update
    void OnEnable()
    {
        questionTitleInput.text = "";
        questionInput.text = "";
        for (int i = 0; i < layoutTransform.childCount; i++)
        {
            Destroy(layoutTransform.GetChild(i).gameObject);
        }

        answerInputs.Clear();
        correctToggles.Clear();
        outPortInputs.Clear();
        scoreInputs.Clear();

        LoadQuestionFromNodeToCreator();
    }


    private void Update()
    {
        //THIS SUCKS BUT THE EVENT WONT TRIGGER IN EDITOR ??
#if UNITY_EDITOR
        HandleMultichoiceToggle(multichoiceToggle.isOn);
#endif
    }

    public void LoadQuestionFromNodeToCreator()
    {
        var question = NodeInspector.instance.CurrentToolNode.Question; // Question is a tool node, should have a spritepath in json

        if (question == null) question = new Question("", "", false, new List<string>() { }, new List<int>(), new List<int>(), "");

        multichoiceToggle.isOn = question.multichoice;
        questionTitleInput.text = question.questionTitleText;
        questionInput.text = question.questionText;
        
        if(!String.IsNullOrEmpty(question.spritePath))
        {
            imagePanel.SetActive(true);
            imageSetter.SetOldLoadedSprite(question.spritePath);
        }
        
        for (int i = 0; i < question.answers.Count; i++)
        {
            AddAnswer();

            answerInputs[i].text = question.answers[i];

            if (question.answerScores != null)
            {
                scoreInputs[i].text = question.answerScores[i].ToString();
            }
            else
            {
                scoreInputs[i].text = "0";
            }

            if (question.multichoice)
            {

                if (question.correctAnswers[i] == 0){
                    correctToggles[i].isOn = true;
                    outPortInputs[i].text = "0";
                }else{
                    correctToggles[i].isOn = false;
                    outPortInputs[i].text = "1";
                }
            }
            else
            {
                outPortInputs[i].text = question.correctAnswers[i].ToString();
                correctToggles[i].isOn = false;
            }
        }
        HandleMultichoiceToggle(question.multichoice);
    }

    public void AddAnswer()
    {
        var go = Instantiate(answerPrefab, layoutTransform);

        var inputFields = go.GetComponentsInChildren<TMP_InputField>();
        answerInputs.Add(inputFields[0]);
        inputFields[1].text = "0";
        outPortInputs.Add(inputFields[1]);
        inputFields[2].text = "0";
        scoreInputs.Add(inputFields[2]);


        var toggle = go.GetComponentInChildren<Toggle>();
        correctToggles.Add(toggle);
    }

    public void RemoveLastAnswer()
    {
        var answer = answerInputs[answerInputs.Count - 1];
        answerInputs.Remove(answer);
        outPortInputs.Remove(outPortInputs[outPortInputs.Count - 1]);
        scoreInputs.Remove(scoreInputs[scoreInputs.Count - 1]);
        correctToggles.Remove(correctToggles[correctToggles.Count - 1]);
        Destroy(answer.transform.parent.gameObject);
    }

    public void SaveQuestion()
    {
        var answers = answerInputs.Select(e => e.text).ToList();
        var correctAnswers = outPortInputs.Select(e => int.Parse(e.text)).ToList();
        var answerScores = scoreInputs.Select(e => int.Parse(e.text)).ToList();
        var spritePath = imageSetter.SpritePath;
        var question = new Question(questionTitleInput.text, questionInput.text, multichoiceToggle.isOn, answers, correctAnswers, answerScores, spritePath);
        NodeInspector.instance.CurrentToolNode.Question = question;
        gameObject.SetActive(false);
        UndoRedoHandler.instance.SaveState();
    }

    public void HandleMultichoiceToggle(bool isMulti)
    {
        NodeInspector.instance.CurrentToolNode.SetOutPortAmountButtonsActive(!isMulti);
        
        if (isMulti)
        {
            for (int i = 0; i < outPortInputs.Count; i++)
            {
                outPortInputs[i].text = correctToggles[i].isOn ? "0" : "1";
                outPortInputs[i].interactable = false;
                correctToggles[i].interactable = true;
            }
            return;
        }

        NodeInspector.instance.CurrentToolNode.RemoveAllExcessOutPorts(2);

        for (int i = 0; i < outPortInputs.Count; i++)
        {
            correctToggles[i].interactable = false;
            outPortInputs[i].interactable = true;
        }
    }
}
