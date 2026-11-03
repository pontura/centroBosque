using UnityEngine;
namespace TreesGame
{
    public class ObstaclesManager : MonoBehaviour
    {
        public Level[] levels;
        public KeyCode keyCode = KeyCode.B;
        int level_ID = 0;
        public Level GetLevel()
        {
            return levels[level_ID];
        }
        void ChangeLevel()
        {
            level_ID++;
            if(level_ID >= levels.Length)
                level_ID = 0;
                
            Level level = levels[level_ID];

            foreach(Level l in levels)
                l.gameObject.SetActive(l == level);

            GetComponent<Trees>().SetLevel(level);
            print("Level changed to " + level_ID);
        }
        void Update()
        {
            if (Input.GetKeyDown(keyCode))
                ChangeLevel();
        }
    }
}