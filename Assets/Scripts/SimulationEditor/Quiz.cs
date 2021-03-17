using System.Collections.Generic;

class Quiz
{
   List<Question> questions = new List<Question>();

   public void CreateQuestion()
   {

   }
}

class Question
{
    bool multipleChoice;
    string questionText;
    List<string> answers = new List<string>();   
    public bool MultipleChoice { get => multipleChoice; set => multipleChoice = value; }
    public string QuestionText { get => questionText; set => questionText = value; }
    public List<string> Answers { get => answers; set => answers = value; }
}