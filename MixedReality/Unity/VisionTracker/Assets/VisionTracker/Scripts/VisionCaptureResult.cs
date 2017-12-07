using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.XR.WSA.WebCam;
using Microsoft.ProjectOxford.Face.Contract;
using UnityEngine;

namespace Microsoft.UnitySamples.Vision
{
    /// <summary>
    /// Represents the result of a vision recognition operation.
    /// </summary>
    public class VisionCaptureResult
    {
        #region Member Variables
        private Face[] faces;
        private PhotoCaptureFrame photoFrame;
        private PhotoCapture.PhotoCaptureResult photoResult;
        private Texture2D photoTexture;
        #endregion // Member Variables

        #region Constructors
        /// <summary>
        /// Initializes a new <see cref="VisionCaptureResult"/>.
        /// </summary>
        /// <param name="photoResult">
        /// The result of the photo capture operation.
        /// </param>
        /// <param name="photoFrame">
        /// The photo frame captured during the operation.
        /// </param>
        /// <param name="photoTexture">
        /// The <see cref="Texture2D"/> that contains the photo.
        /// </param>
        /// <param name="faces">
        /// The array of faces recognized in the photo.
        /// </param>
        public VisionCaptureResult(PhotoCapture.PhotoCaptureResult photoResult, PhotoCaptureFrame photoFrame, Texture2D photoTexture, Face[] faces)
        {
            // Validate
            if (photoFrame == null) throw new ArgumentNullException(nameof(photoFrame));

            // Store
            this.photoResult = photoResult;
            this.photoFrame = photoFrame;
            this.photoTexture = photoTexture;
            this.faces = faces;
        }
        #endregion // Constructors

        #region Public Properties
        /// <summary>
        /// Gets the array of faces recognized in the photo.
        /// </summary>
        public Face[] Faces
        {
            get
            {
                return faces;
            }
        }

        /// <summary>
        /// Gets the photo frame captured during the operation.
        /// </summary>
        public PhotoCaptureFrame PhotoFrame
        {
            get
            {
                return photoFrame;
            }
        }

        /// <summary>
        /// Gets the result of the photo capture operation.
        /// </summary>
        public PhotoCapture.PhotoCaptureResult PhotoResult
        {
            get
            {
                return photoResult;
            }
        }

        /// <summary>
        /// The <see cref="Texture2D"/> that contains the photo.
        /// </summary>
        public Texture2D PhotoTexture
        {
            get
            {
                return photoTexture;
            }
        }
        #endregion // Public Properties
    }
}
