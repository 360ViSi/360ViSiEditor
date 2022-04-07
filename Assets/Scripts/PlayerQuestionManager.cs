using System;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// Singleton that manages the question panel in the player
/// might want to refactor to non-singleton, buuuuuuuut do I, seems to be a sensible solution
/// -> would require the Tool class to be refactored probably, or atleast reference some GameEvent's in Tool
///</summary>
public class PlayerQuestionManager : MonoBehaviour
{
    public static PlayerQuestionManager instance;
    [SerializeField] SimulationManager simulationManager;
    [SerializeField] GameObject panel;
    [SerializeField] GameObject submitButton;
    [SerializeField] GameObject answerButtonPrefab;
    [SerializeField] Transform buttonLayoutTransform;
    [SerializeField] TMPro.TMP_Text questionTitleText;
    [SerializeField] TMPro.TMP_Text questionText;
    Tool currentTool;
    List<int> answers = new List<int>();
    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
    }

    public void OpenQuestionPanel(Tool tool)
    {
        Time.timeScale = 0;
        //Setup
        simulationManager.SetVideoPauseState(true);

        panel.SetActive(true);
        currentTool = tool;
        submitButton.SetActive(currentTool.question.multichoice);
        //questionTitleText.text = currentTool.question.questionTitleText;
        questionText.text = currentTool.question.questionText;
        answers.Clear();

        //Destroy old answers
        for (int i = 0; i < buttonLayoutTransform.childCount; i++)
            Destroy(buttonLayoutTransform.GetChild(i).gameObject);

        //create question and answers
        for (int i = 0; i < currentTool.question.answers.Count; i++)
        {
            var go = Instantiate(answerButtonPrefab, buttonLayoutTransform);
            go.GetComponent<PlayerQuestionAnswerButton>().Setup(currentTool.question.answers[i], i);
        }
    }

    public void SelectAnswer(int answerId)
    {
        if (currentTool.question.multichoice == false)
        {
            answers = new List<int> { answerId };
            CheckAnswers(answerId);
            return;
        }

        if (answers.Contains(answerId))
            answers.Remove(answerId);
        else answers.Add(answerId);
    }

    ///<summary>
    /// Check that all the question.correctAnswers exist also in the answers that the user selected
    ///</summary>
    public void CheckAnswers(int answerId)
    {
        if (!currentTool.question.multichoice)
        {
            SubmitAnswer(answerId);
            return;
        }

        for (int i = 0; i < currentTool.question.correctAnswers.Count; i++)
        {
            if (currentTool.question.correctAnswers[i] == 0 && !answers.Contains(i))
            {
                SubmitAnswer(false);
                return;
            }
            else if (currentTool.question.correctAnswers[i] == 1 && answers.Contains(i))
            {
                SubmitAnswer(false);
                return;
            }
        }

        SubmitAnswer(true);
    }

    private void SubmitAnswer(bool isCorrect)
    {
        panel.SetActive(false);
        simulationManager.SetVideoPauseState(false);

        if (isCorrect)
        {
            foreach (var score in currentTool.question.answerScores)
            {
                simulationManager.Score += score;
            }
            simulationManager.GoToNode(currentTool.nextNodes[0]);
        }
            
        else
            simulationManager.GoToNode(currentTool.nextNodes[1]);
    }

    void SubmitAnswer(int answerId)
    {
        Time.timeScale = 1;
        panel.SetActive(false);
        simulationManager.SetVideoPauseState(false);
        if (answerId == 0)
        {
            foreach (var score in currentTool.question.answerScores)
            {
                simulationManager.Score += score;
            }
        }
        simulationManager.GoToNode(currentTool.nextNodes[currentTool.question.correctAnswers[answerId]]);
    }
}