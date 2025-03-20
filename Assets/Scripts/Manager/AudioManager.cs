using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : BaseSingleton<AudioManager>
{
    public AudioSource bgrSource {get; private set;}
    public float volumeEffect { get; private set; } = 0.6f;
    public float volumeBgr { get; private set; } = 0.6f;
    public bool isSoundMuted { get; private set; } = false;
    public bool isMusicMuted { get; private set; } = false;
    public bool isMuted { get; private set; } = false;

    private Dictionary<SoundEffect, AudioClip> _soundDic;
    protected override void Awake()
    {
        base.Awake();
        LoadAllSounds();
    }

    private void Start()
    {
        if (bgrSource == null) bgrSource = gameObject.AddComponent<AudioSource>();
        bgrSource.clip = GetSound(SoundEffect.BG);
        bgrSource.loop = true;
        bgrSource.volume = 1f;
        bgrSource.Play();
    }
    private void LoadAllSounds()
    {
        _soundDic = new Dictionary<SoundEffect, AudioClip>();

        foreach (SoundEffect audioName in Enum.GetValues(typeof(SoundEffect)))
        {
            string path = $"Audio/{audioName}";
            AudioClip clip = Resources.Load<AudioClip>(path);

            if (clip != null)
            {
                _soundDic[audioName] = clip;
            }
            else
            {
                Debug.LogWarning($"AudioClip '{audioName}' not found at path: {path}");
            }
        }
        //Debug.Log("SoundDic Count:" + _soundDic.Count);
    }
    public void PlaySoundEffect(SoundEffect audioName)
    {
        AudioClip clip = GetSound(audioName);
        if (isSoundMuted) return;
        if (clip != null)
        {
            GameObject tempGO = new GameObject("Audio");
            AudioSource tempSource = tempGO.AddComponent<AudioSource>();
            tempSource.volume = volumeEffect;
            tempSource.PlayOneShot(clip);
            //DontDestroyOnLoad(tempGO);
            Destroy(tempGO, clip.length);
        }
    }
    public void ToggleMusic()
    {
        isMusicMuted = !isMusicMuted;
        bgrSource.mute = isMusicMuted;
    }

    public void ToggleSoundEffects()
    {
        isSoundMuted = !isSoundMuted;
    }
    public void ToggleAllAudio()
    {
        bool newMuteState = !isMuted;
        isMuted = newMuteState;
        bgrSource.mute = newMuteState;
        isMusicMuted = newMuteState;
        isSoundMuted = newMuteState;
    }
    private AudioClip GetSound(SoundEffect audioName)
    {
        if (_soundDic.TryGetValue(audioName, out var clip))
        {
            return clip;
        }
        Debug.LogWarning($"AudioClip '{audioName}' not found in dictionary.");
        return null;
    }
}

public static class AudioExtensions
{
    public static void Play(this SoundEffect audioName)
    {
        AudioManager.Instance.PlaySoundEffect(audioName);
    }
}