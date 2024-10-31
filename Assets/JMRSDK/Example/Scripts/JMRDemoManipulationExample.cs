using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JMRSDK;
using JMRSDK.InputModule;
using System;

public class JMRDemoManipulationExample : MonoBehaviour
{
    [SerializeField] private GameObject[] DragAndDropObjects;
    [SerializeField] private GameObject[] RotateAroundObjects;

    // Start is called before the first frame update
    void Start()
    {
        JMRRigManager.Instance.setHomePage = false;
    }

    void OnEnable()
    {
        JMRInteractionManager.OnConnected += onConnected;
        JMRInteractionManager.OnDisconnected += onDisconnected;
        ToggleHostVisibility();
    }

    void OnDisable()
    {
        JMRInteractionManager.OnConnected -= onConnected;
        JMRInteractionManager.OnDisconnected -= onDisconnected;
    }

    private void onDisconnected(JMRInteractionManager.InteractionDeviceType devType, int index, string name)
    {
        Debug.Log($"== JMRDemoManipulationExample : OnController Connected {devType}");
        ToggleHostVisibility();
    }

    private void onConnected(JMRInteractionManager.InteractionDeviceType devType, int index, string name)
    {
        Debug.Log($"== JMRDemoManipulationExample : OnController DisConnected {devType}");
        ToggleHostVisibility();
    }

    private void ToggleHostVisibility()
    { 
        var currentPoiningSource = JMRPointerManager.Instance.CurrentPointingSource;
        Debug.Log($"== JMRDemoManipulationExample : toggleHostVisibility {currentPoiningSource}");
        switch (currentPoiningSource)
        {
            case JMRPointerManager.PointingSource.Head:
                Array.ForEach(RotateAroundObjects, arrayItem => arrayItem.SetActive(false));
                break;
            case JMRPointerManager.PointingSource.JioGlassController:
                Array.ForEach(RotateAroundObjects, arrayItem => arrayItem.SetActive(true));
                break;
        }
    }
}
