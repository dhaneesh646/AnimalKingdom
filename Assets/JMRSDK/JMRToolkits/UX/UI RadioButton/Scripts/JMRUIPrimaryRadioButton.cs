﻿// Copyright (c) 2020 JioGlass. All Rights Reserved.

using JMRSDK.InputModule;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace JMRSDK.Toolkit
{
    public class JMRUIPrimaryRadioButton : JMRBaseThemeAnimator
    {
        [SerializeField]
        private bool isOn;
        public bool IsOn { get { return isOn; } set { isOn = value; } }
        [Header("Events")]
        [SerializeField]
        private UnityEventBool onValueChanged;
        private UnityEventBool valueChanged;

        /// <summary>
        /// On Value Changed Event Listner
        /// </summary>
        public UnityEventBool OnValueChanged { get { if (valueChanged == null) valueChanged = new UnityEventBool(); return valueChanged; } set { valueChanged = value; } }
        [SerializeField]
        public UnityEvent OnSelect, OnDeselect;
        public Action<JMRUIPrimaryRadioButton> parentClickHandler;

        public override void Awake()
        {
            base.Awake();
            isSelected = IsOn;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            parentClickHandler = null;
        }

        protected override void Update()
        {
            base.Update();
            if (isSelected != IsOn)
            {
                base.OnSelectClicked(null);
            }
        }

        /// <summary>
        /// Handle Object Selected Event
        /// </summary>
        protected override void OnObjectSelect()
        {
            IsOn = isSelected;
            base.OnObjectSelect();
            OnSelect?.Invoke();
            SetDynamicValueChange(true);
        }

        /// <summary>
        /// Handle Object Deselect Event
        /// </summary>
        protected override void OnObjectDeselect()
        {
            IsOn = isSelected;
            base.OnObjectDeselect();
            OnDeselect?.Invoke();
            SetDynamicValueChange(false);
        }

        /// <summary>
        /// Handle Select Clicked Event
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnSelectClicked(SelectClickEventData eventData)
        {
            if (parentClickHandler == null)
            {
                base.OnSelectClicked(eventData);
            }
            else
            {
                parentClickHandler.Invoke(this);
            }
        }

        /// <summary>
        /// Handle Set Dynamic Value change to true/false
        /// </summary>
        /// <param name="value"></param>
        private void SetDynamicValueChange(bool value)
        {
            if (onValueChanged != null)
            {
                for (int i = 0; i < onValueChanged.GetPersistentEventCount(); i++)
                {
                    ((MonoBehaviour)onValueChanged.GetPersistentTarget(i)).SendMessage(onValueChanged.GetPersistentMethodName(i), value);
                }
            }
            OnValueChanged?.Invoke(value);
            if (JMRAnalyticsManager.Instance != null)
                JMRAnalyticsManager.Instance.WriteEvent(JMRAnalyticsManager.Instance.EVENT_XGLSY_GAZE_RADIOBUTTON);
        }
    }
}
