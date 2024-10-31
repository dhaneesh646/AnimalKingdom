using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JMRSDK.InputModule;
using JMRSDK.Toolkit.UI;
using TMPro;

public class JMRDemoToolkit : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI keyboardEventText;

    private void OnEnable()
    {
        SubscribeKeyboardCallbacks();        
    }

    private void OnDisable()
    {
        UnSubscribeKeyboardCallbacks();
    }

    private void SubscribeKeyboardCallbacks()
    {
        JMRVirtualKeyBoard.OnKeyboardOpen += OnKeyboardOpenCallback;
        JMRVirtualKeyBoard.OnKeyboardClose += OnkeyboardCloseCalback;
        JMRVirtualKeyBoard.OnKeyboardTextClear += OnKeyboardTextClearCallback;
    }

    private void UnSubscribeKeyboardCallbacks()
    {
        JMRVirtualKeyBoard.OnKeyboardOpen -= OnKeyboardOpenCallback;
        JMRVirtualKeyBoard.OnKeyboardClose -= OnkeyboardCloseCalback;
        JMRVirtualKeyBoard.OnKeyboardTextClear -= OnKeyboardTextClearCallback;
    }

    private void OnKeyboardOpenCallback()
    {
        keyboardEventText.text = "Keyboard Open";
    }

    private void OnkeyboardCloseCalback()
    {
        keyboardEventText.text = "keyboard Close";
    }

    private void OnKeyboardTextClearCallback()
    {
        keyboardEventText.text = "Keyboard Text Clear";
    }

}
