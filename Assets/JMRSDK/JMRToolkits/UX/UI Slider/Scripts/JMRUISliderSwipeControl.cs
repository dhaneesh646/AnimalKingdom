using UnityEngine;
using UnityEngine.UI;
using JMRSDK.InputModule;
using UnityEngine.Events;

namespace JMRSDK.Toolkit.UI
{
    [RequireComponent(typeof(Slider))]
    public class JMRUISliderSwipeControl : JMRBaseThemeAnimator, ISwipeHandler
    {
        [SerializeField]
        private int j_StepOffset;
        [SerializeField]
        private float j_SwipeDelay;
        public bool supportStep = false;
        [SerializeField]
        private bool invertSwipe = false;
        private Slider j_Slider;
        private Slider.Direction j_SlDirection;
        private int j_SwipeDirectionCntrl;
        private float j_AddVal, j_AdjustVal, j_Timer;
        private float prevMinVal, prevMaxVal, step;
        [SerializeField]
        private UnityEvent OnSwipe;


        enum Direction { Horizontal, Vertical }

        public void OnSwipeCanceled(SwipeEventData eventData)
        {

        }

        public void OnSwipeCompleted(SwipeEventData eventData)
        {

        }

        /// <summary>
        /// Set swipe step value
        /// </summary>
        /// <param name="value"></param>
        public void SetStep(int value)
        {
            j_StepOffset = value;
        }

        float cntrl = 0.75f;

        /// <summary>
        /// Handle On Swipe Up Event
        /// </summary>
        /// <param name="eventData"></param>
        /// <param name="value"></param>
        public void OnSwipeUp(SwipeEventData eventData, float value)
        {
            if (!j_Slider.interactable || (supportStep && j_Timer < j_SwipeDelay))
            {
                return;
            }

            j_Timer = 0;
            if (invertSwipe)
            {
                ProcessSwipe(Direction.Vertical, -1f * (supportStep ? step : value * cntrl));
            }
            else
            {
                ProcessSwipe(Direction.Vertical, supportStep ? step : value * cntrl);
            }
        }

        /// <summary>
        /// Handle Swipe Down Event
        /// </summary>
        /// <param name="eventData"></param>
        /// <param name="value"></param>
        public void OnSwipeDown(SwipeEventData eventData, float value)
        {
            if (!j_Slider.interactable || (supportStep && j_Timer < j_SwipeDelay))
            {
                return;
            }

            j_Timer = 0;
            if (invertSwipe)
            {
                ProcessSwipe(Direction.Vertical, -1f * (supportStep ? -step : value * cntrl));
            }
            else
            {
                ProcessSwipe(Direction.Vertical, supportStep ? -step : value * cntrl);
            }
        }

        /// <summary>
        /// Handle On Swipe Left Event
        /// </summary>
        /// <param name="eventData"></param>
        /// <param name="value"></param>
        public void OnSwipeLeft(SwipeEventData eventData, float value)
        {
            if (!j_Slider.interactable || (supportStep && j_Timer < j_SwipeDelay))
            {
                return;
            }

            j_Timer = 0;
            if (invertSwipe)
            {
                ProcessSwipe(Direction.Horizontal, -1f * (supportStep ? -step : value * cntrl));
            }
            else
            {
                ProcessSwipe(Direction.Horizontal, supportStep ? -step : value * cntrl);
            }

        }

        /// <summary>
        /// Handle On Swipe Right Event
        /// </summary>
        /// <param name="eventData"></param>
        /// <param name="value"></param>
        public void OnSwipeRight(SwipeEventData eventData, float value)
        {
            if (!j_Slider.interactable || (supportStep && j_Timer < j_SwipeDelay))
            {
                return;
            }

            j_Timer = 0;
            if (invertSwipe)
            {
                ProcessSwipe(Direction.Horizontal, -1f * (supportStep ? step : value * cntrl));
            }
            else
            {
                ProcessSwipe(Direction.Horizontal, supportStep ? step : value * cntrl);
            }

        }

        public void OnSwipeStarted(SwipeEventData eventData)
        {

        }

        public void OnSwipeUpdated(SwipeEventData eventData, Vector2 swipeData)
        {

        }

        protected override void OnEnable()
        {
            base.OnEnable();
            j_Timer = j_SwipeDelay;
        }

        // Start is called before the first frame update
        public override void Awake()
        {
            base.Awake();
            j_Slider = GetComponent<Slider>();
            prevInteractable = !interactable;
            if (j_Slider)
            {
                CalculateNewStep();
            }
        }

        /// <summary>
        /// Handle Interactable Change to true or false
        /// </summary>
        /// <param name="isInteractable"></param>
        public override void OnInteractableChange(bool isInteractable)
        {
            base.OnInteractableChange(isInteractable);
            j_Slider.interactable = isInteractable;
            prevInteractable = isInteractable;
        }

        bool prevInteractable = false;
        protected override void Update()
        {
            base.Update();

            if (prevInteractable != j_Slider.interactable)
            {
                interactable = j_Slider.interactable;
            }
            prevInteractable = j_Slider.interactable;

            if (j_Slider && (j_Slider.minValue != prevMinVal || j_Slider.maxValue != prevMaxVal))
            {
                CalculateNewStep();
            }

            if (j_Timer > j_SwipeDelay)
            {
                return;
            }

            j_Timer += Time.deltaTime;
        }

        public override void OnSelectClicked(SelectClickEventData eventData)
        {
            if (JMRAnalyticsManager.Instance != null)
                JMRAnalyticsManager.Instance.WriteEvent(JMRAnalyticsManager.Instance.EVENT_XGLSY_GAZE_SLIDER);
        }

        private void CalculateNewStep()
        {
            if (j_Slider.minValue >= j_Slider.maxValue || j_Slider.maxValue == 0)
            {
                return;
            }

            prevMinVal = j_Slider.minValue;
            prevMaxVal = j_Slider.maxValue;
            step = (prevMaxVal - prevMinVal) / 10;
        }

        private void ProcessSwipe(Direction dir, float stepVal)
        {
            j_SlDirection = j_Slider.direction;
            j_SwipeDirectionCntrl = 1;
            j_AddVal = j_StepOffset * stepVal;

            if (dir == Direction.Horizontal && (j_SlDirection == Slider.Direction.BottomToTop || j_SlDirection == Slider.Direction.TopToBottom))
            {
                return;
            }
            else if (dir == Direction.Vertical && (j_SlDirection == Slider.Direction.LeftToRight || j_SlDirection == Slider.Direction.RightToLeft))
            {
                return;
            }
            else
            {
                if (j_SlDirection == Slider.Direction.RightToLeft)
                {
                    j_SwipeDirectionCntrl = -1;
                }
                else if (j_SlDirection == Slider.Direction.TopToBottom)
                {
                    j_SwipeDirectionCntrl = -1;
                }
            }

            OnSwipe?.Invoke();
            j_AdjustVal = j_Slider.value + j_AddVal * j_SwipeDirectionCntrl;
            j_AdjustVal = j_AdjustVal > j_Slider.maxValue ? j_Slider.maxValue : j_AdjustVal < j_Slider.minValue ? j_Slider.minValue : j_AdjustVal;
            j_Slider.value = j_AdjustVal;
        }
    }
}
