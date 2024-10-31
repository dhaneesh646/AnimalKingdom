using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class QuizMAnager : MonoBehaviour
{
    public List<QuestionandAnswers> QnA;
    public GameObject[] options;
    public int currentQuestion;
    private bool isgameover = false;
    public int Score;
    public GameObject QuizePannel,scorepanel;
    public AnswerScript AnswerScript;
    int totalquestionsn =0;
    public string sceneNameToLoad;
    public string MainMuneu;
    [SerializeField] private GameObject dockmanager;

    public TMP_Text QuestionTxt,ScoreText;

    private void Start()
    {
        totalquestionsn = QnA.Count;
        scorepanel.SetActive(false);
        Generatequestion();
    }
    public void correct()
    {
        Score += 1;
        QnA.RemoveAt(currentQuestion);
        Generatequestion();
    }
    public void wrong()
    {
        QnA.RemoveAt(currentQuestion);
        Generatequestion();
    }
    private void gameover()
    {
        scorepanel.SetActive(true);
        QuizePannel.SetActive(false);
        ScoreText.text = string.Format("Your score is " + Score + "/" + totalquestionsn);
    }

    public void Exit()
    {
        SceneManager.LoadScene(MainMuneu);
    }
    private void setanswers()
    {
        
        for (int i = 0;i < options.Length; i++)
        {
            options[i].GetComponent<AnswerScript>().isCorrect = false;
            options[i].transform.GetChild(0).GetComponent<TMP_Text>().text = QnA[currentQuestion].Answers[i];
            if(QnA[currentQuestion].CorrectAnswer == i+1)
            {
                options[i].GetComponent<AnswerScript>().isCorrect = true;
            }
        }
       
    }
    private void Generatequestion()
    {
        if(QnA.Count >0)
        {
            currentQuestion = Random.Range(0, QnA.Count);

            QuestionTxt.text = QnA[currentQuestion].Questions;
            setanswers();
        }
        else
        {
            gameover();
            Debug.Log("game over");
        }
       

     //   QnA.RemoveAt(currentQuestion);
    }
    
    public void retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void mainmenu()
    {
        SceneManager.LoadScene(sceneNameToLoad);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            dockmanager.SetActive(true);
        }
    }
}