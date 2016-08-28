using UnityEngine;
using System.Collections;

public class MicrophoneInput : MonoBehaviour {

    AudioSource micAudio;

    public float rmsValue;
    public float dbValue;
    public float pitchValue;

    int qSamples = 64;
    float refValue = 0.1f;
    float threshold = 0.02f;

    float[] _samples;
    float[] _spectrum;
    float _fSample;

    void Start () {

        micAudio = GetComponent<AudioSource>();

        _samples = new float[qSamples];
        _spectrum = new float[qSamples];
        _fSample = AudioSettings.outputSampleRate;

        micAudio.clip = Microphone.Start(null, true, 1, 44100);
        //micAudio.mute = true;
        micAudio.Play();

        foreach (string device in Microphone.devices)
        {
            Debug.Log("Name: " + device);
        }
	}
	
	// Update is called once per frame
	void Update () {
        AnalyzeSound();
	}

    void AnalyzeSound()
    {
        micAudio.GetOutputData(_samples, 0);
        float sum = 0;
        for (int i = 0; i < qSamples; i++)
        {
            Debug.Log("Sample data: " + _samples[i]);
            sum = +_samples[i] * _samples[i];
        }
        //Debug.Log("sum is this: " + sum);
        rmsValue = Mathf.Sqrt(sum / qSamples);
        dbValue = 20 * Mathf.Log10(rmsValue / refValue);
        if (dbValue < -160)
        {
            dbValue = -160;
        }

        micAudio.GetSpectrumData(_spectrum, 0, FFTWindow.BlackmanHarris);
        float note;
        float maxV = 0;
        var maxN = 0;
        for (int i = 0; i < qSamples; i++)
        {
            //Debug.Log("spec: " + _spectrum[i]);
            if (!(_spectrum[i] > maxV) || !(_spectrum[i] > threshold))
            {
                continue;
            }
            maxV = _spectrum[i];
            maxN = i;

        }
        float freqN = maxN;
        if (maxN > 0 && maxN < qSamples - 1)
        {
            var dL = _spectrum[maxN - 1] / _spectrum[maxN];
            var dR = _spectrum[maxN + 1] / _spectrum[maxN];
            freqN += 0.5f * (dR * dR - dL * dL);

            pitchValue = freqN * (_fSample / 2) / qSamples;
        }
        //Debug.Log("this is the pitch roughly: " + pitchValue);
    }
}
