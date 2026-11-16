using UnityEngine;

namespace TreesGame
{
    public class Obstacle : MonoBehaviour
    {
        Collider2D[] colliders;
        Animation anim;

        void Awake()
        {
            colliders = GetComponentsInChildren<Collider2D>();
            anim = GetComponentInChildren<Animation>();
        }

        // Lo toco un character: corre la animacion "hit" desde el principio
        public void Hit()
        {
            if (anim == null) return;
            anim.Stop();
            anim.Play("hit");
        }

        // Cambio de level: sale ("exit") y despues vuelve a entrar ("entry")
        public void Exit() => PlayFromStart("exit");
        public void Entry() => PlayFromStart("entry");

        void PlayFromStart(string clip)
        {
            if (anim == null) return;
            anim.Stop();
            anim.Play(clip);
        }

        // Compara la forma real de los colliders (box, circle, polygon, etc)
        public bool Overlaps(Collider2D other)
        {
            foreach (Collider2D c in colliders)
                if (c.enabled && c.Distance(other).isOverlapped)
                    return true;
            return false;
        }
    }
}
