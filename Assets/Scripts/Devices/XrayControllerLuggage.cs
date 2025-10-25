using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;

namespace Devices
{
    public class XrayControllerLuggage : MonoBehaviour
    {
        private readonly List<Transform> _luggages = new();
        private bool _inXrayArea;

        private void OnEnable()
        {
            EventBus.Subscribe<GameEvents.LuggageDropped>(OnLuggageDropped);
            EventBus.Subscribe<GameEvents.LuggagePushed>(OnLuggagePushed);
            EventBus.Subscribe<GameEvents.XRay1AreaEntered>(OnXrayEntered);
            EventBus.Subscribe<GameEvents.XRay1AreaExited>(OnXrayExited);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<GameEvents.LuggageDropped>(OnLuggageDropped);
            EventBus.Unsubscribe<GameEvents.LuggagePushed>(OnLuggagePushed);
            EventBus.Unsubscribe<GameEvents.XRay1AreaEntered>(OnXrayEntered);
            EventBus.Unsubscribe<GameEvents.XRay1AreaExited>(OnXrayExited);
        }

        private void OnLuggageDropped(GameEvents.LuggageDropped e)
        {
            _luggages.Add(e.Luggage);
        }

        private void OnXrayEntered(GameEvents.XRay1AreaEntered e)
        {
            _inXrayArea = e.PlayerEntered;
            StartCoroutine(XraySequence());
        }

        private void OnXrayExited(GameEvents.XRay1AreaExited e)
        {
            _inXrayArea = !e.PlayerExited;
        }

        private IEnumerator XraySequence()
        {
            foreach (var luggage in _luggages)
            {
                // You can publish an event here if pedestal is triggered externally
                EventBus.Publish(new GameEvents.LuggageDropped(luggage));
                yield return new WaitForSeconds(1f);
            }

            // Wait until all are pushed
            while (_luggages.Count > 0)
                yield return null;

            EventBus.Publish(new GameEvents.AllLuggagesScanned());
        }

        private void OnLuggagePushed(GameEvents.LuggagePushed e)
        {
            _luggages.Remove(e.Luggage);
        }
    }
}