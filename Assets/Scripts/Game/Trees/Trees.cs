using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace TreesGame
{
    public class Trees : MonoBehaviour
    {
        public Character[] characters;
        public SettingsData settings;
        public states state;
        public int levelID;
        public List<Tree> trees;
        ObstaclesManager obstaclesManager;
        public enum states
        {
            splash,
            game,
            summary
        }

        void Start()
        {
           obstaclesManager = GetComponent<ObstaclesManager>();
            StartCoroutine(LoadSettings());

        }
        IEnumerator LoadSettings()
        {
            string path = Path.Combine(Application.streamingAssetsPath, "settings.json");

            string json = "";
            if (path.Contains("://"))
            {
                using (WWW www = new WWW(path))
                {
                    yield return www;
                    json = www.text;
                }
            }
            else if (File.Exists(path))
            {
                json = File.ReadAllText(path);
            }
            else
            {
                Debug.LogWarning("Settings file not found at " + path);
            }

            if (json != "")
            {
                settings = JsonUtility.FromJson<SettingsData>(json);
                Debug.Log("Settings loaded: " + json);
                Invoke("StartDelayed", 0.5f);
            }
            SetLevel(obstaclesManager.GetLevel());
        }
        public void SetLevel(Level level)
        {
            trees[0].SetObstacles(level.obstaclesTree1);
            trees[1].SetObstacles(level.obstaclesTree2);
            trees[2].SetObstacles(level.obstaclesTree3);
        }
        void StartDelayed()
        {

            foreach(Character character in characters)
                character.Init(this);
        }
        
    }
}
