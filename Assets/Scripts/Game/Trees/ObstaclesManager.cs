using UnityEngine;
namespace TreesGame
{
    public class ObstaclesManager : MonoBehaviour
    {
        public Level level;
        public int totalLevels = 2;
        public Animation levels;
        public KeyCode keyCode = KeyCode.B;
        public float exitDuration = 0.5f; // tiempo de "exit" de los obstacles antes de reposicionarlos
        int level_ID = 0;
        bool changing;
        Obstacle[] obstacles;

        void Awake()
        {
            obstacles = levels.GetComponentsInChildren<Obstacle>(true);
        }
        void Start()
        {
            levels.Play("level" + (level_ID+1));
            GetComponent<Trees>().SetLevel(level);
        }

        // Los obstacles salen ("exit"), se reposicionan con la animacion del level y entran ("entry")
        void ChangeLevel()
        {
            if (changing) return;
            changing = true;

            foreach (Obstacle o in obstacles)
                o.Exit();
            Invoke("ApplyLevel", exitDuration);
        }

        void ApplyLevel()
        {
            level_ID++;
            if(level_ID >= totalLevels)
                level_ID = 0;

            string clip = "level" + (level_ID+1);
            levels.Play(clip);
            print("Level changed to " + level_ID);

            // Cuando termina de reposicionarse corren "entry"
            AnimationState s = levels[clip];
            Invoke("EntryObstacles", s != null ? s.length / Mathf.Max(s.speed, 0.0001f) : 0f);
        }

        void EntryObstacles()
        {
            foreach (Obstacle o in obstacles)
                o.Entry();
            changing = false;
        }

        void Update()
        {
            if (Input.GetKeyDown(keyCode))
                ChangeLevel();
        }
    }
}