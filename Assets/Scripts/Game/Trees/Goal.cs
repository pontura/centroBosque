using UnityEngine;

namespace TreesGame
{
    public class Goal : MonoBehaviour
    {
        public Transform characterPosition; // donde queda el character al llegar
        public float freezeTime = 2f;

        Collider2D[] colliders;

        void Awake()
        {
            colliders = GetComponentsInChildren<Collider2D>();
        }

        public bool Overlaps(Collider2D other)
        {
            foreach (Collider2D c in colliders)
                if (c.enabled && c.Distance(other).isOverlapped)
                    return true;
            return false;
        }

        public Vector3 GetCharacterPosition()
        {
            return characterPosition != null ? characterPosition.position : transform.position;
        }
    }
}
