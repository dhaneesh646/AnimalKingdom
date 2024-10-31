// Copyright (c) 2020 JioGlass. All Rights Reserved.

using UnityEngine;
using UnityEngine.Events;

namespace JMRSDK.Toolkit
{
    public class JMRUIPrimaryButton : JMRBaseThemeAnimator
    {
        [Header("Events")]
        [SerializeField]
        private UnityEvent onClick;
        private UnityEvent OnClickEvent;

        /// <summary>
        /// On Click Event Listner
        /// </summary>
        public UnityEvent OnClick { get { if (OnClickEvent == null) { OnClickEvent = new UnityEvent(); }return OnClickEvent;   }  private set { OnClickEvent = value; } }

        /// <summary>
        /// Handle Object select
        /// </summary>
        protected override void OnObjectSelect()
        {
            base.OnObjectSelect();
            onClick?.Invoke();
            OnClick?.Invoke();
            if (JMRAnalyticsManager.Instance != null)
                JMRAnalyticsManager.Instance.WriteEvent(JMRAnalyticsManager.Instance.EVENT_XGLSY_GAZE_PRIMARYBUTTON);
        }

        /// <summary>
        /// Handle Object Deselect
        /// </summary>
        protected override void OnObjectDeselect()
        {
            base.OnObjectDeselect();
            onClick?.Invoke();
            OnClick?.Invoke();
        }

        /// <summary>
        /// Change state from hover to click
        /// </summary>
        protected override void ChangeToPressed()
        {
            base.ChangeToOnselectionHoverClick();
        }

        /// <summary>
        /// Change state to Default
        /// </summary>
        protected override void ChangeToOnSelection()
        {
            base.ChangeToDefault();
        }

        /// <summary>
        /// Change state to Hover 
        /// </summary>
        protected override void ChangeToOnSelectionHover()
        {
            base.ChangeToHover();
        }
    }
}
