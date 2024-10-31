// Copyright (c) 2020 JioGlass. All Rights Reserved.

using JMRSDK.InputModule;
using UnityEngine;
using UnityEngine.Events;

namespace JMRSDK.Toolkit.UI
{
    [RequireComponent(typeof(Animator))]
    public class JMRCheckBoxButtonAnimator : MonoBehaviour
    {
        [SerializeField, Header("Property")]
        private JMRInteractable interactable;
        [Header("Events")]
        [SerializeField]
        private UnityEvent onFocusEnter, onFocusExit, onPointerDown, onPointerUp;

        private Animator themeAnimator;
        private bool isEnable;
        private bool isPressed = false;

        private void Start()
        {
            if (!interactable)
                interactable = gameObject.GetComponent<JMRInteractable>();
            themeAnimator = gameObject.GetComponent<Animator>();
            interactable.FocusEnter += OnFocusEnter;
            interactable.FocusExit += OnFocusExit;
            interactable.InputDown += OnPointerDown;
            interactable.InputUp += OnPointerUp;
            interactable.OnEnableChange += OnEnableChange;
            isEnable = interactable.IsEnabled;
            if (!isEnable)
                themeAnimator.SetTrigger("Disabled");
        }

        /// <summary>
        /// Handle Enable Change
        /// </summary>
        /// <param name="obj"></param>
        private void OnEnableChange(bool obj)
        {
            isEnable = obj;
        }

        /// <summary>
        /// Handle Pointer Up
        /// </summary>
        private void OnPointerUp()
        {
            if (!isEnable)
                return;

            //Debug.LogError("Pointer Up Called");

            onPointerUp?.Invoke();
        }

        /// <summary>
        /// Handle Pointer Down
        /// </summary>
        private void OnPointerDown()
        {
            if (!isEnable)
                return;

            //Debug.LogError("Pointer Down Called");
            onPointerDown?.Invoke();

            isPressed = !isPressed;
            themeAnimator.SetTrigger("Pressed");
        }

        /// <summary>
        /// Handle Focus Exit
        /// </summary>
        private void OnFocusExit()
        {
            if (!isEnable)
                return;

            onFocusExit?.Invoke();
            if(!isPressed) themeAnimator.SetTrigger("Normal");

            Debug.Log("OnFocusExit = " + name);
        }

        /// <summary>
        /// Handle Focus Enter
        /// </summary>
        private void OnFocusEnter()
        {
            if (!isEnable)
                return;

            onFocusEnter?.Invoke();
            if(!isPressed) themeAnimator.SetTrigger("Highlighted");

            Debug.Log("OnFocusEnter = " + name);
        }

        private void OnDestroy()
        {
            interactable.FocusEnter -= OnFocusEnter;
            interactable.FocusExit -= OnFocusExit;
            interactable.InputDown -= OnPointerDown;
            interactable.InputUp -= OnPointerUp;
            interactable.OnEnableChange -= OnEnableChange;
        }
    }
}