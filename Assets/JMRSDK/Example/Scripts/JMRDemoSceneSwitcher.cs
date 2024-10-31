using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using JMRSDK;

public class JMRDemoSceneSwitcher : MonoBehaviour
{
    private void Start()
    {
        JMRRigManager.Instance.setHomePage = true;
    }
    public void OnClick(int index)
    {
        SceneManager.LoadScene(index);
    }
}
