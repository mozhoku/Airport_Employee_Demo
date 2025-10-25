using System.Collections.Generic;
using UnityEngine;

namespace NPC
{
    public class NPCWaypoint : MonoBehaviour
    {
        [SerializeField] private bool isCheckpoint;
        [SerializeField] private int queueSlots = 1;

        public bool IsCheckpoint => isCheckpoint;
        public int QueueSlots => Mathf.Max(1, queueSlots);
        public Vector3 Position => transform.position;

        // NEW: local checkpoint queue
        [HideInInspector] public readonly Queue<NPCController> ActiveQueue = new();
        public bool IsBusy => ActiveQueue.Count > 0;

        private void OnDrawGizmos()
        {
            Gizmos.color = isCheckpoint ? Color.red : Color.green;
            Gizmos.DrawSphere(transform.position, 0.2f);
        }
    }
}