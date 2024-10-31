using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManger : MonoBehaviour
{
    public string quiz, forest,mainmuenuscene,Learning;
    // Start is called before the first frame update
    public void quizscen()
    {
        SceneManager.LoadScene(quiz);
    }
    public void learningscene()
    {
        SceneManager.LoadScene(Learning);
    }
    public void forestscene()
    {
        SceneManager.LoadScene(forest);
    }
    public void MainMenuScen()
    {
        SceneManager.LoadScene(mainmuenuscene);
    }
}
