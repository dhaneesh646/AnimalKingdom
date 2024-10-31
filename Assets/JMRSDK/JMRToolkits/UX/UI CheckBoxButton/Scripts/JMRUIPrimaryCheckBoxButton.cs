﻿// Copyright (c) 2020 JioGlass. All Rights Reserved.

using UnityEngine;
using UnityEngine.Events;

namespace JMRSDK.Toolkit
{
    public class JMRUIPrimaryCheckBoxButton : JMRBaseThemeAnimator
    {
        [SerializeField]
        private bool isOn;
        public bool IsOn { get { return isOn; } set { isOn = value; } }
        [Header("Events")]
        [SerializeField]
        private UnityEventBool onValueChanged;
        [SerializeField]
        public UnityEvent OnSelect, OnDeselect;
        private UnityEventBool valueChanged;

        /// <summary>
        /// Checkbox Event Listner
        /// </summary>
        public UnityEventBool OnValueChanged { get { if (valueChanged == null) valueChanged = new UnityEventBool(); return valueChanged; } set { valueChanged = value; } }

        public override void Awake()
        {
            base.Awake();
            isSelected = IsOn;
        }

        protected override void Update()
        {
            base.Update();
            if(isSelected != IsOn)
            {
                base.OnSelectClicked(null);
            }
        }

        /// <summary>
        /// Handle Object Select
        /// </summary>
        protected override void OnObjectSelect()
        {
            IsOn = isSelected;
            base.OnObjectSelect();
            OnSelect?.Invoke();
            SetDynamicValueChange(true);
        }

        /// <summary>
        /// Handle Object Deselect
        /// </summary>
        protected override void OnObjectDeselect()
        {
            IsOn = isSelected;
            base.OnObjectDeselect();
            OnDeselect?.Invoke();
            SetDynamicValueChange(false);
        }

        /// <summary>
        /// Set Dynamic Value to true/false
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
                JMRAnalyticsManager.Instance.WriteEvent(JMRAnalyticsManager.Instance.EVENT_XGLSY_GAZE_CHECKBOX);
        }
    }
}
