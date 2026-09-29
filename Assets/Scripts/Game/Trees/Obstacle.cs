using UnityEngine;

namespace TreesGame
{
    public class Obstacle : MonoBehaviour
    {
        Collider2D[] colliders;

        void Awake()
        {
            colliders = GetComponentsInChildren<Collider2D>();
        }

        // Compara la forma real de los colliders (box, circle, polygon, etc)
        public bool Overlaps(Collider2D other)
        {
            foreach (Collider2D c in colliders)
                if (c.enabled && c.Distance(other).isOverlapped)
                    return true;
            return false;
        }
    }
}
