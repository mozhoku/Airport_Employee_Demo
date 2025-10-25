using System.Collections.Generic;
using UnityEngine;

namespace NPC
{
    public class NPCPath : MonoBehaviour
    {
        [Header("Waypoint Settings")] public List<NPCWaypoint> waypoints = new();
        public GameObject waypointPrefab;
        public float waypointSpacing = 2f;
        public Color gizmoColor = Color.green;
        public float gizmoSize = 0.3f;

        public NPCWaypoint GetWaypoint(int index)
        {
            if (index < 0 || index >= waypoints.Count) return null;
            return waypoints[index];
        }

        /// <summary>
        /// Adds a new waypoint at the last waypoint's position + forward spacing
        /// </summary>
        public void AddWaypoint()
        {
            Vector3 position = transform.position;

            if (waypoints.Count > 0)
            {
                position = waypoints[waypoints.Count - 1].transform.position + Vector3.forward * waypointSpacing;
            }

            GameObject wpObj;
            if (waypointPrefab != null)
            {
                wpObj = Instantiate(waypointPrefab, position, Quaternion.identity, transform);
            }
            else
            {
                wpObj = new GameObject("Waypoint " + (waypoints.Count + 1));
                wpObj.transform.parent = transform;
                wpObj.transform.position = position;
                wpObj.AddComponent<NPCWaypoint>();
            }

            NPCWaypoint wp = wpObj.GetComponent<NPCWaypoint>();
            waypoints.Add(wp);
        }

        // vizualize waypoints and connections in editor
        private void OnDrawGizmos()
        {
            for (int i = 0; i < waypoints.Count; i++)
            {
                if (waypoints[i] == null) continue;

                // Draw waypoint sphere
                Gizmos.color = gizmoColor;
                Gizmos.DrawSphere(waypoints[i].transform.position, gizmoSize);

                // Draw line & arrow to next waypoint
                if (i < waypoints.Count - 1 && waypoints[i + 1] != null)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawLine(waypoints[i].transform.position, waypoints[i + 1].transform.position);

                    // Arrow
                    Vector3 direction = (waypoints[i + 1].transform.position - waypoints[i].transform.position)
                        .normalized;
                    float arrowHeadLength = 0.3f;
                    float arrowHeadAngle = 20f;

                    Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) *
                                    Vector3.forward;
                    Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) *
                                   Vector3.forward;

                    Gizmos.DrawLine(waypoints[i + 1].transform.position,
                        waypoints[i + 1].transform.position + right * arrowHeadLength);
                    Gizmos.DrawLine(waypoints[i + 1].transform.position,
                        waypoints[i + 1].transform.position + left * arrowHeadLength);
                }
            }
        }
    }
}