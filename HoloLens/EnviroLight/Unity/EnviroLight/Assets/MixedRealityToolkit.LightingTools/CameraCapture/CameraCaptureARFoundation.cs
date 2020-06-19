// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if USE_ARFOUNDATION

using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace Microsoft.MixedReality.Toolkit.LightingTools
{
	public class CameraCaptureARFoundation : ICameraCapture
	{
		/// <summary>Preferred resolution for taking pictures, note that resolutions are not guaranteed! Refer to CameraResolution for details.</summary>
		private CameraResolution resolution;
		/// <summary>Texture cache for storing captured images.</summary>
		private Texture2D        captureTex;
		/// <summary>Is this ICameraCapture ready for capturing pictures?</summary>
		private bool             ready = false;

        private ARCameraManager cameraManager;
        private Matrix4x4 lastProjMatrix;
        private Matrix4x4 lastDisplayMatrix;

        /// <summary>
        /// Is the camera completely initialized and ready to begin taking pictures?
        /// </summary>
        public bool  IsReady
		{
			get
			{
				return ready;
			}
		}
		/// <summary>
		/// Is the camera currently already busy with taking a picture?
		/// </summary>
		public bool  IsRequestingImage
		{
			get
			{
				return false;
			}
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
				float fov = Mathf.Atan(1.0f / lastProjMatrix[1,1] ) * 2.0f * Mathf.Rad2Deg;
				return fov;
			}
		}

		/// <summary>
		/// Starts up and selects a device's camera, and finds appropriate picture settings
		/// based on the provided resolution! 
		/// </summary>
		/// <param name="preferGPUTexture">Do you prefer GPU textures, or do you prefer a NativeArray of colors? Certain optimizations may be present to take advantage of this preference.</param>
		/// <param name="resolution">Preferred resolution for taking pictures, note that resolutions are not guaranteed! Refer to CameraResolution for details.</param>
		/// <param name="onInitialized">When the camera is initialized, this callback is called! Some cameras may return immediately, others may take a while. Can be null.</param>
		public void Initialize(bool aPreferGPUTexture, CameraResolution aResolution, Action aOnInitialized)
		{
			resolution = aResolution;
            lastProjMatrix = Matrix4x4.identity;
            lastDisplayMatrix = Matrix4x4.identity;

            Action<ARSessionStateChangedEventArgs> handler = null;
			handler = (aState) =>
			{
				// Camera and orientation data aren't ready until ARFoundation is actually tracking!
				if (aState.state == ARSessionState.SessionTracking)
				{
                    ARSession.stateChanged -= handler;
					ready = true;
					if (aOnInitialized != null)
					{
						aOnInitialized();
					}
				}
			};
            ARSession.stateChanged += handler;
            
            cameraManager = GameObject.FindObjectOfType<ARCameraManager>();
            if (cameraManager == null) { 
                Debug.LogError("CameraCapture needs an ARCameraManager to be present in the scene!");
                return;
            }
            cameraManager.frameReceived += OnFrameReceived;
        }
		
        void OnFrameReceived(ARCameraFrameEventArgs args)
        {
            if (args.projectionMatrix.HasValue)
                lastProjMatrix = args.projectionMatrix.Value;
            if (args.displayMatrix.HasValue)
                lastDisplayMatrix = args.displayMatrix.Value;
        }

		/// <summary>
		/// Gets the image data from ARFoundation, preps it, and drops it into captureTex.
		/// </summary>
		/// <param name="aOnFinished">Gets called when this method is finished with getting the image.</param>
		private void GrabScreen(Action aOnFinished)
		{
            
            // Grab the latest image from ARFoundation
            XRCameraImage image;
			if (!cameraManager.TryGetLatestImage(out image))
			{
				Debug.LogError("[CameraCaptureARFoundation] Could not get latest image!");
				return;
			}
			
			// Set up resizing parameters
			Vector2Int size = resolution.AdjustSize(new Vector2Int( image.width, image.height ));
			var conversionParams = new XRCameraImageConversionParams
			{
				inputRect        = new RectInt(0, 0, image.width, image.height),
				outputDimensions = new Vector2Int(size.x, size.y),
				outputFormat     = TextureFormat.RGB24,
				transformation   = CameraImageTransformation.MirrorY,
			};
			
			// make sure we have a texture to store the resized image
			if (captureTex == null || captureTex.width != size.x || captureTex.height != size.y)
			{
				if (captureTex != null)
				{
					GameObject.Destroy(captureTex);
				}
				captureTex = new Texture2D(size.x, size.y, TextureFormat.RGB24, false);
			}
			
			// And do the resize!
			image.ConvertAsync(conversionParams, (status, p, data) =>
			{
				if (status == AsyncCameraImageConversionStatus.Ready) {
					captureTex.LoadRawTextureData(data);
					captureTex.Apply();
				}
				if (status == AsyncCameraImageConversionStatus.Ready || status == AsyncCameraImageConversionStatus.Failed) {
					image.Dispose();
					aOnFinished();
				}
			});
		}

		/// <summary>
		/// Gets the camera's current transform in Unity coordinates, including any magic or trickery for offsets from ARFoundation.
		/// </summary>
		/// <returns>Camera's current transform in Unity coordinates.</returns>
		private Matrix4x4 GetCamTransform()
		{
			Matrix4x4 matrix = lastDisplayMatrix;

			// This matrix transforms a 2D UV coordinate based on the device's orientation.
			// It will rotate, flip, but maintain values in the 0-1 range. This is technically
			// just a 3x3 matrix stored in a 4x4

			// These are the matrices provided in specific phone orientations:

			#if UNITY_ANDROID

			// 1 0 0 Landscape Left (upside down)
			// 0 1 0
			// 0 0 0 
			if (Mathf.RoundToInt(matrix[0,0]) == 1 && Mathf.RoundToInt(matrix[1,1]) == 1)
			{
				matrix = Matrix4x4.Rotate( Quaternion.Euler(0,0,180) );
			}

			//-1 0 1 Landscape Right
			// 0-1 1
			// 0 0 0
			else if (Mathf.RoundToInt(matrix[0,0]) == -1 && Mathf.RoundToInt(matrix[1,1]) == -1)
			{
				matrix = Matrix4x4.identity;
			}

			// 0 1 0 Portrait
			//-1 0 1
			// 0 0 0
			else if (Mathf.RoundToInt(matrix[0,1]) == 1 && Mathf.RoundToInt(matrix[1,0]) == -1)
			{
				matrix = Matrix4x4.Rotate( Quaternion.Euler(0,0,90) );
			}

			// 0-1 1 Portrait (upside down)
			// 1 0 0
			// 0 0 0
			else if (Mathf.RoundToInt(matrix[0,1]) == -1 && Mathf.RoundToInt(matrix[1,0]) == 1)
			{
				matrix = Matrix4x4.Rotate( Quaternion.Euler(0,0,-90) );
			}

			#elif UNITY_IOS

			// 0-.6 0 Portrait
			//-1  1 0 The source image is upside down as well, so this is identity
			// 1 .8 1 
			if (Mathf.RoundToInt(matrix[0,0]) == 0)
			{
				matrix = Matrix4x4.Rotate( Quaternion.Euler(0,0,90) );
			}

			//-1  0 0 Landscape Right
			// 0 .6 0
			// 1 .2 1
			else if (Mathf.RoundToInt(matrix[0,0]) == -1)
			{
				matrix = Matrix4x4.identity;
			}

			// 1  0 0 Landscape Left
			// 0-.6 0
			// 0 .8 1
			else if (Mathf.RoundToInt(matrix[0,0]) == 1)
			{
				matrix = Matrix4x4.Rotate( Quaternion.Euler(0,0,180) );
			}

			// iOS has no upside down?
			#else

			if (true)
			{
			}
			#endif
			
			else
			{
				#pragma warning disable 0162
				Debug.LogWarningFormat("Unexpected Matrix provided from ARFoundation!\n{0}", matrix.ToString());
				#pragma warning restore 0162
			}
			
			return Camera.main.transform.localToWorldMatrix * matrix;
		}
		
		/// <summary>
		/// Request an image from the camera, and provide it as an array of colors on the CPU!
		/// </summary>
		/// <param name="onImageAcquired">This is the function that will be called when the image is ready. Matrix is the transform of the device when the picture was taken, and integers are width and height of the NativeArray.</param>
		public void RequestImage(Action<NativeArray<Color24>, Matrix4x4, int, int> aOnImageAcquired)
		{
			Matrix4x4 transform = GetCamTransform();
			GrabScreen(()=>
			{ 
				if (aOnImageAcquired != null)
				{
					aOnImageAcquired(captureTex.GetRawTextureData<Color24>(), transform, captureTex.width, captureTex.height);
				}
			});
		}
		/// <summary>
		/// Request an image from the camera, and provide it as a GPU Texture!
		/// </summary>
		/// <param name="onImageAcquired">This is the function that will be called when the image is ready. Texture is not guaranteed to be a Texture2D, could also be a WebcamTexture. Matrix is the transform of the device when the picture was taken.</param>
		public void RequestImage(Action<Texture, Matrix4x4> aOnImageAcquired)
		{
			Matrix4x4 transform = GetCamTransform();
			GrabScreen(()=>
			{
				if (aOnImageAcquired != null)
				{
					aOnImageAcquired(captureTex, transform);
				}
			});
		}

		/// <summary>
		/// Done with the camera, free up resources!
		/// </summary>
		public void Shutdown()
		{
			if (captureTex != null)
			{
				GameObject.Destroy(captureTex);
			}
		}
	}
}

#endif