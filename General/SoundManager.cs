using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] private AudioSource sourceSound;
    [SerializeField] private AudioSource sourceMusic;

    [SerializeField] private List<AudioClip> sourceClipSounds;

    public void PlayFx(int type = 0)
    {
        if (GameData.Data.IsSoundOn)
        {
            sourceSound.clip = sourceClipSounds[type];
            sourceSound.Play();
        }
    }

    public void PlayOnCamera(int type = 0)
    {
        if (GameData.Data.IsSoundOn)
        {
            AudioSource.PlayClipAtPoint(sourceClipSounds[type], Camera.main.transform.position);
        }
    }

    public float GetTimeFx(int type = 0)
    {
        return sourceClipSounds[type].length;
    }

    public void StopMusic()
    {
        sourceMusic.Stop();
    }

    public void SetVolumeMusic(bool IsMusicOn)
    {
        sourceMusic.volume = IsMusicOn ? 1 : 0;
    }
}
