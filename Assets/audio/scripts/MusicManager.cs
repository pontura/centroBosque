using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MusicManager : MonoBehaviour {

    public float defaultVol;
    public List<MusicClip> musicClips;
    AudioSource asource;

    int maxSfxVoices = 16;

    [Serializable]
    public class MusicClip {
        public AudioClip clip;
        public string name;
    }


    // Start is called before the first frame update
    void Start() {
        asource = GetComponent<AudioSource>();
    }

    private void OnDestroy() {
        
    }

    // Update is called once per frame
    void Update() {

    }
    public void PlayLoop(string name) {
        PlayLoop(name, defaultVol);
    }

    public void PlayLoop(string name, float vol) {
        MusicClip sc = musicClips.Find(x => x.name == name);
        if (sc != null) {
            asource.volume = vol;
            asource.pitch = 1f;
            asource.loop = true;
            asource.clip = sc.clip;
            asource.Play();
        }
    }

    public void StopLoop() {
        asource.Stop();
        asource.loop = false;
    }

    public void Play(string name, float vol, Action callback = null) {
        MusicClip sc = musicClips.Find(x => x.name == name);
        if (sc != null) {
            asource.volume = vol;
            asource.pitch = 1f;
            if (!asource.isPlaying)
                asource.PlayOneShot(sc.clip);
            else if (asource.clip != sc.clip)
                asource.PlayOneShot(sc.clip);

            if(callback!=null)
                StartCoroutine(OnEnd(sc.clip.length, callback));
        }
    }

    public void Play(string name) {
        Play(name,defaultVol);
    }

    private IEnumerator OnEnd(float clipLength, Action callback) {
        yield return new WaitForSeconds(clipLength);

        callback();
    }

    int count;
    void PlayRandom(string name) {
        if (count < maxSfxVoices) {
            MusicClip sc = musicClips.Find(x => x.name == name);
            if (sc != null) {
                if (!asource.isPlaying) {
                    asource.pitch = 0.9f + UnityEngine.Random.value * 0.2f;
                    asource.PlayOneShot(sc.clip, 0.5f + UnityEngine.Random.value * 0.5f);
                    count++;
                    StartCoroutine(StartMethod(sc.clip.length));
                } else if (asource.clip != sc.clip) {
                    asource.pitch = 0.9f + UnityEngine.Random.value * 0.2f;
                    asource.PlayOneShot(sc.clip, 0.5f + UnityEngine.Random.value * 0.5f);
                    count++;
                    StartCoroutine(StartMethod(sc.clip.length));
                }
            }
        }
    }

    private IEnumerator StartMethod(float clipLength) {
        yield return new WaitForSeconds(clipLength);

        count--;
    }

    public void PlayTransp(string name,int pitch) {
        MusicClip sc = musicClips.Find(x => x.name == name);
        if (sc != null) {
            if (pitch >= 0)
                asource.pitch = 1 * AudioManager.Instance.pitchSteps[Math.Abs(pitch)];
            else
                asource.pitch = 1 / AudioManager.Instance.pitchSteps[Math.Abs(pitch)];
            asource.PlayOneShot(sc.clip);
        }
    }

    public void PlayTransp(string name, int pitch, float vol) {
        asource.volume = vol;
        PlayTransp(name, pitch);        
    }

}

