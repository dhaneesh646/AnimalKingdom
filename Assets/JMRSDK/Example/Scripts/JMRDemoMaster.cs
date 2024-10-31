using UnityEngine;
using JMRSDK;
using System.Collections;
using UnityEngine.UI;

public class JMRDemoMaster : MonoBehaviour
{
    [SerializeField]
    private JMRBuildAPI buildApi;
    [SerializeField]
    private Text logText;
    bool isGetVersionNum = false;
    // Start is called before the first frame update
    void Start()
    {
        if (!Application.isEditor)
        {
            buildApi = JMRManager.Instance.GetBuildAPI();
            ShowBuildInformations();
        }
    }
    void Update()
    {
        if (!Application.isEditor)
        {
            //if (!string.IsNullOrEmpty(JMRUpdateManager.Instance.GetFirmwareVersion("cx3")) && !isGetVersionNum)
            //{
            //    string logString = logText.text;
            //    logText.text = logString + "Firmware Version:\t" + JMRUpdateManager.Instance.GetFirmwareVersion("cx3");
            //    isGetVersionNum = true;
            //}
        }
        else
        {
            Debug.LogWarning("this feature can not work in editor");
        }
    }
    // Update is called once per frame
    void ShowBuildInformations()
    {
        if(Application.isEditor || buildApi == null)
        {

            logText.text = " JioMixedReality Platform Information \n "
                 + "MinSdkApiLevel:\t" + " Not available in editor" + "\n"
                 + "SdkCodeName:\t" + " Not available in editor" + "\n"
                 + "MinServiceApiLevel:\t" + " Not available in editor" + "\n"
                 + "SDKApiLevel:\t" + " Not available in editor" + "\n"
                 + "SdkVersion:\t" + " Not available in editor" + "\n"
                 + "ServiceApiLevel:\t" + " Not available in editor" + "\n"
                 + "ServiceCodeName:\t" + " Not available in editor" + "\n"
                 + "ServiceVersion:\t" + " Not available in editor" + "\n"
                 ;

            return;
        }

        logText.text = "JioMixedReality Platform Information \n "
                 + "MinSdkApiLevel:\t" + buildApi.minSdkApiLevel() + "\n"
                 + "SDKApiLevel:\t" + buildApi.sdkApiLevel() + "\n"
                 + "MinServiceApiLevel:\t" + buildApi.minServiceApiLevel() + "\n"
                 + "ServiceApiLevel:\t" + buildApi.serviceApiLevel() + "\n"
                 + "SdkCodeName:\t" + buildApi.sdkCodeName() + "\n"
                 + "SdkVersion:\t" + buildApi.sdkVersion() + "\n"
                 + "ServiceCodeName:\t" + buildApi.serviceCodeName() + "\n"
                 + "ServiceVersion:\t" + buildApi.serviceVersion() + "\n"
                 + "SDKLevel:\t" + buildApi.sdkLevel() + "\n"
                 + "minSDKlevel:\t" + buildApi.minSDKLevelForService() + "\n"
                 + "ServiceLevel:\t" + buildApi.serviceLevel() + "\n"
                 + "minServiceLevel:\t" + buildApi.minServiceLevelForSDK() + "\n"

                 ;
    }

    /// <summary>
    /// Quit Current Application
    /// </summary>
    public void QuitApplication()
    {
        Application.Quit();
    }
}
