using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JMRSDK;

public class SkyboxAddition : MonoBehaviour
{
    Camera head, left, right;
    private float targetfarclip = 1000;
    private float fov = 60;

    private void Awake()
    {
        head = JMRRigManager.Instance.transform.Find("JMRRenderer/Head")?.GetComponent<Camera>();
        left = JMRRigManager.Instance.transform.Find("JMRRenderer/Head/Left")?.GetComponent<Camera>();
        right = JMRRigManager.Instance.transform.Find("JMRRenderer/Head/Right")?.GetComponent<Camera>();
    }

    private void OnEnable()
    {
        StartCoroutine(AddSkybox());
    }

    IEnumerator AddSkybox()
    {
        for (int i = 0; i < 2; i++) yield return null;
        head.clearFlags = left.clearFlags = right.clearFlags = CameraClearFlags.Skybox;
        head.farClipPlane = left.farClipPlane = right.farClipPlane = targetfarclip;
        head.fieldOfView = left.fieldOfView = right.fieldOfView = fov;
    }
}
