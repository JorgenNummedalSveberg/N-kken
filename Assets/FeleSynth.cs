using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;
using Object = UnityEngine.Object;

public class FeleSynth : MonoBehaviour
{
    [Range(0, 127)] public byte note;
    [Range(0.1f, 5f)] public float interval;
    public GameObject synthesizerObject;
    private float _timer;
    private AudioSource _source;

    [Range(0, 255)] public byte val1;
    [Range(0, 16383)] public ushort val2 = 8192;
    [Range(0, 127)] public byte val3;
    [Range(0, 255)] public byte val4;

    // Start is called before the first frame update
    void Start()
    {
    }

//    private void OnAudioFilterRead(float[] data, int channels)
//    {
//        increment = frequency * 2.0 * Math.PI / sampling_frequency;
//
//        for (int i = 0; i < data.Length; i += channels)
//        {
//            phase += increment;
//            data[i] = (float) (gain * (2 * (phase - Math.Floor(0.5 + phase))));
//
//            if (channels == 2)
//            {
//                data[i + 1] = data[i];
//            }
//
//            if (phase > (Math.PI * 2))
//            {
//                phase = 0.0;
//            }
//        }
//    }
//
//    public void SetGain(float newGain, float max)
//    {
//        float calcGain = (newGain/max)*0.1f;
//        gain = Math.Min(calcGain, 0.1f);
//    }
//    public void SetFreq(float newFreq, float max)
//    {
//        double calcFreq = _minFreq + (newFreq/max)*(_maxFreq-_minFreq);
//        frequency = calcFreq;
//    }

    private void UpdateSynth()
    {
//        Debug.Log(val1);
//        Debug.Log(val2);
//        Debug.Log(val3);
//        Debug.Log(val4);
//        _synthesizer.Channel[0].Modulation(val1);
//        _synthesizer.Channel[0].Modulation(val1);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSynth();

        _timer += Time.deltaTime;
        if (_timer >= interval)
        {
            _timer = 0;
        }
    }
}
