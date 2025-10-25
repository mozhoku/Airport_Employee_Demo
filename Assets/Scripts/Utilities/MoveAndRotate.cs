using UnityEngine;

namespace Utilities
{
    public class MoveAndRotate : MonoBehaviour
    {
        [Header("Vertical Movement")]
        [SerializeField] private float moveAmplitude = 0.5f;   // height range
        [SerializeField] private float moveSpeed = 2f;         // up/down speed

        [Header("Rotation")]
        [SerializeField] private float rotationSpeed = 50f;    // degrees per second

        private Vector3 _startPos;

        private void Start()
        {
            _startPos = transform.position;
        }

        private void Update()
        {
            // vertical bobbing motion
            float newY = _startPos.y + Mathf.Sin(Time.time * moveSpeed) * moveAmplitude;
            transform.position = new Vector3(_startPos.x, newY, _startPos.z);

            // continuous rotation around Y axis
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.Self);
        }
    }
}