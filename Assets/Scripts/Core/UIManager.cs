using UnityEngine;
using Utilities;

namespace Core
{
    public class UIManager : Singleton<UIManager>
    {
        [SerializeField] private GameObject gameplayUI;
        [SerializeField] private GameObject paintingUI;

        private void OnEnable()
        {
            EventBus.Subscribe<GameEvents.PaintingStarted>(OnPaintingStarted);
            EventBus.Subscribe<GameEvents.GameStarted>(OnGameStarted);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<GameEvents.PaintingStarted>(OnPaintingStarted);
            EventBus.Unsubscribe<GameEvents.GameStarted>(OnGameStarted);
        }

        private void OnGameStarted(GameEvents.GameStarted e)
        {
            gameplayUI.SetActive(true);
            paintingUI.SetActive(false);
        }

        private void OnPaintingStarted(GameEvents.PaintingStarted e)
        {
            gameplayUI.SetActive(false);
            paintingUI.SetActive(true);
        }
    }
}