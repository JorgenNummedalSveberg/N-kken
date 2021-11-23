using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeleSpilling : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] violinClips;
    private float gain = 1;
    private int _vioIndex;

    public float scaryThreshold = 5;

    private void Start()
    {
        _vioIndex = 1;
        FadeInto(0.5f, _vioIndex);
    }

    public void SetGain(float newGain, float max)
    {
        audioSource.volume = Math.Max(newGain/max, 1);
        gain = Math.Max(newGain/max, 1);
    }
    public void SetFreq(float newFreq, float max)
    {
        int oldIndex = _vioIndex;
        _vioIndex = newFreq > scaryThreshold ? 0 : 1;
        if (oldIndex != _vioIndex)
        {
            FadeInto(0.5f, oldIndex);
        }
    }

    public void FadeInto(float duration, int index)
    {
        float startVolume = audioSource.volume;
        while (audioSource.volume > 0) {
            audioSource.volume -= startVolume * Time.deltaTime / (duration/2);
        }
        audioSource.clip = violinClips[index];
        audioSource.Play();
        while (audioSource.volume < gain) {
            audioSource.volume += startVolume * Time.deltaTime / (duration/2);
        }
    }
}
