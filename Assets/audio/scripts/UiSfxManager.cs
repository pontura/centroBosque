using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class UiSfxManager : MonoBehaviour
{
    public float defaultVol;
    public List<UISfxClip> uiClips;
    AudioSource asource;

    Dictionary<string, int> scalesIndex;

    [Serializable]
    public class UISfxClip {
        public AudioClip clip;
        public string name;
    }


    // Start is called before the first frame update
    void Start() {
        asource = GetComponent<AudioSource>();
        scalesIndex = new Dictionary<string, int>();
        //Events.OnLoading += OnLoading;
    }

    private void OnDestroy() {
        //Events.OnLoading -= OnLoading;
    }

    void OnLoading(bool enable) {
        if(!enable)
            PlayTransp("tilde", -5);
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void Stop() {
        asource.Stop();
    }

    public void Play(string name) {
        Play(name, defaultVol);        
    }

    public void Play(string name, float vol) {
        UISfxClip sc = uiClips.Find(x => x.name == name);
        if (sc != null) {
            asource.pitch = 1f;
            asource.volume = vol;
            asource.PlayOneShot(sc.clip);
        }   
    }

    public void PlayTransp(string name, int pitch) {
        UISfxClip sc = uiClips.Find(x => x.name == name);
        if (sc != null) {
            if(pitch>=0)
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

    
    int[] scaleValues = { -3, 0, 2, 4, -5 };
    void PlayScale(string name, bool next, int[] scaleVals = null) {
        if (scaleVals == null)
            scaleVals = scaleValues;
        int scaleIndex = 0;
        if (scalesIndex.ContainsKey(name)) {
            scaleIndex = scalesIndex[name];
            if (next) {
               // Debug.Log("NEXT");
                scaleIndex++;
                if (scaleIndex >= scaleVals.Length)
                    scaleIndex = 0;
            } else {
               // Debug.Log("PREV");
                scaleIndex--;
                if (scaleIndex < 0)
                    scaleIndex = scaleVals.Length - 1;
            }
        } else
            scalesIndex.Add(name, scaleIndex);

       // Debug.Log("Index: " + scaleIndex + " / val: " + scaleValues[scaleIndex]);
        UISfxClip sc = uiClips.Find(x => x.name == name);
        asource.pitch = 1f;
        
        if (scaleVals[scaleIndex] >= 0) {
            asource.pitch = 1f * AudioManager.Instance.pitchSteps[Math.Abs(scaleVals[scaleIndex])];
        } else if (scaleVals[scaleIndex] < 0) {
            asource.pitch = 1f / AudioManager.Instance.pitchSteps[Math.Abs(scaleVals[scaleIndex])];
        }        

        asource.PlayOneShot(sc.clip);
        
        scalesIndex[name] = scaleIndex;
    }
    public void PlayNextScale(string name, int[] scaleVals = null) {
        PlayScale(name, true, scaleVals);

    }
    public void PlayPrevScale(string name, int[] scaleVals = null) {
        PlayScale(name, false, scaleVals);
    }
}
