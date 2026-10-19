using UnityEngine;

namespace PlatformsGame
{
    public class Character : MonoBehaviour
    {
        [SerializeField] Animator anim;
        public KeyCode keyCode;
        public float jumpSpeed = 8f;
        public float gravity = 20f;
        public float winDelay = 6f;     // espera en la ultima platform antes de volver al inicio

        Vector3 startPos;
        bool won;
        float floorY;
        float groundY;
        float velocityY;
        bool grounded = true;
        Renderer rend;
        float bottomOffset;
        float halfWidth;
        Platforms platforms;
        Platform currentPlatform;

        public void Init(Platforms platforms)
        {
            this.platforms = platforms;
            rend = GetComponentInChildren<Renderer>();
            startPos = transform.position;
            floorY = transform.position.y;
            groundY = floorY;
            bottomOffset = transform.position.y - rend.bounds.min.y;
            halfWidth = rend.bounds.extents.x;
        }

        void Update()
        {
            if (platforms == null || won) return;

            if (grounded && Input.GetKeyDown(keyCode))
            {
                velocityY = jumpSpeed;
                grounded = false;
                if (currentPlatform != null)
                {
                    currentPlatform.Resume();
                    currentPlatform = null;
                }                
                Anim("up");
            }

            if (grounded) return;

            velocityY -= gravity * Time.deltaTime;
            Vector3 pos = transform.position;
            float prevBottom = pos.y - bottomOffset;
            pos.y += velocityY * Time.deltaTime;

            if (velocityY < 0)
            {
                // Las platforms se mueven por transform: actualizar los colliders antes de leer sus bounds
                Physics2D.SyncTransforms();
                Platform platform = CheckLanding(pos.x, prevBottom, pos.y - bottomOffset);
                if (platform != null)
                {
                    platform.Stop();
                    currentPlatform = platform;
                    groundY = platform.GetBounds().max.y + bottomOffset;
                    pos.y = groundY;
                    Land();
                    if (platform == platforms.platforms[platforms.platforms.Count - 1])
                        Win();
                }
                else if (pos.y <= floorY)
                {
                    groundY = floorY;
                    pos.y = floorY;
                    Land();
                }
            }
            transform.position = pos;
        }

        // Solo detecta al caer: el pie tiene que cruzar el borde superior de la platform en este frame
        Platform CheckLanding(float x, float prevBottom, float newBottom)
        {
            foreach (Platform p in platforms.platforms)
            {
                Bounds b = p.GetBounds();
                float top = b.max.y;
                bool crossed = prevBottom >= top && newBottom <= top;
                bool overlapX = x + halfWidth > b.min.x && x - halfWidth < b.max.x;
                if (crossed && overlapX)
                    return p;
            }
            return null;
        }

        void Land()
        {
            velocityY = 0;
            grounded = true;
            Anim("land");
        }

        // Llega a la ultima platform: festeja, espera y vuelve al inicio
        void Win()
        {
            won = true;
            Anim("win");
            Invoke("Restart", winDelay);
        }

        void Restart()
        {
            if (currentPlatform != null)
            {
                currentPlatform.Resume();
                currentPlatform = null;
            }
            transform.position = startPos;
            groundY = floorY;
            velocityY = 0;
            grounded = true;
            won = false;
            Anim("land");
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
    }
}
