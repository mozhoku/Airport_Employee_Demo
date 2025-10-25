using UnityEngine;

namespace Player
{
    [ExecuteAlways]
    public class MovementArea : MonoBehaviour
    {
        [Header("Polygon Points (Clockwise or Counter-clockwise)")]
        public Transform[] points;

        // Draw the polygon in the editor
        private void OnDrawGizmos()
        {
            if (points == null || points.Length < 3) return;

            Gizmos.color = Color.green;
            for (int i = 0; i < points.Length; i++)
            {
                Vector3 a = points[i].position;
                Vector3 b = points[(i + 1) % points.Length].position;
                Gizmos.DrawLine(a, b);
            }
        }

        // Point-in-polygon check using raycast method
        public bool Contains(Vector3 position)
        {
            if (points == null || points.Length < 3) return false;

            bool inside = false;
            int j = points.Length - 1;
            for (int i = 0; i < points.Length; j = i++)
            {
                Vector3 pi = points[i].position;
                Vector3 pj = points[j].position;

                if (((pi.z > position.z) != (pj.z > position.z)) &&
                    (position.x < (pj.x - pi.x) * (position.z - pi.z) / (pj.z - pi.z) + pi.x))
                {
                    inside = !inside;
                }
            }

            return inside;
        }

        // Clamp a point to the nearest point inside the polygon (simplified as bounding box)
        public Vector3 ClampToBounds(Vector3 position)
        {
            if (points == null || points.Length < 3) return position;

            float minX = float.MaxValue, maxX = float.MinValue;
            float minZ = float.MaxValue, maxZ = float.MinValue;

            foreach (var p in points)
            {
                minX = Mathf.Min(minX, p.position.x);
                maxX = Mathf.Max(maxX, p.position.x);
                minZ = Mathf.Min(minZ, p.position.z);
                maxZ = Mathf.Max(maxZ, p.position.z);
            }

            position.x = Mathf.Clamp(position.x, minX, maxX);
            position.z = Mathf.Clamp(position.z, minZ, maxZ);
            return position;
        }
    }
}