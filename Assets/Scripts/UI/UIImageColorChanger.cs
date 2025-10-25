using System;
using Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIImageColorChanger : MonoBehaviour
    {
        private enum AreaType
        {
            None,
            Reception,
            LuggageDrop,
            XRay1,
            XRay2
        }

        [Header("Settings")] [SerializeField] private bool isEnabled = true;
        [SerializeField] private AreaType areaType = AreaType.None;
        [SerializeField] private Image targetImage;

        [Header("Colors")] [SerializeField] private Color defaultColor = Color.white;
        [SerializeField] private Color activeColor = Color.green;

        private void Awake()
        {
            if (targetImage != null)
            {
                targetImage.color = defaultColor;
            }
        }

        private void OnEnable()
        {
            if (areaType == AreaType.Reception)
            {
                EventBus.Subscribe<GameEvents.ReceptionEntered>((Action<object>)OnPlayerEnter);
                EventBus.Subscribe<GameEvents.ReceptionExited>((Action<object>)OnPlayerExit);
            }
            else if (areaType == AreaType.LuggageDrop)
            {
                EventBus.Subscribe<GameEvents.LuggageDropAreaEntered>((Action<object>)OnPlayerEnter);
                EventBus.Subscribe<GameEvents.LuggageDropAreaExited>((Action<object>)OnPlayerExit);
            }
            else if (areaType == AreaType.XRay1)
            {
                EventBus.Subscribe<GameEvents.XRay1AreaEntered>((Action<object>)OnPlayerEnter);
                EventBus.Subscribe<GameEvents.XRay1AreaExited>((Action<object>)OnPlayerExit);
            }
            else if (areaType == AreaType.XRay2)
            {
                EventBus.Subscribe<GameEvents.XRay2AreaEntered>((Action<object>)OnPlayerEnter);
                EventBus.Subscribe<GameEvents.XRay2AreaExited>((Action<object>)OnPlayerExit);
            }
        }

        private void OnDisable()
        {
            if (areaType == AreaType.Reception)
            {
                EventBus.Unsubscribe<GameEvents.ReceptionEntered>((Action<object>)OnPlayerEnter);
                EventBus.Unsubscribe<GameEvents.ReceptionExited>((Action<object>)OnPlayerExit);
            }
            else if (areaType == AreaType.LuggageDrop)
            {
                EventBus.Unsubscribe<GameEvents.LuggageDropAreaEntered>((Action<object>)OnPlayerEnter);
                EventBus.Unsubscribe<GameEvents.LuggageDropAreaExited>((Action<object>)OnPlayerExit);
            }
            else if (areaType == AreaType.XRay1)
            {
                EventBus.Unsubscribe<GameEvents.XRay1AreaEntered>((Action<object>)OnPlayerEnter);
                EventBus.Unsubscribe<GameEvents.XRay1AreaExited>((Action<object>)OnPlayerExit);
            }
            else if (areaType == AreaType.XRay2)
            {
                EventBus.Unsubscribe<GameEvents.XRay2AreaEntered>((Action<object>)OnPlayerEnter);
                EventBus.Unsubscribe<GameEvents.XRay2AreaExited>((Action<object>)OnPlayerExit);
            }
        }

        private void OnPlayerEnter(object e)
        {
            ChangeColor(true);
        }

        private void OnPlayerExit(object e)
        {
            ChangeColor(false);
        }


        public void ChangeColor(bool changeToTarget)
        {
            if (targetImage == null) return;
            if (changeToTarget)
            {
                targetImage.color = activeColor;
            }
            else
            {
                targetImage.color = defaultColor;
            }
        }
    }
}