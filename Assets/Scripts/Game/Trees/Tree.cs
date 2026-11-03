using UnityEngine;

namespace TreesGame
{
    public class Tree : MonoBehaviour
    {
        [HideInInspector] public Obstacle[] obstacles;
        Renderer rend;

        public void SetObstacles(Obstacle[] obstacles)
        {
            this.obstacles = obstacles;
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
                {
                    o.Hit();
                    return true;
                }
            return false;
        }
    }
}
