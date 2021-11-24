using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public enum WaveType {Sine, Square, Triangle, Sawtooth, Noise}

public struct Wave
{
    public WaveType Type;
    [Range(0, 1)] public float Amplitude;
    [Range(36, 95)] public int Note;
    public float Time;
    public float TimeStep;
    public float SampleRate;
    public Envelope Envelope;

    public Wave(WaveType type, float amplitude, int note, int sampleRate, Envelope envelope)
    {
        Type = type;
        Amplitude = amplitude;
        Note = note;
        Time = 0;
        TimeStep = 0;
        SampleRate = sampleRate;
        Envelope = envelope;
    }

    private float Increment()
    {
        return AngularVelocity() / SampleRate;
    }

    public (float, float, float) Step()
    {
        float increment = Increment();
        TimeStep += increment;
        Time += increment;
        if (TimeStep > Mathf.PI * 2)
        {
            TimeStep = 0;
        }

        return (Time, TimeStep, increment);
    }

    public float Frequency()
    {
        return 8.1758f * Mathf.Pow(2, Note / 12f);
    }

    public float AngularVelocity()
    {
        return Frequency() * 2f * Mathf.PI;
    }
}

public struct Envelope
{
    public double dAttackTime;
    public double dDecayTime;
    public double dSustainAmplitude;
    public double dReleaseTime;
    public double dStartAmplitude;
    public double dTriggerOffTime;
    public double dTriggerOnTime;
    public bool bNoteOn;
    private double lastAmplitude;
    
    public Envelope(double attackTime, double decayTime, double releaseTime, double sustainAmplitude, double startAmplitude)
    {
        dAttackTime = attackTime * 1000;
        dDecayTime = decayTime * 1000;
        dStartAmplitude = startAmplitude;
        dSustainAmplitude = sustainAmplitude;
        dReleaseTime = releaseTime * 1000;
        bNoteOn = false;
        dTriggerOffTime = 0;
        dTriggerOnTime = 0;
        lastAmplitude = 0;
    }
    
    // Call when key is pressed
    public void NoteOn(double dTimeOn)
    {
        dTriggerOnTime = dTimeOn;
        bNoteOn = true;
    }

    // Call when key is released
    public void NoteOff(double dTimeOff)
    {
        dTriggerOffTime = dTimeOff;
        bNoteOn = false;
    }

    // Get the correct amplitude at the requested point in time
    public double GetAmplitude(double dTime)
    {
        double dAmplitude;
        double dLifeTime = dTime - dTriggerOnTime;

        if (bNoteOn)
        {
            if (dLifeTime <= dAttackTime)
            {
                // In attack Phase - approach max amplitude
                double relativeAttackTimeElapsed = dLifeTime / dAttackTime;
                dAmplitude = dStartAmplitude * relativeAttackTimeElapsed;
            } else if (dLifeTime <= (dAttackTime + dDecayTime))
            {
                // In decay phase - reduce to sustained amplitude
                double decayTimeElapsed = dLifeTime - dAttackTime;
                double relativeDecayTimeElapsed = decayTimeElapsed / dDecayTime;
                double totalAmplitudeChange = dSustainAmplitude - dStartAmplitude;
                dAmplitude = dStartAmplitude + totalAmplitudeChange * relativeDecayTimeElapsed;
            } else
            {
                // In sustain phase - dont change until note released
                dAmplitude = dSustainAmplitude;
            }
            lastAmplitude = dAmplitude;
        }
        else
        {
            // Note has been released, so in release phase
            double releaseTimeElapsed = dTime - dTriggerOffTime;
            double relativereleaseTimeElapsed = releaseTimeElapsed / dReleaseTime;
            dAmplitude = lastAmplitude - lastAmplitude * relativereleaseTimeElapsed;
        }

        // Amplitude should not be negative
        if (dAmplitude < 0)
            dAmplitude = 0.0f;

        return dAmplitude;
    }
}

public class AGrimSynth : MonoBehaviour
{

    public int _sampleRate = 96000;
    [Range(0, 1)] public float volume;
    [Range(36, 95)] public int note;
    private Wave _wave;
    private Wave[] _waves;
    public WaveType waveType;
    private Envelope _envelope;
    public float time;
    public float timeStep;
    public float increment;
    public float frequency;
    private DateTime startTime;

    [Range(0, 2)] public float attackTime;
    [Range(0, 2)] public float decayTime;
    [Range(0, 2)] public float releaseTime;
    [Range(0, 2)] public float sustainAmplitude;
    [Range(0, 2)] public float startAmplitude;

    private void Start()
    {
        Debug.Log("Starto!");
        _envelope = new Envelope(attackTime, decayTime, releaseTime, sustainAmplitude, startAmplitude);
        _wave = new Wave(WaveType.Sine, 1, note, _sampleRate, _envelope);
        _waves = new[] {_wave};
        startTime = DateTime.Now;
    }

    private void Update()
    {
        _envelope.dAttackTime = attackTime * 1000;
        _envelope.dDecayTime = decayTime * 1000;
        _envelope.dReleaseTime = releaseTime * 1000;
        _envelope.dSustainAmplitude = sustainAmplitude;
        _envelope.dStartAmplitude = startAmplitude;
        for (var i = 0; i < _waves.Length; i++)
        {
            _waves[i].Note = note;
            _waves[i].Type = waveType;
            _waves[i].SampleRate = _sampleRate;
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            _envelope.NoteOn(TimeInMilliseconds());
            Debug.Log("On time: " + _envelope.dTriggerOnTime);
            Debug.Log("Off time: " + _envelope.dTriggerOffTime);
            Debug.Log("Button on: " + _envelope.bNoteOn);
        }
        if (Input.GetKeyUp(KeyCode.B))
        {
            _envelope.NoteOff(TimeInMilliseconds());
            Debug.Log("On time: " + _envelope.dTriggerOnTime);
            Debug.Log("Off time: " + _envelope.dTriggerOffTime);
            Debug.Log("Button on: " + _envelope.bNoteOn);
        }
    }

    private double TimeInMilliseconds()
    {
        TimeSpan diff = DateTime.Now - startTime;
        return diff.TotalMilliseconds;
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        for (int i = 0; i < data.Length; i += channels)
        {
            for (int j = 0; j < channels; j++)
            {
//                foreach (Wave wave in _waves)
//                {
//                    (time, increment) = wave.Step();
//                }
                for (var k = 0; k < _waves.Length; k++)
                {
                    (time, timeStep, increment) = _waves[k].Step();
                }

                frequency = _waves[0].Frequency();

                data[i + j] = ((float) _envelope.GetAmplitude(TimeInMilliseconds())) * 0.2f * volume * Oscillate(_waves);
            }
        }
    }
    public static float Oscillate( Wave[] waves)
    {
        float value = Mathf.Sin(waves[0].TimeStep);
        for (var i = 0; i < waves.Length; i++)
        {
            float waveValue = 0;
            Wave wave = waves[i];
            switch (wave.Type)
            {
                case WaveType.Sine:
                    waveValue = wave.Amplitude * Sine(wave.TimeStep);
                    break;
                case WaveType.Square:
                    waveValue = wave.Amplitude * Square(wave.TimeStep);
                    break;
                case WaveType.Triangle:
                    waveValue = wave.Amplitude * Triangle(wave.TimeStep, wave.AngularVelocity());
                    break;
                case WaveType.Sawtooth:
                    waveValue = wave.Amplitude * Sawtooth(wave.TimeStep, wave.Frequency());
                    break;
                case WaveType.Noise:
                    waveValue = wave.Amplitude * Noise(wave.TimeStep);
                    break;
            }

            value = waveValue;
        }
        value /= waves.Length;
        return value;
    }

    private static float Mod(float a, float n)
    {
        return (a % n + n) % n;
    }
    
    public static float Sine(float dTime)
    {
        float value = Mathf.Sin(dTime);
        return value;
    }
    public static float Square(float dTime)
    {
        float value = Mathf.Sign(Mathf.Sin(dTime));
        return value;
    }
    public static float Triangle(float dTime, float angularVelocity)
    {
        float value = Mathf.Asin(Mathf.Sin(dTime));
        return value;
    }
    public static float Sawtooth(float dTime, float frequency)
    {
        float value = 0f;
        float positionInCurve = Mod(dTime, frequency);
        return 0.3f * positionInCurve;
    }
    public static float Noise(float dTime)
    {
         Random rand = new Random();
         float value = (float) rand.NextDouble() * 2 - 1;
        return value;
    }
}
