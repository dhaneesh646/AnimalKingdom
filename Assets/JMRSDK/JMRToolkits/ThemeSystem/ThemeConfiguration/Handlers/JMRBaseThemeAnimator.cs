// Copyright (c) 2020 JioGlass. All Rights Reserved.

using UnityEngine;
using JMRSDK.InputModule;
using System.Collections;

namespace JMRSDK.Toolkit
{
    [RequireComponent(typeof(Animator))]
    public class JMRBaseThemeAnimator : JMRUIInteractable, IFocusable, ISelectClickHandler
    {
        public bool showAppearAnimation = true;
        protected Animator jmrThemeAnimator;
        private JMRConstantThemeSystem.SelectableState jmrCurrentSelectableState = JMRConstantThemeSystem.SelectableState.None;
        private bool isInteractable, isGlobal, isInitialized = false, isClicked = false, isGlobalAdded = false;
        private float timer = 0.2f, checkISelectDelay = 0.2f;

        public virtual void Awake()
        {
            isInteractable = interactable;
            jmrThemeAnimator = GetComponent<Animator>();
        }

        protected virtual void OnEnable()
        {
            isGlobal = !global;
            isFocused = false;

            if (interactable)
            {
                if (!isInitialized)
                {
                    StartCoroutine(SetEndAnimatorEvents());
                }
                else
                {
                    if (showAppearAnimation)
                    {
                        ChangeSelectableState(JMRConstantThemeSystem.SelectableState.Appear);
                    }
                    else
                    {
                        LoadState();
                    }
                }
            }
            else
            {
                ChangeSelectableState(JMRConstantThemeSystem.SelectableState.Disabled);
            }
        }

        protected virtual void Update()
        {
            timer += Time.deltaTime;
            if (timer >= checkISelectDelay)
            {
                if (isInteractable != interactable)
                {
                    isInteractable = interactable;
                    OnInteractableChange(interactable);
                    if (interactable)
                    {
                        LoadState();
                    }
                    else
                    {
                        ChangeSelectableState(JMRConstantThemeSystem.SelectableState.Disabled);
                    }
                }

                if (isGlobal != global)
                {
                    isGlobal = global;
                    if (global)
                    {
                        isGlobalAdded = true;
                        JMRInputManager.Instance.AddGlobalListener(gameObject);
                    }
                    else if (isGlobalAdded)
                    {
                        isGlobalAdded = false;
                        JMRInputManager.Instance.RemoveGlobalListener(gameObject);
                    }
                }

                timer = 0;
            }
        }

        /// <summary>
        /// Set Animator End Events
        /// </summary>
        /// <returns></returns>
        IEnumerator SetEndAnimatorEvents()
        {
            if (jmrThemeAnimator.runtimeAnimatorController == null)
                yield break;

            AnimationClip[] clips = jmrThemeAnimator.runtimeAnimatorController.animationClips;
            int count = (clips.Length + 1) / 2;
            int cntrl = 0;
            foreach (AnimationClip clip in clips)
            {
                cntrl += 1;
                AnimationEvent evt = null;
                bool isAnimationEventAdded = false;
                foreach (AnimationEvent animEvent in clip.events)
                {
                    if (animEvent.functionName == "OnAnimationEnd")
                    {
                        isAnimationEventAdded = true;
                        break;
                    }
                }
                if (!isAnimationEventAdded)
                {
                    evt = new AnimationEvent();
                    evt.time = clip.length;
                    evt.functionName = "OnAnimationEnd";
                    clip.AddEvent(evt);
                }
                if (cntrl == count)
                {
                    count = 0;
                    yield return new WaitForEndOfFrame();
                }
            }
            yield return new WaitForEndOfFrame();

            isInitialized = true;
            if (showAppearAnimation)
            {
                ChangeSelectableState(JMRConstantThemeSystem.SelectableState.Appear);
            }
            else
            {
                LoadState();
            }
        }

        /// <summary>
        /// Load Animator State
        /// </summary>
        private void LoadState()
        {
            if (isClicked)
            {
                if (isSelected)
                {
                    ChangeSelectableState(JMRConstantThemeSystem.SelectableState.OnselectionHoverClick);
                }
                else
                {
                    ChangeSelectableState(JMRConstantThemeSystem.SelectableState.Pressed);
                }
                isClicked = false;
            }
            else if (isFocused)
            {
                if (isSelected && jmrCurrentSelectableState != JMRConstantThemeSystem.SelectableState.OnSelectionHover)
                {
                    ChangeSelectableState(JMRConstantThemeSystem.SelectableState.OnSelectionHover);
                }
                else if (!isSelected && jmrCurrentSelectableState != JMRConstantThemeSystem.SelectableState.Hover)
                {
                    ChangeSelectableState(JMRConstantThemeSystem.SelectableState.Hover);
                }
            }
            else if (!isFocused)
            {
                if (isSelected && jmrCurrentSelectableState != JMRConstantThemeSystem.SelectableState.OnSelection)
                {
                    ChangeSelectableState(JMRConstantThemeSystem.SelectableState.OnSelection);
                }
                else if (!isSelected && jmrCurrentSelectableState != JMRConstantThemeSystem.SelectableState.Default)
                {
                    ChangeSelectableState(JMRConstantThemeSystem.SelectableState.Default);
                }
            }
        }

        /// <summary>
        /// Handle On Animation End Event
        /// </summary>
        private void OnAnimationEnd()
        {
            if (!interactable)
            {
                return;
            }
            LoadState();
        }

        /// <summary>
        /// Handle On Focus Enter Event
        /// </summary>
        public virtual void OnFocusEnter()
        {
            isFocused = true;
            if (!interactable)
            {
                return;
            }
            LoadState();
        }

        /// <summary>
        /// Handle Focus Exit Event
        /// </summary>
        public virtual void OnFocusExit()
        {
            isFocused = false;
            if (!interactable)
            {
                return;
            }
            LoadState();
        }

        /// <summary>
        /// Handle Select Clicked Event
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnSelectClicked(SelectClickEventData eventData)
        {
            if (!interactable)
            {
                return;
            }

            isClicked = true;
            isSelected = !isSelected;
            if (isSelected)
            {
                OnObjectSelect();
            }
            else
            {
                OnObjectDeselect();
            }
            LoadState();
        }

        /// <summary>
        /// Change Selectable State
        /// </summary>
        /// <param name="state"></param>
        private void ChangeSelectableState(JMRConstantThemeSystem.SelectableState state)
        {
            if (jmrCurrentSelectableState == JMRConstantThemeSystem.SelectableState.Hover && !isFocused)
            {
                //Debug.LogError("Rebinding");
                //jmrThemeAnimator.Rebind();
            }
            switch (state)
            {
                case JMRConstantThemeSystem.SelectableState.Appear:
                    ChangeToAppear();
                    break;
                case JMRConstantThemeSystem.SelectableState.Disappear:
                    ChangeToDisappear();
                    break;
                case JMRConstantThemeSystem.SelectableState.Default:
                    ChangeToDefault();
                    break;
                case JMRConstantThemeSystem.SelectableState.Hover:
                    ChangeToHover();
                    break;
                case JMRConstantThemeSystem.SelectableState.Disabled:
                    ChangeToDisabled();
                    break;
                case JMRConstantThemeSystem.SelectableState.Pressed:
                    ChangeToPressed();
                    break;
                case JMRConstantThemeSystem.SelectableState.OnSelection:
                    ChangeToOnSelection();
                    break;
                case JMRConstantThemeSystem.SelectableState.OnSelectionHover:
                    ChangeToOnSelectionHover();
                    break;
                case JMRConstantThemeSystem.SelectableState.OnselectionHoverClick:
                    ChangeToOnselectionHoverClick();
                    break;
            }
            jmrCurrentSelectableState = state;
        }

        /// <summary>
        /// Change Animator State to Appear
        /// </summary>
        protected virtual void ChangeToAppear()
        {
            jmrThemeAnimator.SetTrigger(JMRConstantThemeSystem.JMRButtonStates.APPEAR);
        }

        /// <summary>
        /// Change Animator State to Disappear
        /// </summary>
        protected virtual void ChangeToDisappear()
        {
            jmrThemeAnimator.SetTrigger(JMRConstantThemeSystem.JMRButtonStates.DISAPPEAR);
        }

        /// <summary>
        /// Change Animator State to Default
        /// </summary>
        protected virtual void ChangeToDefault()
        {
            jmrThemeAnimator.SetTrigger(JMRConstantThemeSystem.JMRButtonStates.DEFAULT);
        }

        /// <summary>
        /// Change Animator State to Hover
        /// </summary>
        protected virtual void ChangeToHover()
        {
            jmrThemeAnimator.SetTrigger(JMRConstantThemeSystem.JMRButtonStates.HOVER);
            //Trigger Haptics
            //JMRInteractionManager.Instance.TriggerHaptics(JMRInteractionManager.Instance.HAPTICS_HOVER, JMRInteractionManager.Instance.HAPTICS_INTENSITY_MEDIUM, 0);
        }

        /// <summary>
        /// Change Animator State to Disabled
        /// </summary>
        protected virtual void ChangeToDisabled()
        {
            jmrThemeAnimator.SetTrigger(JMRConstantThemeSystem.JMRButtonStates.DISABLED);
        }

        /// <summary>
        /// Change Animator State to Pressed
        /// </summary>
        protected virtual void ChangeToPressed()
        {
            jmrThemeAnimator.SetTrigger(JMRConstantThemeSystem.JMRButtonStates.PRESSED);
        }

        /// <summary>
        /// Change Animator State to Selection
        /// </summary>
        protected virtual void ChangeToOnSelection()
        {
            jmrThemeAnimator.SetTrigger(JMRConstantThemeSystem.JMRButtonStates.ONSELECTION);
        }

        /// <summary>
        /// Change Animator State to Selection Hover
        /// </summary>
        protected virtual void ChangeToOnSelectionHover()
        {
            jmrThemeAnimator.SetTrigger(JMRConstantThemeSystem.JMRButtonStates.ONHOVERSELECTION);
        }

        /// <summary>
        /// Change Animator State to Selection Hover Click
        /// </summary>
        protected virtual void ChangeToOnselectionHoverClick()
        {
            jmrThemeAnimator.SetTrigger(JMRConstantThemeSystem.JMRButtonStates.ONHOVERSELECTIONCLICK);
        }

        //Dont remove the method is used in child
        public virtual void OnInteractableChange(bool isInteractable)
        {

        }

        //Dont remove the method is used in child
        protected virtual void OnDisable()
        {
            if (isGlobalAdded)
            {
                JMRInputManager.Instance.RemoveGlobalListener(gameObject);
            }
        }

        //Dont remove the method is used in child
        protected virtual void OnObjectSelect()
        {
        }

        //Dont remove the method is used in child
        protected virtual void OnObjectDeselect()
        {
        }
    }
}
