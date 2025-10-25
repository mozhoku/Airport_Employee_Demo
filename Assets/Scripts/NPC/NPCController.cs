using System.Collections;
using Core;
using UnityEngine;

namespace NPC
{
    public class NPCController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private float queueSpacing = 1.2f;

        private NPCPath _path;
        private int _currentIndex;
        private bool _isMoving;
        private bool _isWaiting;
        private Vector3? _queueTarget;

        private NPCController _npcInFront;

        private bool _isOnElevator;
        private bool _isPausedForElevator;
        public bool IsResumed { get; private set; }

        public NPCWaypoint CurrentWaypoint =>
            _path != null && _currentIndex < _path.waypoints.Count
                ? _path.GetWaypoint(_currentIndex)
                : null;

        public void AssignPath(NPCPath path)
        {
            _path = path;
            _currentIndex = 0;
            _isMoving = true;
            StartCoroutine(MoveRoutine());
        }

        private IEnumerator MoveRoutine()
        {
            while (_path != null && _currentIndex < _path.waypoints.Count)
            {
                var target = _path.GetWaypoint(_currentIndex);

                // Movement loop
                while (true)
                {
                    // ✅ always re-evaluate target position in case of queue updates
                    var targetPos = _queueTarget ?? target.Position;
                    Vector3 dir = (targetPos - transform.position).normalized;

                    // Check if too close to the NPC in front
                    if (_npcInFront != null)
                    {
                        float dist = Vector3.Distance(transform.position, _npcInFront.transform.position);
                        if (dist < queueSpacing)
                        {
                            yield return null; // wait until the one in front moves
                            continue;
                        }
                    }

                    transform.position += dir * moveSpeed * Time.deltaTime;
                    if (dir != Vector3.zero)
                        transform.rotation = Quaternion.LookRotation(dir);

                    if (Vector3.Distance(transform.position, targetPos) < 0.05f)
                        break;

                    yield return null;
                }

                // Handle checkpoint wait
                if (target.IsCheckpoint)
                {
                    _isWaiting = true;
                    IsResumed = false;
                    EventBus.Publish(new GameEvents.NPCReachedCheckpoint(this, target));
                    yield return new WaitUntil(() => !_isWaiting);
                }

                _queueTarget = null;
                _currentIndex++;
                yield return null;
            }
        }

        public void SetQueueTarget(Vector3 position, NPCController npcInFront = null)
        {
            _queueTarget = position;
            _npcInFront = npcInFront;
        }

        public void Resume()
        {
            IsResumed = true;
            _isWaiting = false;
        }


        public void PauseForElevator()
        {
            _isOnElevator = true;
            _isPausedForElevator = true;
            StopAllCoroutines(); // stop path movement
        }

        public void ResumeAfterElevator(NPCPath pathOverride = null)
        {
            if (_isOnElevator)
            {
                _isOnElevator = false;
                _isPausedForElevator = false;

                // Move to the next waypoint after the elevator entry
                _currentIndex = Mathf.Min(_currentIndex + 1, _path.waypoints.Count - 1);

                // Optional: update path if elevator leads to a new segment
                if (pathOverride != null)
                    _path = pathOverride;

                StartCoroutine(MoveRoutine());
            }
        }

    }
}