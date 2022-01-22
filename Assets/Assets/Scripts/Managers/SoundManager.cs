using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField]private AudioClip[] _audioClips;
    [SerializeField]private AudioSource _audioSource;
    public static SoundManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void PlayClip(string clipName)
    {
        try
        {
            int clipIndex = 0;
        for (int i = 0; i < _audioClips.Length ; i++)
        {
            if(_audioClips[i].name==clipName)
            {
                clipIndex = i;
                break;
            }
        }
      

        _audioSource.clip = _audioClips[clipIndex];
        _audioSource?.Play();
        }
        catch (System.Exception)
        {
        }
    }  
    public void StopClip(string clipName)
    {
        int clipIndex = 0;
        for (int i = 0; i < _audioClips.Length ; i++)
        {
            if(_audioClips[i].name==clipName)
            {
                clipIndex = i;
                break;
            }
        }
        _audioSource.clip = _audioClips[clipIndex];
        _audioSource.Stop();
    }
}
