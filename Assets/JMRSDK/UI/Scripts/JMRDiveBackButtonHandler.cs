using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JMRSDK.InputModule;
using JMRSDK;


public class JMRDiveBackButtonHandler : MonoBehaviour
{

    [SerializeField]
    private Texture backButtonTexture;  


    void OnGUI()
    {
        if (Application.platform == RuntimePlatform.Android && JMRRigManager.Instance.getDeviceID()==(int)JMRRigManager.DeviceType.JioCardboard)
        {

            if (GUI.Button(new Rect(10, 10, 100, 100), backButtonTexture))
            {
                JMRSystemActions.Instance.OnExitCardboardMode();
            }
        }
    }
}
