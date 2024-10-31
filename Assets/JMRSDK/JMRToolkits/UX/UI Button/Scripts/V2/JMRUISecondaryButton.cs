// Copyright (c) 2020 JioGlass. All Rights Reserved.

using UnityEngine;
using UnityEngine.Events;

namespace JMRSDK.Toolkit
{
    public class JMRUISecondaryButton : JMRBaseThemeAnimator
    {
        [SerializeField]
        private bool isOn;
        public bool IsOn { get { return isOn; } set { isOn = value; } }
        [Header("Events")]
        [SerializeField]
        private UnityEvent onSelect,onDeselect;
        private UnityEvent OnSelectEvent,OnDeselectEvent;

        /// <summary>
        /// On Select Event Listner
        /// </summary>
        public UnityEvent OnSelect { get { if (OnSelectEvent == null) { OnSelectEvent = new UnityEvent(); }return OnSelectEvent;   }  private set { OnSelectEvent = value; } }

        /// <summary>
        /// On Deselect Event Listner
        /// </summary>
        public UnityEvent OnDeselect { get { if (OnDeselectEvent == null) { OnDeselectEvent = new UnityEvent(); } return OnDeselectEvent; } private set { OnDeselectEvent = value; } }
        public override void Awake()
        {
            base.Awake();
            isSelected = IsOn;
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
        /// Handle on Object Select
        /// </summary>
        protected override void OnObjectSelect()
        {
            IsOn = isSelected;
            base.OnObjectSelect();
            onSelect?.Invoke();
            OnSelect?.Invoke();
            if (JMRAnalyticsManager.Instance != null)
                JMRAnalyticsManager.Instance.WriteEvent(JMRAnalyticsManager.Instance.EVENT_XGLSY_GAZE_SECONDARYBUTTON);
        }

        /// <summary>
        /// Handle on Object Deselect
        /// </summary>
        protected override void OnObjectDeselect()
        {
            IsOn = isSelected;
            base.OnObjectDeselect();
            onDeselect?.Invoke();
            OnDeselect?.Invoke();
        }
    }
}
