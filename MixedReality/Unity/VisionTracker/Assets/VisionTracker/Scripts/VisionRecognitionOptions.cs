using Microsoft.ProjectOxford.Face;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.UnitySamples.Vision
{
    /// <summary>
    /// Defines the options that can be used to configure which items will be recognized.
    /// </summary>
    public class VisionRecognitionOptions
    {
        #region Member Variables
        private bool detectFaces = true;
        private FaceAttributeType[] faceAttributes = new FaceAttributeType[] { FaceAttributeType.Gender, FaceAttributeType.Age, FaceAttributeType.Smile, FaceAttributeType.Emotion, FaceAttributeType.Glasses, FaceAttributeType.Hair };
        #endregion // Member Variables

        #region Public Properties
        /// <summary>
        /// Gets or sets a value that indicates if faces should be detected in the image.
        /// </summary>
        public bool DetectFaces { get { return detectFaces; } set { detectFaces = value; } }
        
        /// <summary>
        /// Gets or sets the list of face attributes to detect in the image.
        /// </summary>
        public FaceAttributeType[] FaceAttributes { get { return faceAttributes; } set { faceAttributes = value; } }
        #endregion // Public Properties
    }
}
