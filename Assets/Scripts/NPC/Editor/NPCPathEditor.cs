#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace NPC.Editor
{
    [CustomEditor(typeof(NPCPath))]
    public class NPCPathEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            NPCPath path = (NPCPath)target;

            GUILayout.Space(10);

            if (GUILayout.Button("Add Waypoint"))
            {
                Undo.RecordObject(path, "Add Waypoint");
                path.AddWaypoint();
                EditorUtility.SetDirty(path);
            }

            if (GUILayout.Button("Snap Waypoints to Ground"))
            {
                Undo.RecordObject(path, "Snap Waypoints to Ground");
                SnapWaypointsToGround(path);
                EditorUtility.SetDirty(path);
            }
        }

        private void SnapWaypointsToGround(NPCPath path)
        {
            foreach (var wp in path.waypoints)
            {
                if (wp == null) continue;

                // Start ray from very high up
                Vector3 rayOrigin = new Vector3(wp.transform.position.x, 10000f, wp.transform.position.z);
                Ray ray = new Ray(rayOrigin, Vector3.down);

                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
                {
                    wp.transform.position = hit.point;
                }
                else
                {
                    Debug.LogWarning($"Waypoint '{wp.name}' did not hit anything when snapping to ground.");
                }
            }
        }
    }
}
#endif