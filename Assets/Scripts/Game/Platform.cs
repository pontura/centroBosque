using System.Collections.Generic;
using UnityEngine;

namespace PlatformsGame
{
    public class Platform : MonoBehaviour
    {

        public PlatformData data;
        public bool stopped;

        Renderer rend;
        float direction = -1;

        void Awake()
        {
            rend = GetComponentInChildren<Renderer>();
        }
        public void Init(PlatformData pd)
        {
            this.data = pd;
        }
        void Update()
        {
            if (stopped || data == null) return;

            Vector3 pos = transform.position;
            pos.x += direction * data.speed * Time.deltaTime;

            if (pos.x <= data.minX)
            {
                pos.x = data.minX;
                direction = 1;
            }
            else if (pos.x >= data.maxX)
            {
                pos.x = data.maxX;
                direction = -1;
            }

            transform.position = pos;
        }

        public Bounds GetBounds()
        {
            return rend.bounds;
        }

        public void Stop()
        {
            stopped = true;
        }

        public void Resume()
        {
            stopped = false;
        }
    }
}
