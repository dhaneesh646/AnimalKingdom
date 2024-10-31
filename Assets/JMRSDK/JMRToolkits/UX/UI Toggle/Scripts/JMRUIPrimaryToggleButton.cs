// Copyright (c) 2020 JioGlass. All Rights Reserved.

using JMRSDK.InputModule;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

namespace JMRSDK.Toolkit
{
    public class JMRUIPrimaryToggleButton : JMRBaseThemeAnimator
    {
        [SerializeField]
        private TMP_Text text;
        public string OnString,OffString;
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
        /// On Value Changed Event Listner
        /// </summary>
        public UnityEventBool OnValueChanged { get { if (valueChanged == null) valueChanged = new UnityEventBool(); return valueChanged; } set { valueChanged = value; } }
        public bool hasParent { get; set; }

        public override void Awake()
        {
            base.Awake();
            isSelected = IsOn;
            if (isSelected)
            {
                text.text = OnString;
            }
            else
            {
                text.text = OffString;
            }
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
        /// Handle Object Select Event
        /// </summary>
        protected override void OnObjectSelect()
        {
            IsOn = isSelected;
            text.text = OnString;
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
            text.text = OffString;
            base.OnObjectDeselect();
            OnDeselect?.Invoke();
            SetDynamicValueChange(false);
        }

        /// <summary>
        /// Handle Select Clicked
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnSelectClicked(SelectClickEventData eventData)
        {
            if (!hasParent)
            {
                base.OnSelectClicked(eventData);
            }
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
                JMRAnalyticsManager.Instance.WriteEvent(JMRAnalyticsManager.Instance.EVENT_XGLSY_GAZE_TOGGLE);
        }
    }
}
