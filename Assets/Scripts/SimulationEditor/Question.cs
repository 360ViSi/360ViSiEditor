using System.Globalization;
using System.Collections.Generic;

[System.Serializable]
public class Question
{
    public Question(string questionTitleText, string questionText,bool multichoice, List<string> answers, List<int> correctAnswers, List<int> answerScores, string spritePath)
    {
        this.questionTitleText = questionTitleText;
        this.questionText = questionText;
        this.multichoice = multichoice;
        this.answers = answers;
        this.correctAnswers = correctAnswers;
        this.answerScores = answerScores;
        this.spritePath = spritePath;
    }

    //Fields are public to avoid creating json wrapper for the class
    public string questionTitleText;
    public string questionText;
    public bool multichoice;
    public List<string> answers = new List<string>();
    public List<int> correctAnswers = new List<int>();
    public List<int> answerScores = new List<int>();
    public string spritePath;
}