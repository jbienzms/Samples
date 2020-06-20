// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Windows.WebCam;

namespace Microsoft.MixedReality.Toolkit.LightingTools
{
    /// <summary>
    /// The result of a camera capture as colors.
    /// </summary>
    public class ColorResult : TextureResult
    {
        /// <summary>
        /// Initialize a new <see cref="ColorResult"/>.
        /// </summary>
        /// <param name="matrix">
        /// The camera matrix.
        /// </param>
        /// <param name="texture">
        /// The texture.
        /// </param>
        public ColorResult(Matrix4x4 matrix, Texture2D texture) : base(matrix, texture)
        {
            Colors = texture.GetRawTextureData<Color24>();
        }

        /// <summary>
        /// Gets the colors for the result.
        /// </summary>
        public NativeArray<Color24> Colors { get; }
    }

    /// <summary>
    /// The result of a camera capture as a texture.
    /// </summary>
    public class TextureResult
    {
        /// <summary>
        /// Initialize a new <see cref="TextureResult"/>.
        /// </summary>
        /// <param name="matrix">
        /// The camera matrix.
        /// </param>
        /// <param name="texture">
        /// The texture.
        /// </param>
        public TextureResult(Matrix4x4 matrix, Texture2D texture)
        {
            Matrix = matrix;
            Texture = texture;
        }

        /// <summary>
        /// Gets the camera matrix for the result.
        /// </summary>
        public Matrix4x4 Matrix { get; }

        /// <summary>
        /// Gets the texture for the result.
        /// </summary>
        public Texture2D Texture { get; }
    }

    #if WINDOWS_UWP
    public class CameraCaptureUWP : ICameraCapture
    {
        #region Member Variables
        private Texture2D cacheTex = null;
        private PhotoCapture camera = null;
        private CameraParameters cameraParams;
        private float fieldOfView = 45;
        private bool hasWarnedCameraMatrix = false;
        private Task initializeTask = null;
        private Task<TextureResult> requestImageTask;
        private CameraResolution resolution = null;
        private Texture2D resizedTex = null;
        private VideoDeviceControllerWrapperUWP wrapper;
        #endregion // Member Variables

        #region Internal Methods
        /// <summary>
        /// Captures an image from the camera.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> that yields a <see cref="TextureResult"/>.
        /// </returns>
        private async Task<TextureResult> CaptureImageAsync()
        {
            // Make sure we're initialized before proceeding
            EnsureInitialized();

            // Wait for the camera to start photo mode
            await camera.StartPhotoModeAsync(cameraParams);

            // Take a picture and get the result
            var takeResult = await camera.TakePhotoAsync();

            // Camera matrix is broken in some unity builds
            // See: https://forum.unity.com/threads/locatable-camera-not-working-on-hololens-2.831514/

            // Shortcut to frame
            var frame = takeResult.Frame;

            // Grab the camera matrix
            Matrix4x4 transform;
            if ((frame.hasLocationData) && (frame.TryGetCameraToWorldMatrix(out transform)))
            {
                transform[0, 2] = -transform[0, 2];
                transform[1, 2] = -transform[1, 2];
                transform[2, 2] = -transform[2, 2];
            }
            else
            {
                if (!hasWarnedCameraMatrix)
                {
                    hasWarnedCameraMatrix = true;
                    Debug.Log("[CameraCapture] Can't get camera matrix!!");
                }
                transform = Camera.main.transform.localToWorldMatrix;
            }

            Matrix4x4 proj;
            if (!frame.TryGetProjectionMatrix(out proj))
            {
                fieldOfView = Mathf.Atan(1.0f / proj[0, 0]) * 2.0f * Mathf.Rad2Deg;
            }

            frame.UploadImageDataToTexture(cacheTex);
            Texture tex = resizedTex;
            resolution.ResizeTexture(cacheTex, ref tex, true);
            resizedTex = (Texture2D)tex;

            // Wait for camera to stop
            await camera.StopPhotoModeAsync();

            // Pass on results
            return new TextureResult(transform, resizedTex);
        }

        /// <summary>
        /// Ensures that the camera system has been fully initialized, otherwise throws an exception.
        /// </summary>
        private void EnsureInitialized()
        {
            // Ensure initialized
            if (!IsInitialized) throw new InvalidOperationException($"{nameof(InitializeAsync)} must be completed first.");
        }

        /// <summary>
        /// Internal version of initialize. Should not be called more than once unless Shutdown has been called.
        /// </summary>
        /// <param name="preferGPUTexture">
        /// Whether GPU textures are preferred over NativeArray of colors. Certain optimizations may be present to take advantage of this preference.
        /// </param>
        /// <param name="preferredResolution">
        /// Preferred resolution for taking pictures. Note that resolutions are not guaranteed! Refer to CameraResolution for details.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> that represents the operation.
        /// </returns>
        private async Task InnerInitializeAsync(bool preferGPUTexture, CameraResolution preferredResolution)
        {
            // Store preferred resolution
            resolution = preferredResolution;

            // Find the nearest supported camera resolution to the preferred one
            Resolution cameraResolution = resolution.nativeResolution == NativeResolutionMode.Smallest ?
                                          PhotoCapture.SupportedResolutions.OrderBy((res) => res.width * res.height).First() :
                                          PhotoCapture.SupportedResolutions.OrderBy((res) => -res.width * res.height).First();

            // Create the texture cache
            cacheTex = new Texture2D(cameraResolution.width, cameraResolution.height);

            // Setup parameters for the camera
            cameraParams = new CameraParameters();
            cameraParams.hologramOpacity = 0.0f;
            cameraParams.cameraResolutionWidth = cameraResolution.width;
            cameraParams.cameraResolutionHeight = cameraResolution.height;
            cameraParams.pixelFormat = CapturePixelFormat.BGRA32;

            // Create the PhotoCapture camera
            camera = await CameraExtensions.CreateAsync(false);

            // Create the wrapper
            IntPtr unknown = camera.GetUnsafePointerToVideoDeviceController();
            wrapper = new VideoDeviceControllerWrapperUWP(unknown);

            // Set defaults for the camera
            await wrapper.SetExposureAsync(-7);
            await wrapper.SetWhiteBalanceAsync(5000);
            await wrapper.SetISOAsync(80);
        }
        #endregion // Internal Methods

        #region Public Methods
        /// <summary>
        /// Initializes a device's camera and finds appropriate picture settings based on the provided resolution.
        /// </summary>
        /// <param name="preferGPUTexture">
        /// Whether GPU textures are preferred over NativeArray of colors. Certain optimizations may be present to take advantage of this preference.
        /// </param>
        /// <param name="preferredResolution">
        /// Preferred resolution for taking pictures. Note that resolutions are not guaranteed! Refer to CameraResolution for details.
        /// </param>
        public Task InitializeAsync(bool preferGPUTexture, CameraResolution preferredResolution)
        {
            // Make sure not initialized or initializing
            if (initializeTask != null) throw new InvalidOperationException("Already initializing.");

            // Now initializing
            initializeTask = InnerInitializeAsync(preferGPUTexture, preferredResolution);

            // Return task in process
            return initializeTask;
        }

        /// <summary>
        /// Requests an image from the camera and provide it as a Texture on the GPU and array of Colors on the CPU.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> that represents the operation.
        /// </returns>
        public async Task<ColorResult> RequestColorAsync()
        {
            // Call texture overload
            var textureResult = await RequestTextureAsync();

            // Return color result
            return new ColorResult(textureResult.Matrix, textureResult.Texture);
        }

        /// <summary>
        /// Requests an image from the camera and provides it as a Texture on the GPU.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> that yields the <see cref="TextureResult"/>.
        /// </returns>
        public Task<TextureResult> RequestTextureAsync()
        {
            // Make sure we're initialized before proceeding
            EnsureInitialized();

            // If already requesting, just wait for it to complete
            if (requestImageTask != null && !requestImageTask.IsCompleted)
            {
                return requestImageTask;
            }
            else
            {
                requestImageTask = CaptureImageAsync();
                return requestImageTask;
            }
        }

        /// <summary>
        /// Done with the camera, free up resources!
        /// </summary>
        public void Shutdown()
        {
            if (wrapper != null)
            {
                wrapper.Dispose();
                camera = null;
            }

            if (cacheTex   != null)
            {
                GameObject.Destroy(cacheTex);
                cacheTex = null;
            }

            if (resizedTex != null)
            {
                GameObject.Destroy(resizedTex);
                resizedTex = null;
            }

            requestImageTask = null;
            initializeTask = null;
        }
        #endregion // Public Methods

        #region Public Properties
        /// <summary>
        /// Field of View of the camera in degrees. This value is never ready until after
        /// initialization, and in many cases, isn't accurate until after a picture has
        /// been taken. It's best to check this after each picture if you need it.
        /// </summary>
        public float FieldOfView
        {
            get
            {
                return fieldOfView;
            }
        }

        /// <summary>
        /// Is the camera completely initialized and ready to begin taking pictures?
        /// </summary>
        public bool IsInitialized => (initializeTask != null && initializeTask.IsCompleted && !initializeTask.IsFaulted);

        /// <summary>
        /// Is the camera currently already busy with taking a picture?
        /// </summary>
        public bool IsRequestingImage => (requestImageTask != null && !requestImageTask.IsCompleted);
        #endregion // Public Properties
    }
    #endif // WINDOWS_UWP
}
