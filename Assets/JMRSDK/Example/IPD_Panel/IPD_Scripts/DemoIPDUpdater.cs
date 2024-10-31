using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using JMRSDK;

namespace Launcher
{
    public class DemoIPDUpdater : MonoBehaviour
    {
        [SerializeField] Slider slider;
        [SerializeField] TextMeshProUGUI sliderValueText;
        [SerializeField] TextMeshProUGUI iPDValue;

        private int defaultIPDValue = 62;

        private void Start()
        {
            slider.value = JMRRigManager.Instance.GetIPD();
            sliderValueText.text = JMRRigManager.Instance.GetIPD().ToString();


        }
        public void OnSliderValueChange()
        {
            sliderValueText.text = slider.value.ToString();
            SetIPD();
        }

        public void SetIPD()
        {

            bool isIPDset=JMRRigManager.Instance.SetIPD(Mathf.RoundToInt(slider.value));
            Debug.Log("jmrsdk11: isIPD set=>>>>>>> " + isIPDset+ " get IPD has sent:=>>> "+ JMRRigManager.Instance.GetIPD().ToString());
        }

        public void GetIPD()
        {
            iPDValue.text = "Get IPD: " + JMRRigManager.Instance.GetIPD().ToString();

        }

        public void OnReset()
        {
            slider.value = defaultIPDValue;
            SetIPD();
        }
    } 
}
