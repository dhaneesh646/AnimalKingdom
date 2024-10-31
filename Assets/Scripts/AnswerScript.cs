using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnswerScript : MonoBehaviour
{
    // Start is called before the first frame update
    public bool isCorrect = false;
    public QuizMAnager QuizMAnager;
    public GameObject CorrectAnswer;
    public GameObject WrongAnswer;
    public int score;

    private void Start()
    {
        CorrectAnswer.SetActive(false);
        WrongAnswer.SetActive(false);
    }
    public void Answer()
    {
        if(isCorrect)
        {
            Correctanswer();
            Debug.Log("Correct Answer");
            QuizMAnager.correct();

        }
        else
        {
            Wronganswer();
            Debug.Log("Wrong Answer");
            QuizMAnager.wrong();
        }
    }
    public void Correctanswer()
    {
        CorrectAnswer.SetActive(true);
        Invoke("TurnOff", 1);
    }
    public void Wronganswer()
    {
        WrongAnswer.SetActive(true);
        Invoke("TurnOff", 1);
    }
    private void TurnOff()
    {
        CorrectAnswer.SetActive(false);
        WrongAnswer.SetActive(false);
    }
}
