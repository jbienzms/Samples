using UnityEngine;
using System.Collections;
using HoloToolkit.Unity;
using UnityEngine.VR.WSA.Input;
using System;

public class MemoController : MonoBehaviour
{
    [Tooltip("The GameObject where recordings will be parented in the scene")]
    public GameObject recordingContainer;

    // Inspector Variables
    [Tooltip("The prefab that represents a single recording")]
    public GameObject recordingPrefab;

    // Private variables
    private GestureRecognizer gestureRecognizer;
    private Recording lastRecording;

    // Internal Methods
    private void FindGazeRecording(out RaycastHit hitInfo, out Recording recording)
    {
        // Defaults
        hitInfo = new RaycastHit();
        recording = null;

        // Shortcut
        GazeManager gm = GazeManager.Instance;

        // Hit?
        if (gm.Hit)
        {
            // Store the hit info
            hitInfo = gm.HitInfo;

            // Get the game object
            GameObject ob = gm.HitInfo.collider.gameObject;

            // See if it's ar recording
            if (ob != null)
            {
                recording = ob.GetComponentInParent<Recording>();
            }
        }
    }

    private void StartGestures()
    {
        // Create a new GestureRecognizer. Sign up for tapped events.
        gestureRecognizer = new GestureRecognizer();
        gestureRecognizer.SetRecognizableGestures(GestureSettings.Tap);

        gestureRecognizer.TappedEvent += GestureRecognizer_TappedEvent;

        // Start looking for gestures.
        gestureRecognizer.StartCapturingGestures();
    }

    private void GestureRecognizer_TappedEvent(InteractionSourceKind source, int tapCount, Ray headRay)
    {
        // Shortcut
        GazeManager gm = GazeManager.Instance;

        // Find existing recording?
        RaycastHit hitInfo;
        Recording rec;
        FindGazeRecording(out hitInfo, out rec);

        // If recording found, select it.
        if (rec != null)
        {
            rec.Select();
        }
        else
        {
            CreateRecording();
        }
    }

    public void CreateRecording()
    {
        // Default to camera
        Vector3 position = Camera.main.transform.position;

        // If surface position, use it instead.
        if (GazeManager.Instance.Hit)
        {
            position = GazeManager.Instance.HitInfo.point;
        }

        // If there was a previous recording
        if (lastRecording != null)
        {
            // If still recording, stop recording
            if (lastRecording.IsRecording)
            {
                lastRecording.Stop();
            }
        }

        // Create
        GameObject recordingGO = Instantiate(recordingPrefab);

        // Parent
        recordingGO.transform.parent = recordingContainer.transform;

        // Move it
        recordingGO.transform.position = position;

        // Get recording behavior
        Recording rec = recordingGO.GetComponentInChildren<Recording>();

        // Start recording
        rec.Record();

        // Remember as last recording
        lastRecording = rec;
    }

    public void DeleteRecording()
    {
        // Shortcut
        GazeManager gm = GazeManager.Instance;

        // Hit?
        if (gm.Hit)
        {
            // Get the game object
            GameObject ob = gm.HitInfo.collider.gameObject;

            // See if it's ar recording
            Recording r = ob.GetComponentInParent<Recording>();

            // If it's a recording destroy it
            if (r != null)
            {
                Destroy(ob);
            }
        }

    }

    // Use this for initialization
    void Start ()
    {
        StartGestures();
	}
}
