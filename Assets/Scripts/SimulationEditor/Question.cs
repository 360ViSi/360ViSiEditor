using System.Globalization;
using System.Collections.Generic;

[System.Serializable]
public class Question
{
    public Question(string questionText, List<string> answers, bool multipleChoice, List<int> correctAnswers)
    {
        this.questionText = questionText;
        this.answers = answers;
        this.correctAnswers = correctAnswers;
    }

    //Fields are public to avoid creating json wrapper for the class
    public string questionText;
    public List<string> answers = new List<string>();
    public List<int> correctAnswers = new List<int>();
}