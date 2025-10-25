using System.Collections;
using System.Collections.Generic;
using Core;
using NPC;
using UnityEngine;

namespace Player
{
    public class PlayerLuggageHandler : MonoBehaviour
    {
        [Header("Carry Settings")] [SerializeField]
        private Transform luggageStackRoot;

        [SerializeField] private float stackHeight = 0.6f;
        [SerializeField] private float pickupDelay = 0.4f;
        [SerializeField] private float attachSpeed = 4f;

        [Header("Drop Settings")] [SerializeField]
        private Transform dropSpot;

        [SerializeField] private float dropInterval = 0.5f;

        private readonly Queue<Transform> _pendingLuggages = new();
        private readonly List<Transform> _carriedLuggages = new();

        private Coroutine _pickupRoutine;
        private Coroutine _dropRoutine;

        private bool _playerInReception;
        private bool _inDropArea;

        #region Unity Events

        private void OnEnable()
        {
            // Pickup-related
            EventBus.Subscribe<GameEvents.ReceptionEntered>(OnReceptionEntered);
            EventBus.Subscribe<GameEvents.ReceptionExited>(OnReceptionExited);
            EventBus.Subscribe<GameEvents.LuggageGiven>(OnLuggageReady);

            // Drop-related
            EventBus.Subscribe<GameEvents.LuggageDropAreaEntered>(OnDropAreaEntered);
            EventBus.Subscribe<GameEvents.LuggageDropAreaExited>(OnDropAreaExited);
        }

        private void OnDisable()
        {
            // Pickup
            EventBus.Unsubscribe<GameEvents.ReceptionEntered>(OnReceptionEntered);
            EventBus.Unsubscribe<GameEvents.ReceptionExited>(OnReceptionExited);
            EventBus.Unsubscribe<GameEvents.LuggageGiven>(OnLuggageReady);

            // Drop
            EventBus.Unsubscribe<GameEvents.LuggageDropAreaEntered>(OnDropAreaEntered);
            EventBus.Unsubscribe<GameEvents.LuggageDropAreaExited>(OnDropAreaExited);

            if (_pickupRoutine != null)
            {
                StopCoroutine(_pickupRoutine);
                _pickupRoutine = null;
            }

            if (_dropRoutine != null)
            {
                StopCoroutine(_dropRoutine);
                _dropRoutine = null;
            }
        }

        #endregion

        #region Pickup Logic

        private void OnReceptionEntered(GameEvents.ReceptionEntered e)
        {
            _playerInReception = true;
        }

        private void OnReceptionExited(GameEvents.ReceptionExited e)
        {
            _playerInReception = false;
        }

        private void OnLuggageReady(GameEvents.LuggageGiven e)
        {
            // Only accept luggage if player is currently in the reception area
            if (!_playerInReception || e.Luggage == null)
                return;

            _pendingLuggages.Enqueue(e.Luggage);

            // Start processing if not already doing so
            if (_pickupRoutine == null)
                _pickupRoutine = StartCoroutine(PickupRoutine());
        }

        private IEnumerator PickupRoutine()
        {
            while (_pendingLuggages.Count > 0)
            {
                var luggage = _pendingLuggages.Dequeue();
                var npc = luggage.GetComponentInParent<NPCController>();

                yield return StartCoroutine(MoveLuggageToPlayer(luggage));

                _carriedLuggages.Add(luggage);

                // Notify NPC that luggage was taken
                EventBus.Publish(new GameEvents.LuggageTaken(npc.transform, npc));

                // Attach luggage to player's stack
                luggage.SetParent(luggageStackRoot);
                luggage.localPosition = Vector3.up * (stackHeight * (_carriedLuggages.Count - 1));
                luggage.localRotation = Quaternion.Euler(0, 0, 90);

                yield return new WaitForSeconds(pickupDelay);
            }

            _pickupRoutine = null;
        }

        private IEnumerator MoveLuggageToPlayer(Transform luggage)
        {
            Vector3 start = luggage.position;
            Quaternion startRot = luggage.rotation;
            Vector3 end = luggageStackRoot.position + Vector3.up * (stackHeight * _carriedLuggages.Count);

            float elapsed = 0f;
            float duration = 1f / attachSpeed;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                luggage.position = Vector3.Lerp(start, end, t);
                luggage.rotation = Quaternion.Slerp(startRot, Quaternion.Euler(0, 0, 90), t);
                yield return null;
            }

            luggage.position = end;
        }

        #endregion

        #region Drop Logic

        private void OnDropAreaEntered(GameEvents.LuggageDropAreaEntered e)
        {
            _inDropArea = true;
            if (_dropRoutine == null)
                _dropRoutine = StartCoroutine(DropRoutine());
        }

        private void OnDropAreaExited(GameEvents.LuggageDropAreaExited e)
        {
            _inDropArea = false;
            if (_dropRoutine != null)
            {
                StopCoroutine(_dropRoutine);
                _dropRoutine = null;
            }
        }


        private IEnumerator DropRoutine()
        {
            // Drop from top to bottom (reverse order)
            int droppedCount = 0;

            while (_inDropArea && _carriedLuggages.Count > 0)
            {
                var luggage = PopTopLuggage();
                if (luggage == null)
                    break;

                // Compute drop target stacking position
                Vector3 dropTarget = dropSpot.position + Vector3.up * (stackHeight * droppedCount);

                // Smoothly move luggage from stack to drop target
                yield return StartCoroutine(MoveLuggageToDropSpot(luggage, dropTarget));

                // Parent to drop spot (optional, helps for visual alignment)
                luggage.SetParent(dropSpot);

                // Notify pedestal / xray systems
                EventBus.Publish(new GameEvents.LuggageDropped(luggage));

                droppedCount++;
                yield return new WaitForSeconds(dropInterval);
            }

            _dropRoutine = null;
        }

        // Smooth drop movement
        private IEnumerator MoveLuggageToDropSpot(Transform luggage, Vector3 targetPosition)
        {
            Vector3 startPos = luggage.position;
            Quaternion startRot = luggage.rotation;
            Quaternion endRot = Quaternion.Euler(0, 0, 90);

            float elapsed = 0f;
            float duration = 0.5f; // drop time
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);

                luggage.position = Vector3.Lerp(startPos, targetPosition, t);
                luggage.rotation = Quaternion.Slerp(startRot, endRot, t);

                yield return null;
            }

            luggage.position = targetPosition;
            luggage.rotation = endRot;
        }

        public Transform PopTopLuggage()
        {
            if (_carriedLuggages.Count == 0)
                return null;

            var top = _carriedLuggages[^1];
            _carriedLuggages.RemoveAt(_carriedLuggages.Count - 1);
            return top;
        }

        public int CarriedCount => _carriedLuggages.Count;

        #endregion

        #region Utility

        public void DropAllLuggages()
        {
            foreach (var lug in _carriedLuggages)
                lug.SetParent(null);

            _carriedLuggages.Clear();
        }

        #endregion
    }
}