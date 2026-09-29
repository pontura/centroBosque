using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    static AudioManager mInstance = null;
    public AudioMixer mixer;
    public AudioMixerGroup sfx;
    public AudioMixerGroup uiSfx;    

    public SfxManager sfxManager;
    public UiSfxManager uiSfxManager;
    public MusicManager musicManager;
    public MusicManager ambienceManager;
    public ModSfx modSfx;

    public float[] pitchSteps;

    int maxVoices = 16;
    int voicesCount;

    public static AudioManager Instance {
        get {
            return mInstance;
        }
    }
    void Awake() {
        if (!mInstance)
            mInstance = this;

        pitchSteps = new float[13];
        pitchSteps[0] = 1f;
        for (int i = 1; i < 12; i++)
            pitchSteps[i] = Mathf.Pow(2, i / 12f);
        pitchSteps[12] = 2f;
    }

    public bool IsVoiceRoom() {
        return voicesCount < maxVoices;
    }

    public void VoiceCount(float clipLength) {
        voicesCount++;
        StartCoroutine(OnEnd(clipLength, RemoveVoice));
    }

    void RemoveVoice() {
        voicesCount--;
    }

    private IEnumerator OnEnd(float clipLength, System.Action callback) {
        yield return new WaitForSeconds(clipLength);
        callback();
    }

    public void PlaySound(AudioSource source, AudioClip clip, float volume = 1, bool loop = false) {

        print("PlaySoundPlaySound " + source.name + " " + clip.name + " loop " + loop);
        source.volume = volume;
        source.clip = clip;
        source.loop = loop;
        source.Play();
    }
    
    public void PlaySoundOneShot(AudioSource source, AudioClip clip, float volume = 1) {
        source.volume = volume;
        source.PlayOneShot(clip);
    }
}
