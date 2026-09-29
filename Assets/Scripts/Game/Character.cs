using UnityEngine;

namespace PlatformsGame
{
    public class Character : MonoBehaviour
    {
        public KeyCode keyCode;
        public float jumpSpeed = 8f;
        public float gravity = 20f;

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
            floorY = transform.position.y;
            groundY = floorY;
            bottomOffset = transform.position.y - rend.bounds.min.y;
            halfWidth = rend.bounds.extents.x;
        }

        void Update()
        {
            if (grounded && Input.GetKeyDown(keyCode))
            {
                velocityY = jumpSpeed;
                grounded = false;
                if (currentPlatform != null)
                {
                    currentPlatform.Resume();
                    currentPlatform = null;
                }
            }

            if (grounded) return;

            velocityY -= gravity * Time.deltaTime;
            Vector3 pos = transform.position;
            float prevBottom = pos.y - bottomOffset;
            pos.y += velocityY * Time.deltaTime;

            if (velocityY < 0)
            {
                Platform platform = CheckLanding(pos.x, prevBottom, pos.y - bottomOffset);
                if (platform != null)
                {
                    platform.Stop();
                    currentPlatform = platform;
                    groundY = platform.GetBounds().max.y + bottomOffset;
                    pos.y = groundY;
                    Land();
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
        }
    }
}
