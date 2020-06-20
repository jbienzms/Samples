// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Threading.Tasks;
using Unity.Collections;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.LightingTools
{
    public class CameraCaptureWebcam : ICameraCapture
    {
        /// <summary> A transform to reference for the position of the camera. </summary>
        private Transform        poseSource     = null;
        /// <summary> The WebCamTexture we're using to get a webcam image. </summary>
        private WebCamTexture    webcamTex      = null;
        /// <summary> Webcam device that we picked to read from. </summary>
        private WebCamDevice     device;
        /// <summary> When was this created? WebCamTexture isn't ready right way, so we'll need to wait a bit.</summary>
        private float            startTime      = 0;
        /// <summary> What field of view should this camera report? </summary>
        private float            fieldOfView    = 45;
        /// <summary>Preferred resolution for taking pictures, note that resolutions are not guaranteed! Refer to CameraResolution for details.</summary>
        private CameraResolution resolution     = null;
        /// <summary>Cache tex for storing the final image.</summary>
        private Texture          resizedTexture = null;

        /// <summary>
        /// Is the camera completely initialized and ready to begin taking pictures?
        /// </summary>
        public bool  IsInitialized
        {
            get
            {
                return webcamTex != null && webcamTex.isPlaying && (Time.time - startTime) > 0.5f;
            }
        }
        /// <summary>
        /// Is the camera currently already busy with taking a picture?
        /// </summary>
        public bool  IsRequestingImage
        {
            get;
            private set;
        }
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

        public CameraCaptureWebcam(Transform aPoseSource, float aFieldOfView)
        {
            poseSource  = aPoseSource;
            fieldOfView = aFieldOfView;
        }

        /// <summary>
        /// Starts up and selects a device's camera, and finds appropriate picture settings
        /// based on the provided resolution!
        /// </summary>
        /// <param name="preferGPUTexture">Do you prefer GPU textures, or do you prefer a NativeArray of colors? Certain optimizations may be present to take advantage of this preference.</param>
        /// <param name="preferredResolution">Preferred resolution for taking pictures, note that resolutions are not guaranteed! Refer to CameraResolution for details.</param>
        /// <returns>
        /// A <see cref="Task"/> that represents the operation.
        /// </returns>
        public Task InitializeAsync(bool preferGPUTexture, CameraResolution preferredResolution)
        {
            if (webcamTex != null)
            {
                throw new Exception("[CameraCapture] Only need to initialize once!");
            }
            // No cameras? Ditch out!
            if (WebCamTexture.devices.Length <= 0)
            {
                return Task.CompletedTask;
            }

            resolution = preferredResolution;

            // Find a rear facing camera we can use, or use the first one
            WebCamDevice[] devices = WebCamTexture.devices;
            device = devices[0];
            for (int i = 0; i < devices.Length; i++)
            {
                if (!devices[i].isFrontFacing || devices[i].name.ToLower().Contains("rear"))
                {
                    device = devices[i];
                }
            }

            // Pick a camera resolution
            if (resolution.nativeResolution == NativeResolutionMode.Largest)
            {
                webcamTex = new WebCamTexture(device.name, 10000, 10000, 2);
            }
            else if (resolution.nativeResolution == NativeResolutionMode.Smallest)
            {
                webcamTex = new WebCamTexture(device.name, 1, 1, 2);
            }
            else if (resolution.nativeResolution == NativeResolutionMode.Closest)
            {
                webcamTex = new WebCamTexture(device.name, resolution.size.x, resolution.size.y, 2);
            }
            else
            {
                throw new NotImplementedException(resolution.nativeResolution.ToString());
            }

            webcamTex.Play();

            startTime = Time.time;

            return Task.CompletedTask;
        }

        /// <summary>
        /// Requests an image from the camera and provide it as a Texture on the GPU and array of Colors on the CPU.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> that represents the operation.
        /// </returns>
        public Task<ColorResult> RequestColorAsync()
        {
            resolution.ResizeTexture(webcamTex, ref resizedTexture, true);
            ColorResult result = new ColorResult(poseSource == null ? Matrix4x4.identity : poseSource.localToWorldMatrix, (Texture2D)resizedTexture);
            return Task.FromResult(result);
        }

        /// <summary>
        /// Requests an image from the camera and provides it as a Texture on the GPU.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> that yields the <see cref="TextureResult"/>.
        /// </returns>
        public Task<TextureResult> RequestTextureAsync()
        {
            resolution.ResizeTexture(webcamTex, ref resizedTexture, false);
            TextureResult result = new TextureResult(poseSource == null ? Matrix4x4.identity : poseSource.localToWorldMatrix, (Texture2D)resizedTexture);
            return Task.FromResult(result);
        }

        /// <summary>
        /// Done with the camera, free up resources!
        /// </summary>
        public void Shutdown()
        {
            if (webcamTex != null && webcamTex.isPlaying)
            {
                webcamTex.Stop();
            }
            webcamTex = null;
        }
    }
}
