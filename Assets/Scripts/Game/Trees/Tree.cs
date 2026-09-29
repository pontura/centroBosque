using UnityEngine;

namespace TreesGame
{
    public class Tree : MonoBehaviour
    {
        [HideInInspector] public Obstacle[] obstacles;
        Renderer rend;

        void Awake()
        {
            obstacles = GetComponentsInChildren<Obstacle>();

            // El renderer del tronco: el primero que no pertenezca a un Obstacle
            foreach (Renderer r in GetComponentsInChildren<Renderer>())
            {
                if (r.GetComponentInParent<Obstacle>() == null)
                {
                    rend = r;
                    break;
                }
            }
        }

        // X del borde del tronco del lado indicado (-1 izquierda, 1 derecha)
        public float GetSideX(int side)
        {
            if (rend == null)
                return transform.position.x;
            return side < 0 ? rend.bounds.min.x : rend.bounds.max.x;
        }

        public bool HitsObstacle(Collider2D c)
        {
            foreach (Obstacle o in obstacles)
                if (o.Overlaps(c))
                    return true;
            return false;
        }
    }
}
