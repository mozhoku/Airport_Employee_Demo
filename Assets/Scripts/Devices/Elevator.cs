using System.Collections.Generic;
using Core;
using UnityEngine;

namespace Devices
{
    [RequireComponent(typeof(BoxCollider))]
    public class Elevator : MonoBehaviour
    {
        [Header("Step Settings")] [SerializeField]
        private GameObject stepPrefab;

        [SerializeField] private int stepCount = 10;
        [SerializeField] private float moveSpeed = 2f;

        [Header("Path Points")] [SerializeField]
        private Transform startPoint;

        [SerializeField] private Transform endPoint;
        [SerializeField] private Transform exitPoint;

        [Header("Passenger Settings")] [SerializeField]
        private List<string> passengerTags = new() { "Player", "NPC" };

        private Step[] _steps;
        private Vector3 _localStart;
        private Vector3 _localEnd;
        private Vector3 _direction;
        private Vector3 _worldDirection;
        private float _pathLength;
        private readonly HashSet<Transform> _passengers = new();

        private struct Step
        {
            public Transform Transform;
            public float Progress;
        }

        private void Awake()
        {
            if (!startPoint || !endPoint || !stepPrefab)
            {
                Debug.LogError("Elevator is missing references!");
                enabled = false;
                return;
            }

            _localStart = startPoint.localPosition;
            _localEnd = endPoint.localPosition;
            _direction = (_localEnd - _localStart).normalized;
            _worldDirection = (endPoint.position - startPoint.position).normalized;
            _pathLength = Vector3.Distance(_localStart, _localEnd);

            // Initialize steps
            _steps = new Step[Mathf.Max(stepCount, 1)];
            float stepOffset = 1f / stepCount;

            for (int i = 0; i < stepCount; i++)
            {
                GameObject stepObj = Instantiate(stepPrefab, transform);
                _steps[i] = new Step
                {
                    Transform = stepObj.transform,
                    Progress = i * stepOffset
                };
                _steps[i].Transform.localPosition = Vector3.Lerp(_localStart, _localEnd, _steps[i].Progress);
            }

            var trigger = GetComponent<BoxCollider>();
            trigger.isTrigger = true;
        }

        private void Update()
        {
            MoveSteps();
            BroadcastMotion();
        }

        private void MoveSteps()
        {
            float delta = (moveSpeed / _pathLength) * Time.deltaTime;
            for (int i = 0; i < _steps.Length; i++)
            {
                Step step = _steps[i];
                step.Progress += delta;
                if (step.Progress > 1f) step.Progress -= 1f;
                step.Transform.localPosition = Vector3.Lerp(_localStart, _localEnd, step.Progress);
                _steps[i] = step;
            }
        }

        private void BroadcastMotion()
        {
            if (startPoint == null || endPoint == null) return;

            // Calculate world-space direction and frame delta
            Vector3 delta = _worldDirection * moveSpeed * Time.deltaTime;

            // Move each passenger along the elevator path
            foreach (var p in new List<Transform>(_passengers))
            {
                p.position += delta;

                // Check if passenger reached or passed the end
                float distToEnd = Vector3.Dot(endPoint.position - p.position, _worldDirection);
                if (distToEnd <= 0f && exitPoint != null)
                {
                    p.position = exitPoint.position; // Snap to exit
                    _passengers.Remove(p);
                    if (_worldDirection.y > 0)
                    {
                        EventBus.Publish(new GameEvents.ElevatorRideEnded(p, this, 2));
                    }
                    else
                    {
                        EventBus.Publish(new GameEvents.ElevatorRideEnded(p, this, 0));
                    }
                }
                else
                {
                    //Notify of movement for other systems
                    EventBus.Publish(new GameEvents.ElevatorRideUpdated(p, this, delta, 1));
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (passengerTags.Contains(other.tag))
            {
                _passengers.Add(other.transform);
                EventBus.Publish(new GameEvents.ElevatorRideStarted(other.transform, this));
            }
        }

        // ----- Debug visualization -----
        private void OnDrawGizmosSelected()
        {
            if (!startPoint || !endPoint) return;

            // Draw the path
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(startPoint.position, endPoint.position);

            // Draw _direction vector from startPoint
            if (Application.isPlaying)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(startPoint.position, startPoint.position + _direction * _pathLength);
            }

            // Draw distance to end for each passenger
            if (_passengers != null)
            {
                Gizmos.color = Color.cyan;
                foreach (var p in _passengers)
                {
                    float distToEnd = Vector3.Dot(endPoint.position - p.position, _worldDirection);
                    Vector3 endPos = p.position + _direction * distToEnd;
                    Gizmos.DrawLine(p.position, endPos);
                    Gizmos.DrawSphere(endPos, 0.1f);
                }
            }
        }
    }
}