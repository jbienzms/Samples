// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
#if WINDOWS_UWP
using System;
using System.Linq;
using Unity.Collections;
using UnityEngine;

using System.Runtime.InteropServices;

using Windows.Media.Devices;

namespace Microsoft.MixedReality.Toolkit.LightingTools
{
	public class CameraCaptureUWP : ICameraCapture
	{
		#region Fields
		private UnityEngine.Windows.WebCam.PhotoCapture     camera      = null;
		private UnityEngine.Windows.WebCam.CameraParameters cameraParams;
		private CameraResolution resolution  = null;
		private Texture2D        cacheTex    = null;
		private Texture2D        resizedTex  = null;
		private bool             isReady     = false;
		private float            fieldOfView = 45;

		/// <summary>
		/// Is the camera completely initialized and ready to begin taking pictures?
		/// </summary>
		public bool  IsReady           { get { return isReady; } }
		/// <summary>
		/// Is the camera currently already busy with taking a picture?
		/// </summary>
		public bool  IsRequestingImage { get; private set; }
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
		public int   Exposure
		{
			set
			{ 
				IntPtr unknown = camera.GetUnsafePointerToVideoDeviceController();
				using (VideoDeviceControllerWrapperUWP wrapper = new VideoDeviceControllerWrapperUWP(unknown))
				{
					wrapper.SetExposure(value);
				}
			} 
		}
		public int   Whitebalance 
		{
			set
			{ 
				IntPtr unknown = camera.GetUnsafePointerToVideoDeviceController();
				using (VideoDeviceControllerWrapperUWP wrapper = new VideoDeviceControllerWrapperUWP(unknown))
				{
					wrapper.SetWhiteBalance(value);
				}
			} 
		}
		#endregion

		#region Methods
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
			Resolution cameraResolution = resolution.nativeResolution == NativeResolutionMode.Smallest ?
				UnityEngine.Windows.WebCam.PhotoCapture.SupportedResolutions.OrderBy((res) =>  res.width * res.height).First() :
				UnityEngine.Windows.WebCam.PhotoCapture.SupportedResolutions.OrderBy((res) => -res.width * res.height).First();

			cacheTex = new Texture2D(cameraResolution.width, cameraResolution.height);

			// Create a PhotoCapture object
			UnityEngine.Windows.WebCam.PhotoCapture.CreateAsync(false, delegate(UnityEngine.Windows.WebCam.PhotoCapture captureObject)
			{
				camera = captureObject;
				cameraParams = new UnityEngine.Windows.WebCam.CameraParameters();
				cameraParams.hologramOpacity        = 0.0f;
				cameraParams.cameraResolutionWidth  = cameraResolution.width;
				cameraParams.cameraResolutionHeight = cameraResolution.height;
				cameraParams.pixelFormat            = UnityEngine.Windows.WebCam.CapturePixelFormat.BGRA32;
			
				IntPtr unknown = camera.GetUnsafePointerToVideoDeviceController();
				using (VideoDeviceControllerWrapperUWP wrapper = new VideoDeviceControllerWrapperUWP(unknown))
				{
					wrapper.SetExposure(-7);
					wrapper.SetWhiteBalance(5000);
					wrapper.SetISO(80);
				}

				if (aOnInitialized != null)
				{
					aOnInitialized();
				}

				isReady = true;
			});
		}
		
		private void GetImage(Action<Texture2D, Matrix4x4> aOnFinished)
		{
			IsRequestingImage = true;
		
			if (camera == null)
			{
				Debug.LogError("[CameraCapture] camera hasn't been initialized!");
			}

			camera.StartPhotoModeAsync(cameraParams, startResult =>
			{
				camera.TakePhotoAsync((photoResult, frame) =>
				{
					// Grab the camera matrix
					Matrix4x4 transform;
					if (!frame.TryGetCameraToWorldMatrix(out transform))
					{
						Debug.Log("[CameraCapture] Can't get camera matrix!!");
						transform = Camera.main.transform.localToWorldMatrix;
					}
					else
					{
						transform[0,2] = -transform[0,2];
						transform[1,2] = -transform[1,2];
						transform[2,2] = -transform[2,2];
						//transform = transform; //transform * Camera.main.transform.localToWorldMatrix.inverse;
					}
					Matrix4x4 proj;
					if (!frame.TryGetProjectionMatrix(out proj))
					{
						fieldOfView = Mathf.Atan(1.0f / proj[0,0] ) * 2.0f * Mathf.Rad2Deg;
					}

					frame.UploadImageDataToTexture(cacheTex);
					Texture tex = resizedTex;
					resolution.ResizeTexture(cacheTex, ref tex, true);
					resizedTex = (Texture2D)tex;

					if (aOnFinished != null)
					{
						aOnFinished(resizedTex, transform);
					}

					camera.StopPhotoModeAsync((a)=>
					{
						IsRequestingImage = false;
					});
				});
			});
		}

		/// <summary>
		/// Request an image from the camera, and provide it as an array of colors on the CPU!
		/// </summary>
		/// <param name="onImageAcquired">This is the function that will be called when the image is ready. Matrix is the transform of the device when the picture was taken, and integers are width and height of the NativeArray.</param>
		public void RequestImage(Action<NativeArray<Color24>, Matrix4x4, int, int> aOnImageAcquired)
		{
			if (!isReady || IsRequestingImage)
			{
				return;
			}

			GetImage((tex, transform) =>
			{
				if (aOnImageAcquired != null)
				{
					aOnImageAcquired(tex.GetRawTextureData<Color24>(), transform, tex.width, tex.height);
				}
			});
		}
		/// <summary>
		/// Request an image from the camera, and provide it as a GPU Texture!
		/// </summary>
		/// <param name="onImageAcquired">This is the function that will be called when the image is ready. Texture is not guaranteed to be a Texture2D, could also be a WebcamTexture. Matrix is the transform of the device when the picture was taken.</param>
		public void RequestImage(Action<Texture, Matrix4x4> aOnImageAcquired)
		{
			if (!isReady || IsRequestingImage)
			{
				return;
			}

			GetImage((tex, transform) =>
			{
				if (aOnImageAcquired != null)
				{
					aOnImageAcquired(tex, transform);
				}
			});
		}

		/// <summary>
		/// Done with the camera, free up resources!
		/// </summary>
		public void Shutdown()
		{
			if (cacheTex   != null)
			{
				GameObject.Destroy(cacheTex);
			}
			if (resizedTex != null)
			{
				GameObject.Destroy(resizedTex);
			}

			if (camera != null)
			{
				camera.Dispose();
			}
			camera = null;
		}
		#endregion
	}
}
#endif