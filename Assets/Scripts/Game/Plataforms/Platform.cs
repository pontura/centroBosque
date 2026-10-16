using System.Collections.Generic;
using UnityEngine;

namespace PlatformsGame
{
    public class Platform : MonoBehaviour
    {

        public PlatformData data;
        public bool stopped;

        Renderer rend;
        Collider2D col; // si tiene, define la zona de aterrizaje (editable a mano)
        float direction = -1;

        void Awake()
        {
            rend = GetComponentInChildren<Renderer>();
            col = GetComponentInChildren<Collider2D>();
        }
        public void Init(PlatformData pd)
        {
            this.data = pd;
        }
        void Update()
        {
            if (stopped || data == null) return;

            Vector3 pos = transform.localPosition;
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

            transform.localPosition = pos;
        }

        public Bounds GetBounds()
        {
            if (col != null && col.enabled)
                return col.bounds;
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
