using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JMRSDK;
using JMRSDK.Toolkit.UI;
 public class JMRDemoLicenseValidation : MonoBehaviour
{

    [SerializeField]
    private Text text;

    public JMRUIPrimaryInputField packageName;

    private void OnEnable()
    {
        JMRManager.TRIGGER_LICENSE_VALIDATION_FAIL += Showfailure;
        JMRManager.TRIGGER_LICENSE_VALIDATION_CHECK += Showfailure;
    }

    public void CheckLicense()
    {

        if (!string.IsNullOrEmpty(packageName.Text))
        {
            JMRManager.Instance.checkLicenseValidity(packageName.Text);
        }
        else
        {
            text.text = "LOGS: " + " Invalid Package Name > ";
        }
    }

    public void Showfailure(int error, string appPackageName)
    {
        Debug.LogError("LAUNCHER OnFAILE>> :" + error + " APP NAME" + appPackageName);
        text.text = "LOGS: " + "LAUNCHER OnFAILED>> :" + error + " APP NAME" + appPackageName;
    }
    public void Showfailure(string appPackageName)
    {
        Debug.LogError("LAUNCHER >> :" +  " APP NAME" + appPackageName);
        text.text = "LOGS: " + "LAUNCHER onValidated>> :" + " APP NAME" + appPackageName;
    }


}
