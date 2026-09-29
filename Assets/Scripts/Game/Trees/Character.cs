using UnityEngine;

namespace TreesGame
{
    public class Character : MonoBehaviour
    {
        public KeyCode keyCode = KeyCode.Space;
        public int startTreeID = 0;
        public float jumpHeight = 2f;   // cuanto sube en cada salto
        public float arcHeight = 1f;    // altura extra de la parabola sobre el punto de llegada
        public float jumpDuration = 0.5f;
        public float slideSpeed = 0.5f; // velocidad a la que resbala por el tree
        public float gravity = 20f;     // al caerse por un obstacle
        public float restartDelay = 0.5f; // espera en el piso antes de empezar de nuevo

        enum states { onTree, jumping, falling, dead, won }
        states state;

        Trees trees;
        Collider2D col;
        Vector3 startPos;
        float floorY;
        int treeID;
        float halfWidth;
        float jumpTime;
        float velocityY;
        Vector3 from;
        Vector3 to;

        public void Init(Trees t)
        {
            trees = t;
            col = GetComponentInChildren<Collider2D>();
            if (col == null)
                Debug.LogWarning("Character sin Collider2D: no va a detectar obstacles");
            Renderer rend = GetComponentInChildren<Renderer>();
            halfWidth = rend != null ? rend.bounds.extents.x : 0;
            floorY = transform.position.y;

            // Se ubica al lado del tree inicial, del lado que mira al otro tree
            treeID = startTreeID;
            startPos = transform.position;
            startPos.x = GetSideX(treeID, GetOtherTreeID());
            Restart();
        }

        void Restart()
        {
            CancelInvoke();
            treeID = startTreeID;
            transform.position = startPos;
            velocityY = 0;
            state = states.onTree;
        }

        void Update()
        {
            if (trees == null) return;

            switch (state)
            {
                case states.onTree:
                    if (Input.GetKeyDown(keyCode))
                        Jump();
                    else
                        Slide();
                    break;
                case states.jumping:
                    UpdateJump();
                    break;
                case states.falling:
                    UpdateFall();
                    return;
                default:
                    return;
            }

            if (col == null) return;
            // Movemos el transform a mano: actualizar los colliders antes de comparar
            Physics2D.SyncTransforms();

            if (trees.goal != null && trees.goal.Overlaps(col))
                Win();
            else if (HitsObstacle())
                Fall();
        }

        // Llega al goal: queda quieto en la posicion del goal y despues empieza de nuevo
        void Win()
        {
            transform.position = trees.goal.GetCharacterPosition();
            state = states.won;
            Invoke("Restart", trees.goal.freezeTime);
        }

        void UpdateJump()
        {
            jumpTime += Time.deltaTime;
            float t = Mathf.Clamp01(jumpTime / jumpDuration);
            Vector3 pos = Vector3.Lerp(from, to, t);
            pos.y += arcHeight * 4 * t * (1 - t);
            transform.position = pos;

            if (t >= 1)
                state = states.onTree;
        }

        // Pegado al tree resbala hacia abajo hasta tocar el piso
        void Slide()
        {
            Vector3 pos = transform.position;
            if (pos.y <= floorY) return;
            pos.y = Mathf.Max(floorY, pos.y - slideSpeed * Time.deltaTime);
            transform.position = pos;
        }

        void Jump()
        {
            int targetID = GetOtherTreeID();
            from = transform.position;
            to = from;
            to.x = GetSideX(targetID, treeID);
            to.y += jumpHeight;
            treeID = targetID;
            jumpTime = 0;
            state = states.jumping;
        }

        void Fall()
        {
            velocityY = 0;
            state = states.falling;
        }

        // Cae en vertical hasta el piso y despues empieza de nuevo
        void UpdateFall()
        {
            velocityY -= gravity * Time.deltaTime;
            Vector3 pos = transform.position;
            pos.y += velocityY * Time.deltaTime;
            if (pos.y <= floorY)
            {
                pos.y = floorY;
                state = states.dead;
                Invoke("Restart", restartDelay);
            }
            transform.position = pos;
        }

        bool HitsObstacle()
        {
            foreach (Tree tree in trees.trees)
                if (tree.HitsObstacle(col))
                    return true;
            return false;
        }

        int GetOtherTreeID()
        {
            return treeID == 0 ? 1 : 0;
        }

        // X donde queda el character pegado al tree, del lado que mira hacia lookAtTreeID
        float GetSideX(int id, int lookAtTreeID)
        {
            Tree tree = trees.trees[id];
            int side = trees.trees[lookAtTreeID].transform.position.x > tree.transform.position.x ? 1 : -1;
            return tree.GetSideX(side) + side * halfWidth;
        }
    }
}
