using System;
using UnityEngine;

public class MicrophoneRecorder : MonoBehaviour
{
    public float multiplier = 1000;
    public float cameraClosestDistance = 5;
    public float cameraShake = 10;
    public float lineOffsetY = 5;

    public Transform cameraRoot;
    public AudioSource audioSource;
    public string microphoneDevice; // Optional: specify a particular microphone
    public int numSamples = 1024; // Must be a power of 2
    private float[] spectrumData;
    public LineRenderer lineRenderer;
    public int bucketSize = 4;
    public float spacing = 0.08f;


    private void Awake()
    {
        spectrumData = new float[numSamples];
        lineRenderer.positionCount = (numSamples / bucketSize) * 2 + 1;
    }

    void Start()
    {
        // Get the default microphone device if not specified
        if (string.IsNullOrEmpty(microphoneDevice) && Microphone.devices.Length > 0)
        {
            microphoneDevice = Microphone.devices[0];
        }

        if (!string.IsNullOrEmpty(microphoneDevice))
        {
            // Start recording from the microphone
            audioSource.clip = Microphone.Start(microphoneDevice, true, 10, AudioSettings.outputSampleRate);

            audioSource.loop = true; // Loop the audio clip
            while (!(Microphone.GetPosition(microphoneDevice) > 0)) { } // Wait until recording starts
            audioSource.Play(); // Play the recorded audio
        }
        else
        {
            Debug.LogError("No microphone devices found or specified.");
        }
    }

    private void Update()
    {
        if (audioSource.isPlaying)
        {
            float overallAverage = 0;
            float lowsAverage = 0;


            audioSource.GetSpectrumData(spectrumData, 0, FFTWindow.Rectangular);

            float[] averages = new float[numSamples / bucketSize];
            Vector3[] posArr = new Vector3[(averages.Length * 2) +1];

            

            for (int i = 1; i < numSamples; i++) {
                averages[i / 8] += spectrumData[i];
                overallAverage += spectrumData[i];
                if (i < 24)
                {
                    lowsAverage+= spectrumData[i];
                }
            }
            lowsAverage /= 24;
            overallAverage /= spectrumData.Length;

            for (int i = 0; i < averages.Length; i++)
            {
                posArr[averages.Length - i - 1] = new Vector3(i* spacing, Mathf.Log10(averages[i] + 1) * multiplier, 0f);
                posArr[averages.Length + i + 1] = new Vector3(-i * spacing, -Mathf.Log10(averages[i] + 1) * multiplier, 0f);

            }

            lineRenderer.SetPositions(posArr);



            lineRenderer.transform.localPosition = new Vector3(0, lineOffsetY + (overallAverage * multiplier * 10),0);
            Camera.main.transform.localPosition = new Vector3(0, 5.3f, cameraClosestDistance + (lowsAverage * multiplier * cameraShake));

            // Now, 'spectrumData' contains the frequency amplitudes.
            // You can use this data to visualize the audio spectrum.
            // For example, you can create a series of UI elements or 3D objects
            // and adjust their size or position based on the spectrumData values.
        }
    }

    void OnDisable()
    {
        // Stop recording when the script is disabled or application quits
        if (!string.IsNullOrEmpty(microphoneDevice) && Microphone.IsRecording(microphoneDevice))
        {
            Microphone.End(microphoneDevice);
        }
    }
}