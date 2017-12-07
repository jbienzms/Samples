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

#if !WINDOWS_UWP
using System.Security.Cryptography.X509Certificates;
#endif

#if WINDOWS_UWP
using Windows.Storage;
#endif


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
        private IFaceServiceClient faceServiceClient;
        private bool isCameraInitializing;
        private bool isCapturingPhoto;
        private PhotoCapture photoCapture;
        private Resolution selectedResolution;
        #endregion // Private Member Variables

        #region Unity Inspector Variables
        [SerializeField]
        [Tooltip("The subscription key for Cognitive Services Face API.")]
        private string faceApiKey = string.Empty;

        [SerializeField]
        [Tooltip("The URI endpoint for accessing the Face API (region must match your API key).")]
        private string faceApiUri = "https://westcentralus.api.cognitive.microsoft.com/face/v1.0";

        [SerializeField]
        [Tooltip("Whether the camera should be initialized when the manager is started.")]
        private bool initializeCameraOnStart = true;

        [SerializeField]
        [Tooltip("Whether the last captured photo should be saved to disk. On UWP the photo is saved to the Pictures library, which requires the PicturesLibrary capability.")]
        private bool saveLastPhoto = false;
        #endregion // Unity Inspector Variables

        #region Internal Methods
        #if !WINDOWS_UWP
        private bool CheckValidCertificateCallback(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            bool valid = true;

            // If there are errors in the certificate chain, look at each error to determine the cause.
            if (sslPolicyErrors != SslPolicyErrors.None)
            {
                for (int i = 0; i < chain.ChainStatus.Length; i++)
                {
                    if (chain.ChainStatus[i].Status != X509ChainStatusFlags.RevocationStatusUnknown)
                    {
                        chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                        chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                        chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                        chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                        bool chainIsValid = chain.Build((X509Certificate2)certificate);
                        if (!chainIsValid)
                        {
                            valid = false;
                        }
                    }
                }
            }
            return valid;
        }
        #endif
        #if WINDOWS_UWP
        private async Task WriteImageToDiskAsync(Stream imageStream)
        {
            Debug.Log("Getting camera roll");
            // Get Camera Roll folder
            StorageFolder folder = KnownFolders.CameraRoll;

            Debug.Log("Creating file");
            // Create file and overrwite
            StorageFile captureFile = await folder.CreateFileAsync("VisionCapture.jpg", CreationCollisionOption.ReplaceExisting);
            using (Stream storageStream = await captureFile.OpenStreamForWriteAsync())
            {
                Debug.Log("Rewinding stream");

                // Rewind image stream
                imageStream.Seek(0, SeekOrigin.Begin);

                Debug.Log("Writing file");
                // Copy
                await imageStream.CopyToAsync(storageStream);

                Debug.Log("Rewinding stream");

                // Rewind image stream
                imageStream.Seek(0, SeekOrigin.Begin);
            }
        }
        #endif
        #endregion // Internal Methods

        #region Unity Behavior Overrides
        void Awake()
        {
            Debug.Log("Ensure your Face API Key was generated for the region of your Face API URI or it might not work!!");
            faceServiceClient = new FaceServiceClient(faceApiKey, faceApiUri);
            #if !WINDOWS_UWP
            //This works, and one of these two options are required as Unity's TLS (SSL) support doesn't currently work like .NET
            //ServicePointManager.CertificatePolicy = new CustomCertificatePolicy();

            //This 'workaround' seems to work for the .NET Storage SDK, but not event hubs. 
            //If you need all of it (ex Storage, event hubs,and app insights) then consider using the above.
            //If you don't want to check an SSL certificate coming back, simply use the return true delegate below.
            //Also it may help to use non-ssl URIs if you have the ability to, until Unity fixes the issue (which may be fixed by the time you read this)
            ServicePointManager.ServerCertificateValidationCallback = CheckValidCertificateCallback; //delegate { return true; };
            #endif
        }

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
        private async void OnPhotoCaptured(PhotoCapture.PhotoCaptureResult photoResult, PhotoCaptureFrame photoFrame, VisionRecognitionOptions recognitionOptions, TaskCompletionSource<VisionCaptureResult> captureTaskSource)
        {
            Debug.Log("Photo captured.");
            // No longer capturing
            isCapturingPhoto = false;

            // Create a texture to hold the photo data
            Texture2D photoTexture = new Texture2D(selectedResolution.width, selectedResolution.height, TextureFormat.BGRA32, false);

            // Have the frame fill in the texture
            photoFrame.UploadImageDataToTexture(photoTexture);
            photoTexture.wrapMode = TextureWrapMode.Clamp;

            // Create result object
            VisionCaptureResult result = new VisionCaptureResult(photoResult, photoFrame, photoTexture);

            // Placeholder
            Face[] faces = null;

            // Only do the following work if we're saving the photo to disk or actually going to recognize something
            if (saveLastPhoto || recognitionOptions.DetectFaces) // || recognitionOptions.DetectObjects) 
            {
                // Convert the texture to a JPG byte array
                byte[] jpgBytes = ImageConversion.EncodeToJPG(photoTexture);

                // Temporary wrap memory stream
                using (MemoryStream imageStream = new MemoryStream(jpgBytes))
                {
                    // Save to disk?
                    if (saveLastPhoto)
                    {
                        #if WINDOWS_UWP
                        await WriteImageToDiskAsync(imageStream);
                        #endif
                    }

                    // Detect faces?
                    if (recognitionOptions.DetectFaces)
                    {
                        // Call API
                        faces = await faceServiceClient.DetectAsync(imageStream, returnFaceId: true, returnFaceLandmarks: false, returnFaceAttributes: recognitionOptions.FaceAttributes);

                        // Copy Faces to result collection
                        foreach (Face face in faces)
                        {
                            result.Recognitions.Add(new FaceRecognitionResult(face));
                        }
                    }

                    /*
                    // Detect custom objects?
                    if (recognitionOptions.DetectObjects)
                    {
                        // Call API
                        // Add to Results
                    }
                    */
                }
            }

            // Complete task
            captureTaskSource.SetResult(result);
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
        public async Task<VisionCaptureResult> CaptureAndRecognizeAsync(VisionRecognitionOptions recognitionOptions)
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
            TaskCompletionSource<VisionCaptureResult> captureTaskSource = new TaskCompletionSource<VisionCaptureResult>();

            // Start callback process
            photoCapture.TakePhotoAsync((photoResult, photoFrame) => OnPhotoCaptured(photoResult, photoFrame, recognitionOptions, captureTaskSource));

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
        /// Gets or sets the subscription key for Cognitive Services Face API.
        /// </summary>
        public string FaceApiKey { get { return faceApiKey; } set { faceApiKey = value; } }

        /// <summary>
        /// The URI endpoint for accessing the Face API.
        /// </summary>
        /// <remarks>
        /// IMPORTANT: The endpoint region must match your API key.
        /// </remarks>
        public string FaceApiUri { get { return faceApiUri; } set { faceApiUri = value; } }

        /// <summary>
        /// Gets or sets a value that indicates if the camera should be initialized when the manager is started.
        /// </summary>
        public bool InitializeCameraOnStart { get { return initializeCameraOnStart; } set { initializeCameraOnStart = value; } }
        /// <summary>
        /// Gets a value that indicates if the camera has been initialized.
        /// </summary>
        public bool IsCameraInitialized { get { return cameraInitTaskSource.Task.IsCompleted; } }

        /// <summary>
        /// Gets or sets a value that indicates if the last captured photo should be saved to disk.
        /// </summary>
        /// <remarks>
        /// This can be helpful for debugging purposes.
        /// </remarks>
        public bool SaveLastPhoto { get { return saveLastPhoto; } set { saveLastPhoto = value; } }
        #endregion // Public Properties

        #region Public Events
        /// <summary>
        /// Occurs when the camera has been initialized.
        /// </summary>
        public event EventHandler CameraInitialized;
        #endregion // Public Events
    }
}