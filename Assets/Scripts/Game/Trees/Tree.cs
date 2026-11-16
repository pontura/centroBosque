using UnityEngine;

namespace TreesGame
{
    public class Tree : MonoBehaviour
    {
        [HideInInspector] public Obstacle[] obstacles;
        public float offsetX = 0.5f; // distancia del centro al borde del tronco (medio grosor)

        public void SetObstacles(Obstacle[] obstacles)
        {
            this.obstacles = obstacles;
        }
        

        // X del borde del tronco del lado indicado (-1 izquierda, 1 derecha)
        public float GetSideX(int side)
        {
            return transform.position.x + side * offsetX;
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Vector3 p = transform.position;
            Gizmos.DrawLine(new Vector3(p.x - offsetX, p.y - 10, p.z), new Vector3(p.x - offsetX, p.y + 10, p.z));
            Gizmos.DrawLine(new Vector3(p.x + offsetX, p.y - 10, p.z), new Vector3(p.x + offsetX, p.y + 10, p.z));
        }

        public bool HitsObstacle(Collider2D c)
        {
            foreach (Obstacle o in obstacles)
                if (o.Overlaps(c))
                {
                    o.Hit();
                    return true;
                }
            return false;
        }
    }
}
