using System.Collections;
using Core;
using UI;
using UnityEngine;

namespace Utilities
{
    public class ScaleAnimator : MonoBehaviour
    {
        [Header("Scale Settings")] [SerializeField]
        private float duration = 0.5f;

        [SerializeField] private float targetScale = 1f; // Scale to reach on FillComplete
        [SerializeField] private bool disableOnEnd = false;

        [Header("Ease Settings")]
        [Tooltip("Controls the scale transition shape (e.g., ease-in, bounce, elastic).")]
        [SerializeField]
        private AnimationCurve easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("Start Settings")] [Tooltip("Initial scale of the object when entering Play Mode.")] [SerializeField]
        private float initialPlayScale = 0f;

        [Tooltip("If true, sets the scale automatically when play starts.")] [SerializeField]
        private bool setScaleOnStart = true;

        private Vector3 _defaultScale;
        private Coroutine _scaleRoutine;

        private void Start()
        {
            _defaultScale = transform.localScale;

            // Apply initial play scale if enabled
            if (Application.isPlaying && setScaleOnStart)
                transform.localScale = Vector3.one * initialPlayScale;
        }

        private void OnEnable()
        {
            EventBus.Subscribe<UIEvents.UIFillComplete>(OnFillComplete);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<UIEvents.UIFillComplete>(OnFillComplete);

            if (_scaleRoutine != null)
            {
                StopCoroutine(_scaleRoutine);
                _scaleRoutine = null;
            }
        }

        private void OnFillComplete(UIEvents.UIFillComplete e)
        {
            StartScale(targetScale);
        }

        /// <summary>
        /// Public method to trigger scaling manually.
        /// </summary>
        public void StartScale(float endScale)
        {
            if (_scaleRoutine != null)
                StopCoroutine(_scaleRoutine);

            _scaleRoutine = StartCoroutine(ScaleRoutine(endScale));
        }

        private IEnumerator ScaleRoutine(float endScale)
        {
            Vector3 startScale = transform.localScale;
            Vector3 target = Vector3.one * endScale;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float easedT = easeCurve.Evaluate(t);
                transform.localScale = Vector3.Lerp(startScale, target, easedT);
                yield return null;
            }

            transform.localScale = target;

            if (disableOnEnd)
                gameObject.SetActive(false);

            _scaleRoutine = null;
        }
    }
}