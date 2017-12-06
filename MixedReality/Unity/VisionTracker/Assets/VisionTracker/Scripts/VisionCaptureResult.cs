using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.XR.WSA.WebCam;

namespace Microsoft.UnitySamples.Vision
{
    /// <summary>
    /// Represents the result of a vision recognition operation.
    /// </summary>
    public class VisionCaptureResult
    {
        #region Member Variables
        private PhotoCaptureFrame photoFrame;
        private int photoHeight;
        private PhotoCapture.PhotoCaptureResult photoResult;
        private int photoWidth;
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
        /// <param name="photoWidth">
        /// The width of the captured photo.
        /// </param>
        /// <param name="photoHeight">
        /// The height of the captured photo.
        /// </param>
        public VisionCaptureResult(PhotoCapture.PhotoCaptureResult photoResult, PhotoCaptureFrame photoFrame, int photoWidth, int photoHeight)
        {
            // Validate
            if (photoFrame == null) throw new ArgumentNullException(nameof(photoFrame));

            // Store
            this.photoResult = photoResult;
            this.photoFrame = photoFrame;
            this.photoWidth = photoWidth;
            this.photoHeight = photoHeight;
        }
        #endregion // Constructors

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
        /// Gets the height of the captured photo.
        /// </summary>
        public int PhotoHeight
        {
            get
            {
                return photoHeight;
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
        /// Gets the width of the captured photo.
        /// </summary>
        public int PhotoWidth
        {
            get
            {
                return photoWidth;
            }
        }
    }
}
