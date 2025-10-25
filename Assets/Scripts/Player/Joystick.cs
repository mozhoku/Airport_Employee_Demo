using UnityEngine;
using UnityEngine.EventSystems;

namespace Player
{
    public class Joystick : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        [Header("Joystick References")] [SerializeField]
        private RectTransform handle;

        [SerializeField] private RectTransform background;

        [Header("Settings")] [SerializeField] private float handleRange = 100f;

        private Vector2 _inputVector;

        public float Horizontal => _inputVector.x;
        public float Vertical => _inputVector.y;

        public Vector2 Direction => _inputVector;

        public void OnPointerDown(PointerEventData eventData)
        {
            OnDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                background,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 position);

            position = Vector2.ClampMagnitude(position / (background.sizeDelta / 2f), 1f);
            _inputVector = position;

            handle.anchoredPosition = new Vector2(_inputVector.x * handleRange, _inputVector.y * handleRange);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _inputVector = Vector2.zero;
            handle.anchoredPosition = Vector2.zero;
        }
    }
}