using System.Collections;
using Core;
using UI;
using UnityEngine;

namespace Player
{
    public class CameraFollow : MonoBehaviour
    {
        [Header("Follow Settings")]
        [SerializeField] private Transform playerTarget;
        [SerializeField] private Vector3 followOffset = new Vector3(0, 10, -10);
        [SerializeField] private float followSpeed = 5f;

        [Header("Translation Settings")]
        [SerializeField] private Transform targetTransform;
        [SerializeField] private float moveDuration = 1.5f;
        [SerializeField] private float holdDuration = 1f;

        private bool _isFollowing = true;
        private Coroutine _moveRoutine;

        private void OnEnable()
        {
            EventBus.Subscribe<UIEvents.UIFillComplete>(OnMoveToTarget);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<UIEvents.UIFillComplete>(OnMoveToTarget);
            if (_moveRoutine != null)
            {
                StopCoroutine(_moveRoutine);
                _moveRoutine = null;
            }
        }

        private void LateUpdate()
        {
            if (!_isFollowing || playerTarget == null)
                return;

            Vector3 desiredPos = playerTarget.position + followOffset;
            transform.position = Vector3.Lerp(transform.position, desiredPos, followSpeed * Time.deltaTime);
            transform.LookAt(playerTarget);
        }

        private void OnMoveToTarget(UIEvents.UIFillComplete e)
        {
            if (_moveRoutine == null && targetTransform != null)
                _moveRoutine = StartCoroutine(TranslateToTargetAndBack());
        }

        private IEnumerator TranslateToTargetAndBack()
        {
            // Disable player movement
            _isFollowing = false;
            EventBus.Publish(new GameEvents.PlayerMovementEnabled(false));

            Vector3 startPos = transform.position;
            Quaternion startRot = transform.rotation;

            Vector3 endPos = targetTransform.position;
            Quaternion endRot = targetTransform.rotation;

            float elapsed = 0f;

            // 1️⃣ Move to target
            while (elapsed < moveDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / moveDuration);
                transform.position = Vector3.Lerp(startPos, endPos, t);
                transform.rotation = Quaternion.Slerp(startRot, endRot, t);
                yield return null;
            }

            yield return new WaitForSeconds(holdDuration);

            // 2️⃣ Move back *the same way* (reverse interpolation)
            elapsed = 0f;
            while (elapsed < moveDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / moveDuration);
                transform.position = Vector3.Lerp(endPos, startPos, t);
                transform.rotation = Quaternion.Slerp(endRot, startRot, t);
                yield return null;
            }

            // Resume follow & player control
            _isFollowing = true;
            EventBus.Publish(new GameEvents.PlayerMovementEnabled(true));
            EventBus.Publish(new GameEvents.AreaUnlockCompleted(isCompleted:true));
            _moveRoutine = null;
        }
    }
}
