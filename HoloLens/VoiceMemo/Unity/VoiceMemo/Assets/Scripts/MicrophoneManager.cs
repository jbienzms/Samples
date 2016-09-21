using HoloToolkit;
using HoloToolkit.Unity;
using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;

public class MicrophoneManager : Singleton<MicrophoneManager>
{
    [Tooltip("The (optional) KeywordManager to restart when recording has completed.")]
    public KeywordManager keywordToStart;

    // Using an empty string specifies the default microphone. 
    private static string deviceName = string.Empty;
    private int samplingRate;
    private const int messageLength = 10;

    void Awake()
    {
        // Query the maximum frequency of the default microphone. Use 'unused' to ignore the minimum frequency.
        int unused;
        Microphone.GetDeviceCaps(deviceName, out unused, out samplingRate);
    }

    /// <summary>
    /// Turns on the dictation recognizer and begins recording audio from the default microphone.
    /// </summary>
    /// <returns>The audio clip recorded from the microphone.</returns>
    public AudioClip StartRecording()
    {
        if (IsRecording) { throw new Exception("Already recording"); }

        // Shutdown the PhraseRecognitionSystem. This controls the KeywordRecognizers
        PhraseRecognitionSystem.Shutdown();

        // Set recording flag
        IsRecording = true;

        // Start recording from the microphone for 10 seconds
        return Microphone.Start(deviceName, false, messageLength, samplingRate);
    }

    /// <summary>
    /// Ends the recording session.
    /// </summary>
    public void StopRecording()
    {
        if (!IsRecording) { return; }

        // End the recording
        Microphone.End(deviceName);

        // If KeywordManager supplied, restart it
        if (keywordToStart)
        {
            keywordToStart.StartKeywordRecognizer();
        }

        // Not recording
        IsRecording = false;
    }

    /// <summary>
    /// Gets a value that indicates if a clip is being recorded.
    /// </summary>
    public bool IsRecording { get; private set; }
}
