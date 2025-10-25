using System.Collections;
using Core;
using UnityEngine;

namespace NPC
{
    public class NPCGiveLuggage : MonoBehaviour
    {
        [SerializeField] private Transform luggage;
        [SerializeField] private NPCController npcController;
        [SerializeField] private float delayBeforeGive = 0.5f;

        private bool _hasGivenLuggage;
        private bool _playerInReception;
        private Coroutine _giveRoutine;

        private void OnEnable()
        {
            EventBus.Subscribe<GameEvents.ReceptionEntered>(OnReceptionEntered);
            EventBus.Subscribe<GameEvents.ReceptionExited>(OnReceptionExited);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<GameEvents.ReceptionEntered>(OnReceptionEntered);
            EventBus.Unsubscribe<GameEvents.ReceptionExited>(OnReceptionExited);

            if (_giveRoutine != null)
            {
                StopCoroutine(_giveRoutine);
                _giveRoutine = null;
            }
        }

        private void OnReceptionEntered(GameEvents.ReceptionEntered e)
        {
            if (!e.PlayerEntered || _hasGivenLuggage)
                return;

            _playerInReception = true;
            _giveRoutine = StartCoroutine(GiveLuggageRoutine());
        }

        private void OnReceptionExited(GameEvents.ReceptionExited e)
        {
            _playerInReception = false;
            _hasGivenLuggage = false;

            if (_giveRoutine != null)
            {
                StopCoroutine(_giveRoutine);
                _giveRoutine = null;
            }
        }

        private IEnumerator GiveLuggageRoutine()
        {
            yield return new WaitForSeconds(delayBeforeGive);

            // Double-check player is still in reception before giving
            if (!_playerInReception)
                yield break;

            _hasGivenLuggage = true;
            EventBus.Publish(new GameEvents.LuggageGiven(luggage, npcController));
        }
    }
}