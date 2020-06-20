// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Threading.Tasks;
using Unity.Collections;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.LightingTools
{
    public interface ICameraCapture
    {
        /// <summary>
        /// Is the camera completely initialized and ready to begin taking pictures?
        /// </summary>
        bool  IsInitialized { get; }
        /// <summary>
        /// Is the camera currently already busy with taking a picture?
        /// </summary>
        bool  IsRequestingImage { get; }
        /// <summary>
        /// Field of View of the camera in degrees. This value is never ready until after
        /// initialization, and in many cases, isn't accurate until after a picture has
        /// been taken. It's best to check this after each picture if you need it.
        /// </summary>
        float FieldOfView { get; }

        /// <summary>
        /// Initializes a device's camera and finds appropriate picture settings based on the provided resolution.
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
        Task InitializeAsync(bool preferGPUTexture, CameraResolution preferredResolution);

        /// <summary>
        /// Requests an image from the camera and provide it as a Texture on the GPU and array of Colors on the CPU.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> that represents the operation.
        /// </returns>
        Task<ColorResult> RequestColorAsync();

        /// <summary>
        /// Requests an image from the camera and provides it as a Texture on the GPU.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> that yields the <see cref="TextureResult"/>.
        /// </returns>
        Task<TextureResult> RequestTextureAsync();

        /// <summary>
        /// Done with the camera, free up resources!
        /// </summary>
        void Shutdown();
    }
}