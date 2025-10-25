using Core;
using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")] [SerializeField]
        private float moveSpeed = 5f;

        [SerializeField] private float rotationSpeed = 10f;
        [SerializeField] private MovementArea[] floors;
        private MovementArea _currentFloor;


        [Header("References")] [SerializeField]
        private Joystick joystick;

        [SerializeField] private PlayerAnimation playerAnimation;

        private Camera _mainCamera;
        private bool _canMove = true;
        private Vector3 _pendingMotion; // from elevator

        private void Awake() => _mainCamera = Camera.main;

        private void Start()
        {
            // Start with first area
            if (floors.Length > 0)
                _currentFloor = floors[0];
        }

        private void OnEnable()
        {
            EventBus.Subscribe<GameEvents.PlayerMovementEnabled>(OnMovementToggle);

            EventBus.Subscribe<GameEvents.ElevatorRideStarted>(OnRideStart);
            EventBus.Subscribe<GameEvents.ElevatorRideUpdated>(OnRideUpdate);
            EventBus.Subscribe<GameEvents.ElevatorRideEnded>(OnRideEnd);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<GameEvents.PlayerMovementEnabled>(OnMovementToggle);

            EventBus.Unsubscribe<GameEvents.ElevatorRideStarted>(OnRideStart);
            EventBus.Unsubscribe<GameEvents.ElevatorRideUpdated>(OnRideUpdate);
            EventBus.Unsubscribe<GameEvents.ElevatorRideEnded>(OnRideEnd);
        }


        private void Update()
        {
            Vector3 move = Vector3.zero;

            if (_canMove)
            {
                float h = joystick.Horizontal;
                float v = joystick.Vertical;

                Vector3 camF = _mainCamera.transform.forward;
                Vector3 camR = _mainCamera.transform.right;
                camF.y = 0;
                camR.y = 0;
                camF.Normalize();
                camR.Normalize();

                Vector3 moveDir = (camF * v + camR * h).normalized;
                playerAnimation.UpdateAnimation(moveDir.magnitude > 0.1f);

                if (moveDir.sqrMagnitude > 0.01f)
                {
                    move += moveDir * moveSpeed * Time.deltaTime;
                    Quaternion rot = Quaternion.LookRotation(moveDir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotationSpeed * Time.deltaTime);
                }
            }

            move += _pendingMotion;
            transform.position += move;
            _pendingMotion = Vector3.zero;

            // Clamp to current movement area
            if (_currentFloor != null)
                transform.position = _currentFloor.ClampToBounds(transform.position);
        }

        private void OnMovementToggle(GameEvents.PlayerMovementEnabled e)
        {
            _canMove = e.Enabled;
        }

        private void OnRideStart(GameEvents.ElevatorRideStarted e)
        {
            if (e.Rider == transform)
                _canMove = false;
        }

        private void OnRideUpdate(GameEvents.ElevatorRideUpdated e)
        {
            if (e.Rider == transform)
            {
                _pendingMotion += e.DeltaMotion;
                _currentFloor = floors[e.CurrentFloor];
            }
        }

        private void OnRideEnd(GameEvents.ElevatorRideEnded e)
        {
            if (e.Rider == transform)
            {
                _canMove = true;
                _currentFloor = floors[e.CurrentFloor];
                _pendingMotion = Vector3.zero; // clear leftover delta
            }
        }
    }
}