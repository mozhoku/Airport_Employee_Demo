using UnityEngine;

namespace Core
{
    [RequireComponent(typeof(Collider))]
    public class AreaTrigger : MonoBehaviour
    {
        private enum AreaEventType
        {
            None,
            AreaUnlockEvent,
            ReceptionEvent,
            LuggageDropAreaEvent,
            XRay1AreaEvent,
            XRay2AreaEvent,
            PaintingAreaEvent,
        }

        [Header("Event Settings")] [SerializeField]
        private AreaEventType eventType = AreaEventType.None;

        [SerializeField] private bool triggerOnce = true;

        private bool _triggered;

        private void Reset()
        {
            // make sure collider is trigger
            var col = GetComponent<Collider>();
            col.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            if (eventType == AreaEventType.AreaUnlockEvent)
            {
                EventBus.Publish(new GameEvents.AreaUnlockEntered(playerEntered: true));
            }
            else if (eventType == AreaEventType.ReceptionEvent)
            {
                EventBus.Publish(new GameEvents.ReceptionEntered(playerEntered: true,
                    gameObject.GetComponent<Transform>()));
            }
            else if (eventType == AreaEventType.LuggageDropAreaEvent)
            {
                EventBus.Publish(new GameEvents.LuggageDropAreaEntered(playerEntered: true));
            }
            else if (eventType == AreaEventType.XRay1AreaEvent)
            {
                EventBus.Publish(new GameEvents.XRay1AreaEntered(playerEntered: true));
            }
            else if (eventType == AreaEventType.XRay2AreaEvent)
            {
                EventBus.Publish(new GameEvents.XRay2AreaEntered(playerEntered: true));
            }
            else if (eventType == AreaEventType.PaintingAreaEvent)
            {
                EventBus.Publish(new GameEvents.PaintingAreaEntered(playerEntered: true));
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            if (eventType == AreaEventType.AreaUnlockEvent)
            {
                EventBus.Publish(new GameEvents.AreaUnlockExited(playerExited: true));
            }
            else if (eventType == AreaEventType.ReceptionEvent)
            {
                EventBus.Publish(new GameEvents.ReceptionExited(playerExited: true));
            }
            else if (eventType == AreaEventType.LuggageDropAreaEvent)
            {
                EventBus.Publish(new GameEvents.LuggageDropAreaExited(playerExited: true));
            }
            else if (eventType == AreaEventType.XRay1AreaEvent)
            {
                EventBus.Publish(new GameEvents.XRay1AreaExited(playerExited: true));
            }
            else if (eventType == AreaEventType.XRay2AreaEvent)
            {
                EventBus.Publish(new GameEvents.XRay2AreaExited(playerExited: true));
            }
        }
    }
}