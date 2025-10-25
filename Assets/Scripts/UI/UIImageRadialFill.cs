using Core;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace UI
{
    internal enum AreaName
    {
        Area1,
        Area2,
    }

    public class UIRadialFill : MonoBehaviour
    {
        [Header("UI Settings")] [SerializeField]
        private Image radialImage;

        [SerializeField] private float fillDuration = 2f;
        [SerializeField] private float reverseSpeed = 2f;

        [Header("Behaviour Settings")]
        [Tooltip("If true, the fill resets to 0 after completion when the player re-enters.")]
        [SerializeField]
        private bool resettable = false;

        private Coroutine _fillRoutine;
        private bool _isCompleted;

        private void Awake()
        {
            if (radialImage != null)
                radialImage.fillAmount = 0f;
        }

        private void OnEnable()
        {
            EventBus.Subscribe<GameEvents.AreaUnlockEntered>(OnPlayerEnter);
            EventBus.Subscribe<GameEvents.AreaUnlockExited>(OnPlayerExit);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<GameEvents.AreaUnlockEntered>(OnPlayerEnter);
            EventBus.Unsubscribe<GameEvents.AreaUnlockExited>(OnPlayerExit);

            if (_fillRoutine != null)
            {
                StopCoroutine(_fillRoutine);
                _fillRoutine = null;
            }
        }

        private void OnPlayerEnter(GameEvents.AreaUnlockEntered e)
        {
            if (!e.PlayerEntered) return;

            // Reset if allowed
            if (resettable && _isCompleted)
            {
                _isCompleted = false;
                radialImage.fillAmount = 0f;
            }

            if (_fillRoutine == null && !_isCompleted)
                _fillRoutine = StartCoroutine(FillCoroutine(forward: true));
        }

        private void OnPlayerExit(GameEvents.AreaUnlockExited e)
        {
            if (!e.PlayerExited || _isCompleted) return;

            if (_fillRoutine != null)
            {
                StopCoroutine(_fillRoutine);
                _fillRoutine = null;
            }

            _fillRoutine = StartCoroutine(FillCoroutine(forward: false));
        }

        private IEnumerator FillCoroutine(bool forward)
        {
            float timer = radialImage.fillAmount * fillDuration;

            while (true)
            {
                float delta = Time.deltaTime * (forward ? 1f : reverseSpeed);
                timer += forward ? delta : -delta;

                float t = Mathf.Clamp01(timer / fillDuration);
                radialImage.fillAmount = t;

                // Completed
                if (forward && t >= 1f)
                {
                    _isCompleted = true;
                    EventBus.Publish(new UIEvents.UIFillComplete());
                    break;
                }

                // Cancelled (only before completion)
                if (!forward && t <= 0f)
                {
                    EventBus.Publish(new UIEvents.UIFillCancelled());
                    break;
                }

                yield return null;
            }

            _fillRoutine = null;
        }
    }
}