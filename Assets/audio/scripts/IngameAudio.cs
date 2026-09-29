using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using static UiSfxManager;

namespace YaguarLib.Audio
{
    public class IngameAudio : MonoBehaviour
    {
        public List<ClipData> clips;
        [SerializeField] AudioSource source;

        [Serializable]
        public class ClipData
        {
            public AudioClip clip;
            public string name;
            public float vol;
        }

        public void Play(string key) {
            ClipData cp = clips.Find(x => x.name == key);
            if (cp == null)
                return;
            AudioManager.Instance.PlaySound(source, cp.clip, cp.vol);
        }

        public void PlayLoop(string key) {
            ClipData cp = clips.Find(x => x.name == key);
            if (cp == null)
                return;
            AudioManager.Instance.PlaySound(source, cp.clip, cp.vol, true);
        }

        public void Stop() {
            source.Stop();
            source.loop = false;
        }

        public void PlayOneShot(string key) {
            ClipData cp = clips.Find(x => x.name == key);
            if (cp == null)
                return;
            AudioManager.Instance.PlaySoundOneShot(source, cp.clip, cp.vol);
        }

        public void Play(string key, bool loop=false, Action onClipDone=null, bool noRepeat = false) {
            Debug.Log("Play " + key);
            ClipData cp = clips.Find(x => x.name == key);
            if (cp == null)
                return;

            //AudioManager.Instance.PlaySound(cp.clip, sourceName:sourceKey, volume:cp.vol);

            //TO-DO
            AudioManager.Instance.PlaySound(source, cp.clip, cp.vol);
            if (onClipDone != null)
                StartCoroutine(OnClipDone(cp.clip.length,onClipDone));
        }        

        IEnumerator OnClipDone(float clipLength, Action onDone) {
            yield return new WaitForSeconds(clipLength);
            onDone();
        }

        public void CancelAllOnClipDone() {
            StopAllCoroutines();
        }
    }
}
