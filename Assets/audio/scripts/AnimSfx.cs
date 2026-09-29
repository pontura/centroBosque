using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AnimSfx : MonoBehaviour {

	public AudioClip[] clips;
	AudioSource source;

    //public AnimationsManager.anim animName;

	// Use this for initialization
	void Start () {
		source = gameObject.AddComponent<AudioSource> ();
        source.playOnAwake = false;
        source.outputAudioMixerGroup = AudioManager.Instance.sfx;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Init() {
        source.Stop();
        source.volume = 1;
        source.pitch = 1f;
    }
    
    /*public void PlayMod(int i) {
        if (animName != AnimationsManager.anim.NONE) {
            AudioClip clip = AudioManager.Instance.modSfx.GetModClip(animName, i);
            source.PlayOneShot(clip);
        }
    }*/

    public void Play(int i){
		source.PlayOneShot (clips [i]);
	}

    public void Stop() {
        source.Stop();
    }

    public void Pitch(float pitch) {
        source.pitch = pitch;
    }

    public void SetVol(float vol) {
        source.volume = vol;
    }

    public void SetTranspose(int pitch) {
        if (pitch >= 0)
            source.pitch = 1 * AudioManager.Instance.pitchSteps[Math.Abs(pitch)];
        else
            source.pitch = 1 / AudioManager.Instance.pitchSteps[Math.Abs(pitch)];
    }

    public void PlayMusic(string name) {
        AudioManager.Instance.musicManager.Play(name);
    }

    public void PlayMusicLoop(string name) {
        AudioManager.Instance.musicManager.PlayLoop(name);
    }

    public void StopMusicLoop() {
        AudioManager.Instance.musicManager.StopLoop();
    }

    public void PlayUISfx(string name) {
        AudioManager.Instance.uiSfxManager.Play(name);
    }

    public void PlaySfx(string name) {
        AudioManager.Instance.sfxManager.Play(name);
    }
        
}
