using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;

namespace NPC
{
    public class NPCManager : MonoBehaviour
    {
        [Header("References")] [SerializeField]
        private GameObject npcPrefab;

        [SerializeField] private NPCPath npcPath;
        [SerializeField] private Transform spawnPoint;

        [Header("Settings")] [SerializeField] private int npcCount = 3;
        [SerializeField] private float spawnDelay = 0.5f;
        [SerializeField] private float queueSpacing = 1.2f;

        private readonly List<NPCController> _npcs = new();

        private void OnEnable()
        {
            EventBus.Subscribe<GameEvents.AreaUnlockCompleted>(OnAreaUnlockCompleted);
            EventBus.Subscribe<GameEvents.NPCReachedCheckpoint>(OnNPCReachedCheckpoint);
            EventBus.Subscribe<GameEvents.LuggageTaken>(OnLuggageTaken);
            EventBus.Subscribe<GameEvents.ElevatorRideStarted>(OnElevatorRideStarted);
            EventBus.Subscribe<GameEvents.ElevatorRideEnded>(OnElevatorRideEnded);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<GameEvents.AreaUnlockCompleted>(OnAreaUnlockCompleted);
            EventBus.Unsubscribe<GameEvents.NPCReachedCheckpoint>(OnNPCReachedCheckpoint);
            EventBus.Unsubscribe<GameEvents.LuggageTaken>(OnLuggageTaken);
            EventBus.Unsubscribe<GameEvents.ElevatorRideStarted>(OnElevatorRideStarted);
            EventBus.Unsubscribe<GameEvents.ElevatorRideEnded>(OnElevatorRideEnded);
        }

        private void OnElevatorRideStarted(GameEvents.ElevatorRideStarted e)
        {
            var npc = e.Rider.GetComponent<NPCController>();
            if (npc != null)
            {
                npc.PauseForElevator();
            }
        }

        private void OnElevatorRideEnded(GameEvents.ElevatorRideEnded e)
        {
            var npc = e.Rider.GetComponent<NPCController>();
            if (npc != null)
            {
                // Optional: If the elevator goes to a new path segment, pass it in
                npc.ResumeAfterElevator();
            }
        }


        private void OnAreaUnlockCompleted(GameEvents.AreaUnlockCompleted e)
        {
            if (e.IsCompleted)
                StartCoroutine(SpawnNPCs());
        }

        private IEnumerator SpawnNPCs()
        {
            for (int i = 0; i < npcCount; i++)
            {
                var npcObj = Instantiate(npcPrefab, spawnPoint.position, Quaternion.identity);
                var controller = npcObj.GetComponent<NPCController>();
                controller.AssignPath(npcPath);
                _npcs.Add(controller);
                yield return new WaitForSeconds(spawnDelay);
            }
        }

        private void OnNPCReachedCheckpoint(GameEvents.NPCReachedCheckpoint e)
        {
            var waypoint = e.Waypoint;
            if (!waypoint.IsCheckpoint) return; //check if checkpoint

            var queue = waypoint.ActiveQueue;
            queue.Enqueue(e.Controller);

            UpdateCheckpointQueue(waypoint);

            // If NPC is first, begin processing
            if (queue.Count == 1)
                StartCoroutine(HandleCheckpointNPC(waypoint, e.Controller));
        }

        private IEnumerator HandleCheckpointNPC(NPCWaypoint waypoint, NPCController npc)
        {
            // Wait until NPC reaches checkpoint
            yield return new WaitUntil(() =>
                Vector3.Distance(npc.transform.position, waypoint.Position) < 0.1f);

            // Wait until resumed externally
            yield return new WaitUntil(() => npc.IsResumed);

            if (waypoint.ActiveQueue.Count > 0)
                waypoint.ActiveQueue.Dequeue();

            UpdateCheckpointQueue(waypoint);

            if (waypoint.ActiveQueue.Count > 0)
                StartCoroutine(HandleCheckpointNPC(waypoint, waypoint.ActiveQueue.Peek()));
        }

        private void OnLuggageTaken(GameEvents.LuggageTaken e)
        {
            e.Controller.Resume();
        }

        private void UpdateCheckpointQueue(NPCWaypoint waypoint)
        {
            Vector3 basePos = waypoint.Position;
            Vector3 backDir = -waypoint.transform.forward.normalized;
            int i = 0;
            NPCController npcInFront = null;

            foreach (var npc in waypoint.ActiveQueue)
            {
                Vector3 targetPos = basePos + backDir * (i * queueSpacing);
                npc.SetQueueTarget(targetPos, npcInFront);
                npcInFront = npc;
                i++;
            }
        }
    }
}