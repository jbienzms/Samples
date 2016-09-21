using UnityEngine;
using System.Collections;

public class Recording : MonoBehaviour
{
    [Tooltip("Audio source used to play the clip")]
    public AudioSource audioSource;

    [Tooltip("The visual used to indicate stop")]
    public GameObject stopVisual;

    [Tooltip("The visual used to indicate play")]
    public GameObject playVisual;

    // Member Variables
    private AudioClip audioClip;

    private void UpdateVisualState()
    {
        if ((IsRecording || IsPlaying))
        {
            stopVisual.SetActive(true);
            playVisual.SetActive(false);
        }
        else
        {
            stopVisual.SetActive(false);
            playVisual.SetActive(true);
        }
    }

    // Use this for initialization
    void Start()
    {
        UpdateVisualState();
    }

    void EndOfClip()
    {
        UpdateVisualState();
    }

    public void Select()
    {
        if (IsPlaying || IsRecording)
        {
            Stop();
        }
        else
        {
            Play();
        }
    }

    public void Play()
    {
        if ((audioClip != null) && (audioSource != null) && (!audioSource.isPlaying))
        {
            // Associate clip with source
            audioSource.clip = audioClip;

            // Start source playing
            audioSource.Play();

            // Update visual state
            UpdateVisualState();

            // Start callback
            Invoke("EndOfClip", audioClip.length + 0.5f);
        }
    }

    public void Record()
    {
        // If already recording, ignore
        if (IsRecording) { return; }

        // Now we're recording
        IsRecording = true;

        // Update visuals
        UpdateVisualState();

        // Start recording and update clip
        AudioClip = MicrophoneManager.Instance.StartRecording();
    }

    public void Stop()
    {
        if (IsRecording)
        {
            MicrophoneManager.Instance.StopRecording();
            IsRecording = false;
        }

        if (IsPlaying)
        {
            audioSource.Stop();
        }

        // Update visuals
        UpdateVisualState();
    }

    public AudioClip AudioClip
    {
        get
        {
            return audioClip;
        }
        set
        {
            if (audioClip != value)
            {
                audioClip = value;
                if (audioSource != null)
                {
                    audioSource.clip = value;
                }
            }
        }
    }

    public bool IsPlaying
    {
        get
        {
            return ((audioSource != null) && (audioSource.isPlaying));
        }
    }

    public bool IsRecording { get; private set; }
}
