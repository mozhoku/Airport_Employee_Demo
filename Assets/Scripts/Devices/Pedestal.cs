using System.Collections;
using Core;
using UnityEngine;

namespace Devices
{
    public class Pedestal : MonoBehaviour
    {
        [Header("Pedestal Settings")]
        [SerializeField] private Transform pushStartPoint;     // where luggage sits initially
        [SerializeField] private Transform targetPosition;     // where the pedestal pushes luggage toward (e.g. arc end)
        [SerializeField] private Transform finalParent;        // e.g. truck transform
        [SerializeField] private float pushDuration = 1.5f;    // total push time
        [SerializeField] private AnimationCurve pushCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private bool _isPushing;

        private void OnEnable()
        {
            EventBus.Subscribe<GameEvents.LuggageDropped>(OnLuggageDropped);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<GameEvents.LuggageDropped>(OnLuggageDropped);
        }

        private void OnLuggageDropped(GameEvents.LuggageDropped e)
        {
            // Only react if the luggage is dropped near this pedestal
            if (!_isPushing && Vector3.Distance(e.Luggage.position, pushStartPoint.position) < 2f)
                StartCoroutine(PushRoutine(e.Luggage));
        }

        private IEnumerator PushRoutine(Transform luggage)
        {
            _isPushing = true;

            Vector3 start = pushStartPoint.position;
            Vector3 end = targetPosition.position;

            // Simulate an upward arc (optional)
            Vector3 peak = (start + end) / 2f + Vector3.up * 1.5f;

            float elapsed = 0f;
            while (elapsed < pushDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / pushDuration);
                float curveValue = pushCurve.Evaluate(t);

                // Parabolic interpolation for smooth "throw"
                Vector3 a = Vector3.Lerp(start, peak, curveValue);
                Vector3 b = Vector3.Lerp(peak, end, curveValue);
                luggage.position = Vector3.Lerp(a, b, curveValue);
                yield return null;
            }

            // Snap to final position (simulate landing on truck)
            luggage.position = end;
            luggage.rotation = Quaternion.identity;

            // ✅ Parent luggage to finalParent (e.g. truck)
            if (finalParent != null)
                luggage.SetParent(finalParent);

            // Notify the system this luggage is delivered
            EventBus.Publish(new GameEvents.LuggagePushed(luggage));

            _isPushing = false;
        }
    }
}