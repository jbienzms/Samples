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
    /// <summary>
    /// A wrapper for setting camera exposure settings in UWP.
    /// </summary>
    sealed class VideoDeviceControllerWrapperUWP : IDisposable
    {
        /// <summary>PhotoCapture native object.</summary>
        private VideoDeviceController controller = null;
        /// <summary>IDisposable pattern</summary>
        private bool                  disposed   = false;

        /// <param name="unknown">Pointer to a PhotoCapture camera.</param>
        public VideoDeviceControllerWrapperUWP(IntPtr unknown)
        {
            controller = (VideoDeviceController)Marshal.GetObjectForIUnknown(unknown);
        }

        ~VideoDeviceControllerWrapperUWP()
        {
            Dispose(false);
        }

        /// <summary>
        /// Manually override the camera's exposure.
        /// </summary>
        /// <param name="exposure">These appear to be imaginary units of some kind. Seems to be integer values around, but not exactly -10 to +10.</param>
        public async void SetExposure(int exposure)
        {
            //Debug.LogFormat("({3} : {0}-{1}) +{2}", controller.Exposure.Capabilities.Min, controller.Exposure.Capabilities.Max, controller.Exposure.Capabilities.Step, exposure);

            // Use exposure control or exposure?
            if (controller.ExposureControl.Supported)
            {
                Debug.Log("Setting exposure using ExposureControl.");

                // Get total possible exposure range as a TimeSpan
                TimeSpan range = (controller.ExposureControl.Max - controller.ExposureControl.Min);

                // Convert exposure passed in (-10 to 10) to floating point range (0.0 to 1.0)
                double exp = ((double)(exposure + 10)) / 20;

                // Convert exposure to milliseconds
                double expms = range.TotalMilliseconds * exp;

                // Convert exposure to TimeSpan (starting with Minimum and adding to it)
                TimeSpan expTime = controller.ExposureControl.Min + TimeSpan.FromMilliseconds(expms);


                // Update the controller
                try
                {
                    await controller.ExposureControl.SetAutoAsync(false);
                    await controller.ExposureControl.SetValueAsync(expTime);
                }
                catch (Exception ex)
                {
                    Debug.LogErrorFormat("[{0}] HoloLens locatable camera has failed to set exposure to {1}. {2}", typeof(VideoDeviceControllerWrapperUWP), expTime, ex);
                }
            }
            else
            {
                Debug.Log("Setting exposure using regular Exposure.");

                if (!controller.Exposure.TrySetAuto(false))
                {
                    Debug.LogErrorFormat("[{0}] HoloLens locatable camera has failed to set auto exposure off", typeof(VideoDeviceControllerWrapperUWP));
                }

                if (!controller.Exposure.TrySetValue((double)exposure))
                {
                    Debug.LogErrorFormat("[{0}] HoloLens locatable camera has failed to set exposure to {1} as requested", typeof(VideoDeviceControllerWrapperUWP), exposure);
                }
            }
        }

        /// <summary>
        /// Manually override the camera's white balance.
        /// </summary>
        /// <param name="kelvin">White balance temperature in kelvin! Also seems a bit arbitrary as to what values it accepts.</param>
        public void SetWhiteBalance(int kelvin)
        {
            if (!controller.WhiteBalance.TrySetAuto(false))
            {
                Debug.LogErrorFormat("[{0}] HoloLens locatable camera has failed to set auto WhiteBalance off", typeof(VideoDeviceControllerWrapperUWP));
            }

            if (!controller.WhiteBalance.TrySetValue((double)kelvin))
            {
                Debug.LogErrorFormat("[{0}] HoloLens locatable camera has failed to set WhiteBalance to {1} as requested", typeof(VideoDeviceControllerWrapperUWP), kelvin);
            }
        }
        /// <summary>
        /// Manually override the camera's ISO.
        /// </summary>
        /// <param name="iso">Camera's sensitivity to light, kinda like gain.</param>
        public void SetISO(int iso)
        {
            var task = controller.IsoSpeedControl.SetValueAsync((uint)iso);
        }

        /// <summary>
        /// Dispose of resources!
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.
        /// If that's confusing to you too, maybe read this: https://docs.microsoft.com/en-us/dotnet/api/system.idisposable.dispose
        /// </summary>
        public void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (controller != null)
                {
                    Marshal.ReleaseComObject(controller);
                    controller = null;
                }
                disposed = true;
            }
        }
    }
}

#endif