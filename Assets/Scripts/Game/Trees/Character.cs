using UnityEngine;

namespace TreesGame
{
    public class Character : MonoBehaviour
    {
        [SerializeField] Animator anim;
        public KeyCode keyCode = KeyCode.Space;
        public int startTreeID = 0;     // tree donde arranca
        public int otherTreeID = 1;     // el otro tree entre los que salta
        public Goal goal;               // goal propio de este character
        public float jumpHeight = 2f;   // cuanto sube en cada salto
        public float arcHeight = 1f;    // altura extra de la parabola sobre el punto de llegada
        public float jumpDuration = 0.5f;
        [UnityEngine.Serialization.FormerlySerializedAs("slideSpeed")]
        public float climbSpeed = 0.5f; // velocidad a la que sube lentamente por el tree
        public float gravity = 20f;     // al caerse por un obstacle
        public float restartDelay = 0.5f; // espera en el piso antes de empezar de nuevo

        enum states { waiting, onTree, jumping, falling, dead, won }
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
            if (goal == null)
                Debug.LogWarning("Character sin Goal asignado: " + name);
            col =GetComponentInChildren<Collider2D>();
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
            // Queda quieto en la base hasta que se apriete el boton
            state = states.waiting;
            Anim("idle");
        }

        void Update()
        {
            if (trees == null) return;

            switch (state)
            {
                case states.waiting:
                    if (Input.GetKeyDown(keyCode))
                        state = states.onTree;
                    return;
                case states.onTree:
                    if (Input.GetKeyDown(keyCode))
                        Jump();
                    else
                        Climb();
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

            if (goal != null && goal.Overlaps(col))
                Win();
            else if (HitsObstacle())
                Fall();
        }

        // Llega al goal: queda quieto en la posicion del goal y despues empieza de nuevo
        void Win()
        {
            transform.position = goal.GetCharacterPosition();
            state = states.won;
            Invoke("Restart", goal.freezeTime);
            
            Anim("win");
        }

        void UpdateJump()
        {
            jumpTime += Time.deltaTime;
            float t = Mathf.Clamp01(jumpTime / jumpDuration);
            Vector3 pos = Vector3.Lerp(from, to, t);
            pos.y += arcHeight * 4 * t * (1 - t);
            transform.position = pos;

            if (t >= 1)
            {
                state = states.onTree;
            }
        }
        string lastName;
        void Anim(string name)
        {
            if(anim == null) return;
            if(lastName == name) return;
            print("character anim " + name);
            lastName = name;
            anim.Play(name);
        }

        // Pegado al tree sube lentamente
        void Climb()
        {
            Anim("up");
            Vector3 pos = transform.position;
            pos.y += climbSpeed * Time.deltaTime;
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
            Anim("mid");
        }

        void Fall()
        {
            Anim("down");
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
                
                Anim("land");
                Invoke("Restart", restartDelay);
            }
            transform.position = pos;
        }

        // Solo los obstacles de sus dos trees
        bool HitsObstacle()
        {
            return trees.trees[startTreeID].HitsObstacle(col)
                || trees.trees[otherTreeID].HitsObstacle(col);
        }

        int GetOtherTreeID()
        {
            return treeID == startTreeID ? otherTreeID : startTreeID;
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
