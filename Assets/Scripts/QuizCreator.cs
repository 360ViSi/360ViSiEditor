using System.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class QuizCreator : MonoBehaviour
{
    [SerializeField] Transform layoutTransform;
    [SerializeField] GameObject answerPrefab;
    [SerializeField] TMP_InputField questionInput;
    [SerializeField] Toggle isMultiplechoiceToggle;
    List<TMP_InputField> answerInputs = new List<TMP_InputField>();
    List<Toggle> correctToggles = new List<Toggle>();
    
    // Start is called before the first frame update
    void OnEnable()
    {
        questionInput.text = "";
        for (int i = 0; i < layoutTransform.childCount; i++)
        {
            Destroy(layoutTransform.GetChild(i).gameObject);
        }

        answerInputs.Clear();
        correctToggles.Clear();

        LoadQuestionFromNodeToCreator();
    }

    public void LoadQuestionFromNodeToCreator()
    {
        var question = NodeInspector.instance.CurrentToolNode.Question;

        if (question == null) return;

        questionInput.text = question.questionText;
        for (int i = 0; i < question.answers.Count; i++)
        {
            AddAnswer();

            answerInputs[i].text = question.answers[i];

            if (question.correctAnswers.Contains(i))
                correctToggles[i].isOn = true;
        }
    }

    public void AddAnswer()
    {
        var go = Instantiate(answerPrefab, layoutTransform);
        answerInputs.Add(go.GetComponentInChildren<TMP_InputField>());
        correctToggles.Add(go.GetComponentInChildren<Toggle>());
    }

    public void RemoveLastAnswer()
    {
        var answer = answerInputs[answerInputs.Count - 1];
        answerInputs.Remove(answer);
        correctToggles.Remove(correctToggles[correctToggles.Count - 1]);
        Destroy(answer.transform.parent.gameObject);
    }

    public void CreateQuestion()
    {
        var answers = answerInputs.Select(e => e.text).ToList();
        var correctAnswers = new List<int>();

        for (int i = 0; i < correctToggles.Count; i++)
            if (correctToggles[i].isOn) correctAnswers.Add(i);

        var question = new Question(questionInput.text, answers, isMultiplechoiceToggle.isOn, correctAnswers);
        NodeInspector.instance.CurrentToolNode.Question = question;
        gameObject.SetActive(false);
    }
}
