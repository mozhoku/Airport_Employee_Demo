using Core;
using TMPro;
using UnityEngine;
using System.Collections;

namespace UI
{
    public class UINumberCounter : MonoBehaviour
    {
        [Header("UI Settings")]
        [SerializeField] private TextMeshProUGUI numberText;
        [SerializeField] private int startValue = 1000;
        [SerializeField] private int targetValue = 800;
        [SerializeField] private float decreaseDuration = 2f;
        [SerializeField] private float reverseSpeed = 2f;

        [Header("Behaviour Settings")]
        [Tooltip("If true, the counter resets to starting value when completed and player re-enters.")]
        [SerializeField] private bool resettable = false;

        private Coroutine _countRoutine;
        private bool _isCompleted;
        private int _currentValue;

        private void Awake()
        {
            _currentValue = startValue;
            if (numberText != null)
                numberText.text = _currentValue.ToString();
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

            if (_countRoutine != null)
            {
                StopCoroutine(_countRoutine);
                _countRoutine = null;
            }
        }

        private void OnPlayerEnter(GameEvents.AreaUnlockEntered e)
        {
            if (!e.PlayerEntered) return;

            if (resettable && _isCompleted)
            {
                _isCompleted = false;
                _currentValue = startValue;
                numberText.text = _currentValue.ToString();
            }

            if (_countRoutine == null && !_isCompleted)
                _countRoutine = StartCoroutine(CountCoroutine(forward: true));
        }

        private void OnPlayerExit(GameEvents.AreaUnlockExited e)
        {
            if (!e.PlayerExited || _isCompleted) return;

            if (_countRoutine != null)
            {
                StopCoroutine(_countRoutine);
                _countRoutine = null;
            }

            _countRoutine = StartCoroutine(CountCoroutine(forward: false));
        }

        private IEnumerator CountCoroutine(bool forward)
        {
            float timer = Mathf.InverseLerp(startValue, targetValue, _currentValue);
            float totalTime = decreaseDuration;

            while (true)
            {
                float delta = Time.deltaTime * (forward ? 1f : reverseSpeed);
                timer += forward ? delta / totalTime : -delta / totalTime;

                float t = Mathf.Clamp01(timer);
                _currentValue = Mathf.RoundToInt(Mathf.Lerp(startValue, targetValue, t));

                if (numberText)
                    numberText.text = _currentValue.ToString();

                // Completed (when reached target)
                if (forward && t >= 1f)
                {
                    _isCompleted = true;
                    EventBus.Publish(new UIEvents.UINumberDecreaseComplete());
                    break;
                }

                // Reverted fully (when reached start)
                if (!forward && t <= 0f)
                {
                    EventBus.Publish(new UIEvents.UINumberIncreaseComplete());
                    break;
                }

                yield return null;
            }

            _countRoutine = null;
        }
    }
}
