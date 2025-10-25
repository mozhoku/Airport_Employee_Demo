using Player;
using Utilities;

namespace Core
{
    public class GameManager : Singleton<GameManager>
    {
        public MovementArea[] floorAreas;

        public enum GameState
        {
            Menu,
            Gameplay,
            Painting,
            Completed
        }

        public GameState CurrentState { get; private set; }

        protected void Awake()
        {
            CurrentState = GameState.Menu;
        }

        public void StartGame()
        {
            CurrentState = GameState.Gameplay;
            EventBus.Publish(new GameEvents.GameStarted());
        }

        public void EnterPaintingMode()
        {
            CurrentState = GameState.Painting;
            EventBus.Publish(new GameEvents.PaintingStarted());
        }

        public void EndGame()
        {
            CurrentState = GameState.Completed;
            EventBus.Publish(new GameEvents.GameCompleted());
        }
    }
}