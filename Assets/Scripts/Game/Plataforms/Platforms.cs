using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PlatformsGame
{
    public class Platforms : MonoBehaviour
    {
        public Character[] characters;
        public SettingsData settings;
        public states state;
        public int levelID;
        public enum states
        {
            splash,
            game,
            summary
        }
        public List<Platform> platforms;

        void Start()
        {
           
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
        }
        void StartDelayed()
        {
            int id = 0;

            foreach(Platform p in platforms)
            {
                PlatformData pd = new PlatformData();
                pd.minX = settings.platform_minX;
                pd.maxX = settings.platform_maxX;
                pd.speed = settings.platform_speed;

                if(id<platforms.Count-1)
                {
                    Vector3 pos = p.transform.localPosition;
                    pos.x += settings.platform_offset * id;
                    pd.minX += settings.platform_narrower *id; 
                    pd.maxX -= settings.platform_narrower *id; 
                    p.transform.localPosition = pos;
                    p.Init(pd);
                }
                id++;
            }

            foreach(Character character in characters)
                character.Init(this);
        }
        
    }
}
