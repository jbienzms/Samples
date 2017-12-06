using Microsoft.ProjectOxford.Common.Contract;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR.WSA.WebCam;


namespace Microsoft.UnitySamples.Vision
{
    /// <summary>
    /// Manages the connection between a Unity project and Cognitive Vision services.
    /// </summary>
    public class VisionManager : MonoBehaviour
    {
        #region Private Member Variables
        private TaskCompletionSource<bool> cameraInitTaskSource = new TaskCompletionSource<bool>(); // Task source that represents the initialization of the camera.
        private CameraParameters cameraParameters;
        private TaskCompletionSource<VisionCaptureResult> captureTaskSource;
        private bool isCameraInitializing;
        private bool isCapturingPhoto;
        private PhotoCapture photoCapture;
        private Resolution selectedResolution;
        #endregion // Private Member Variables

        #region Unity Inspector Variables
        [SerializeField]
        [Tooltip("Whether the camera should be initialized when the manager is started.")]
        private bool initializeCameraOnStart = true;
        #endregion // Unity Inspector Variables

        private Face[] CreateFakeFaces()
        {
            List<Face> faces = new List<Face>()
            {
                new Face()
                {
                    FaceRectangle = new FaceRectangle()
                    {
                        Left = 100,
                        Top = 100,
                        Width = 100,
                        Height = 100
                    }
                },

                new Face()
                {
                    FaceRectangle = new FaceRectangle()
                    {
                        Left = 300,
                        Top = 400,
                        Width = 100,
                        Height = 100
                    }
                }
            };

            return faces.ToArray();
        }

        #region Unity Behavior Overrides
        private void Start()
        {
            if (initializeCameraOnStart)
            {
                InitializeCameraAsync();
            }
        }
        #endregion // Unity Behavior Overrides


        #region Camera Initialization Callbacks
        private void OnCreatedPhotoCapture(PhotoCapture captureObject)
        {
            photoCapture = captureObject;
            photoCapture.StartPhotoModeAsync(cameraParameters, OnStartPhotoMode);
        }

        private void OnStartPhotoMode(PhotoCapture.PhotoCaptureResult result)
        {
            Debug.Log("Camera Initialized.");

            // Clear the initializing flag
            isCameraInitializing = false;

            // Complete the task
            cameraInitTaskSource.SetResult(true);

            // Notify event subscribers
            if (CameraInitialized != null) { CameraInitialized(this, EventArgs.Empty); }
        }
        #endregion // Camera Initialization Callbacks

        #region Photo Capture Callbacks
        private void OnPhotoCaptured(PhotoCapture.PhotoCaptureResult photoResult, PhotoCaptureFrame photoFrame)
        {
            Debug.Log("Photo captured.");

            // Create result object
            VisionCaptureResult result = new VisionCaptureResult(photoResult, photoFrame, selectedResolution.width, selectedResolution.height, CreateFakeFaces());

            // Copy task source locally
            TaskCompletionSource<VisionCaptureResult> source = captureTaskSource;
            captureTaskSource = null;
            
            // No longer capturing
            isCapturingPhoto = false;

            // Complete task
            source.SetResult(result);
        }
        #endregion // Photo Capture Callbacks


        #region Public Methods
        /// <summary>
        /// Captures a photo and recognizes objects.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> that yields a <see cref="VisionCaptureResult"/>.
        /// </returns>
        /// <remarks>
        /// If the camera has not yet been initialzied, it will be initialized as part of this call.
        /// </remarks>
        public async Task<VisionCaptureResult> CaptureAndRecognizeAsync()
        {
            // Can only capture and analyze one photo at a time.
            if (isCapturingPhoto)
            {
                throw new InvalidOperationException("A vision capture operation is already in progress.");
            }
            
            // If not yet initialized, initialize now
            if (!IsCameraInitialized)
            {
                await InitializeCameraAsync();
            }

            // Now capture a photo
            isCapturingPhoto = true;
            Debug.Log("Capturing photo...");

            // Create task source for completion
            captureTaskSource = new TaskCompletionSource<VisionCaptureResult>();

            // Start callback process
            photoCapture.TakePhotoAsync(OnPhotoCaptured);

            // Return task, which will be completed in callback
            return await captureTaskSource.Task;
        }

        /// <summary>
        /// Initializes the camera and prepares it to capture frames.
        /// </summary>
        public Task InitializeCameraAsync()
        {
            // Don't initialize more than once
            if (isCameraInitializing || IsCameraInitialized) { return cameraInitTaskSource.Task; }

            // Now initializing
            isCameraInitializing = true;
            Debug.Log("Initializing Camera...");

            // Get all resolutions
            List<Resolution> resolutions = new List<Resolution>(PhotoCapture.SupportedResolutions);

            // Use first available
            selectedResolution = resolutions[0];

            // Create camera parameters
            cameraParameters = new CameraParameters(WebCamMode.PhotoMode);
            cameraParameters.cameraResolutionWidth = selectedResolution.width;
            cameraParameters.cameraResolutionHeight = selectedResolution.height;
            cameraParameters.hologramOpacity = 0.0f;
            cameraParameters.pixelFormat = CapturePixelFormat.BGRA32;

            // Create the PhotoCapture instance using callback
            PhotoCapture.CreateAsync(false, OnCreatedPhotoCapture);

            // Return our task source task which will be marked completed by the callback
            return cameraInitTaskSource.Task;
        }
        #endregion // Public Methods

        #region Public Properties
        /// <summary>
        /// Gets or sets a value that indicates if the camera should be initialized when the manager is started.
        /// </summary>
        public bool InitializeCameraOnStart => initializeCameraOnStart;
        /// <summary>
        /// Gets a value that indicates if the camera has been initialized.
        /// </summary>
        public bool IsCameraInitialized { get { return cameraInitTaskSource.Task.IsCompleted; } }
        #endregion // Public Properties

        #region Public Events
        /// <summary>
        /// Occurs when the camera has been initialized.
        /// </summary>
        public event EventHandler CameraInitialized;
        #endregion // Public Events
    }
}