using Microsoft.ProjectOxford.Face.Contract;
using System;
using UnityEngine;
using UnityEngine.XR.WSA.WebCam;

namespace Microsoft.UnitySamples.Vision
{
    /// <summary>
    /// The base class for all types of objects that can be recognized.
    /// </summary>
    public abstract class RecognitionResult
    {
        #region Member Variables
        private Rect location = new Rect();
        #endregion // Member Variables

        #region Public Properties
        /// <summary>
        /// Gets or sets the location of the recognized object.
        /// </summary>
        public Rect Location { get { return location; } set { location = value; } }
        #endregion // Public Properties
    }

    /// <summary>
    /// A recognition result for a human face.
    /// </summary>
    public class FaceRecognitionResult : RecognitionResult
    {
        #region Member Variables
        private Face face;
        #endregion // Member Variables

        #region Constants
        /// <summary>
        /// Initializes a new <see cref="FaceRecognitionResult"/> for the specified face.
        /// </summary>
        /// <param name="face">
        /// The face that was recognized.
        /// </param>
        public FaceRecognitionResult(Face face)
        {
            // Validate
            if (face == null) throw new ArgumentNullException(nameof(face));

            // Store
            this.face = face;

            // Set common location
            FaceRectangle faceRect = face.FaceRectangle;
            this.Location = new Rect(faceRect.Left, faceRect.Top, faceRect.Width, faceRect.Height);
        }
        #endregion // Constants

        #region Public Properties
        /// <summary>
        /// Gets information about the face that was recognized.
        /// </summary>
        public Face Face
        {
            get
            {
                return face;
            }
        }
        #endregion // Public Properties
    }
}