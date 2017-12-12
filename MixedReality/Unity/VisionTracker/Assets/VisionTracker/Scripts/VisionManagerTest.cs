using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.WSA.Input;
using Microsoft.UnitySamples.Vision;
using Microsoft.ProjectOxford.Face.Contract;
using ProtoTurtle.BitmapDrawing;

public class VisionManagerTest : MonoBehaviour
{
    #region Member Variables
    private GestureRecognizer gestureRecognizer;
    private VisionRecognitionOptions recoOptions;
   
    #endregion // Member Variables

    #region Unity Inspector Fields
    [SerializeField]
    private Shader shader;

    [SerializeField]
    private VisionManager visionManager;

    [SerializeField]
    private GameObject headMarker;
    #endregion // Unity Inspector Fields

    private void Initialize()
    {
        Debug.Log("Initializing...");

        // Setup Gestures
        gestureRecognizer = new GestureRecognizer();
        gestureRecognizer.SetRecognizableGestures(GestureSettings.Tap);
        gestureRecognizer.Tapped += GestureRecognizer_Tapped;
        gestureRecognizer.StartCapturingGestures();

        // What are we going to recognize?
        recoOptions = new VisionRecognitionOptions();
        recoOptions.DetectFaces = true;
    }

    private void VisualizeResult2D(VisionCaptureResult result)
    {
        // Create a canvas to show the result
        GameObject canvas = GameObject.CreatePrimitive(PrimitiveType.Quad);
        canvas.name = "PhotoCaptureCanvas";


        Renderer renderer = canvas.GetComponent<Renderer>() as Renderer;
        //could use Shader.Find("AR/HolographicImageBlend") but using hard reference to m_shader
        //so the shader isn't stripped out as there's nothing directly in the scene using it
        renderer.material = new Material(shader);
        
        // Get the Matrix
        Matrix4x4 cameraToWorldMatrix;
        result.PhotoFrame.TryGetCameraToWorldMatrix(out cameraToWorldMatrix);
        Matrix4x4 worldToCameraMatrix = cameraToWorldMatrix.inverse;

        Matrix4x4 projectionMatrix;
        //result.PhotoFrame.TryGetProjectionMatrix(out projectionMatrix);
        //took out Camera.main.nearClipPlane, Camera.main.farClipPlane,
        result.PhotoFrame.TryGetProjectionMatrix( out projectionMatrix);
        Matrix4x4 pixelToCameraMatrix = projectionMatrix.inverse;


        var cameraRotation = Quaternion.LookRotation(-cameraToWorldMatrix.GetColumn(2), cameraToWorldMatrix.GetColumn(1));

        // Draw face rectangles into the texture
        bool drewOne = false;
        foreach (RecognitionResult reco in result.Recognitions)
        {
            // Draw the location into the photo
            result.PhotoTexture.DrawRectangle(reco.Location, Color.red);
            drewOne = true;

            // See if we have a face
            FaceRecognitionResult faceReco = reco as FaceRecognitionResult;
            if (faceReco != null)
            {
                // TODO: Draw something about the face
                //get middle pixel of face result
          
                Debug.Log($"reco.Location:{reco.Location}");
                Vector2 middlePixel = new Vector2(reco.Location.left +  ( reco.Location.width / 2), reco.Location.top + ( reco.Location.height / 2));
                Debug.Log($"middlePixel:{middlePixel}");

                Vector2 ImagePosZeroToOne = new Vector2(middlePixel.x / result.PhotoTexture.width, 1.0f - (middlePixel.y / result.PhotoTexture.height));
                Debug.Log($"ImagePosZeroToOne:{ImagePosZeroToOne}");

                Vector2 ImagePosProjected = ((ImagePosZeroToOne * 2.0f) - Vector2.one); // -1 to 1 space
                Debug.Log($"ImagePosProjected:{ImagePosProjected}");

                //was projectionMatrix
                //get a 2d point and unproject it to a 3d space. 
                //Vector3 CameraSpacePos = UnProjectVector(projectionMatrix, new Vector3(ImagePosProjected.x, ImagePosProjected.y, 1));
                Vector3 CameraSpacePos = UnProjectVector(projectionMatrix, new Vector3(middlePixel.x, middlePixel.y, 1));
                Debug.Log($"CameraSpacePos:{CameraSpacePos}");

                //camera location - works.
                Vector3 WorldSpaceRayPoint1 = cameraToWorldMatrix * new Vector4(0, 0, 0, 1);// mul(cameraToWorldMatrix, float4(0, 0, 0, 1)); // camera location in world space
                Debug.Log($"WorldSpaceRayPoint1:{WorldSpaceRayPoint1}");
                var cube1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube1.transform.localScale = new Vector3(.1f, .1f, .1f);
                cube1.transform.position = WorldSpaceRayPoint1;
                //cube.GetComponent<MeshRenderer>().material.SetColor()

                //The pixel in world space
                Vector3 WorldSpaceRayPoint2 = cameraToWorldMatrix.MultiplyVector(CameraSpacePos); //mul(cameraToWorldMatrix, CameraSpacePos); // ray point in world space
                var cube2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube2.transform.localScale = new Vector3(.1f, .1f, .1f);
                cube2.transform.position = WorldSpaceRayPoint2;
                Debug.Log($"WorldSpaceRayPoint2:{WorldSpaceRayPoint2}");


                var cameraResolution = result.CameraResolution;
                float top = -(reco.Location.top / cameraResolution.height - .5f);
                float left = reco.Location.left / cameraResolution.width - .5f;
                float width = reco.Location.width / cameraResolution.width;
                float height = reco.Location.height / cameraResolution.height;

                GameObject faceBounds = (GameObject)Instantiate(headMarker);
                faceBounds.transform.position = cameraToWorldMatrix.MultiplyPoint3x4(pixelToCameraMatrix.MultiplyPoint3x4(new Vector3(left + width / 2, top, 0)));
                faceBounds.transform.rotation = cameraRotation;
                Vector3 scale = pixelToCameraMatrix.MultiplyPoint3x4(new Vector3(width, height, 0));
                scale.z = .1f;
                faceBounds.transform.localScale = scale;
                //faceBounds.tag = "faceBounds";
                Debug.Log(string.Format("{0},{1} translates to {2},{3}", left, top, faceBounds.transform.position.x, faceBounds.transform.position.y));

                var lineRenderer = GameObject.FindWithTag("LineRenderer").GetComponent<LineRenderer>();
                lineRenderer.SetPosition(0, cube1.transform.position);
                lineRenderer.SetPosition(1, faceBounds.transform.position);

                Vector3 origin = cameraToWorldMatrix.MultiplyPoint3x4(pixelToCameraMatrix.MultiplyPoint3x4(new Vector3(left + width + .1f, top, 0)));
                //Text or canvas position
                //txtObject.transform.position = origin;
                //txtObject.transform.rotation = cameraRotation;
                //txtObject.tag = "faceText";
                //if (j.list.Count > 1)
                //{
                //    txtObject.transform.localScale /= 2;
                //}


            }
        }
        
        // If at least one was drawn, update the texture
        if (drewOne)
        {
            result.PhotoTexture.Apply();
        }

        // Assign the texture to the material
        renderer.sharedMaterial.SetTexture("_MainTex", result.PhotoTexture);
        renderer.sharedMaterial.SetMatrix("_WorldToCameraMatrix", worldToCameraMatrix);
        renderer.sharedMaterial.SetMatrix("_CameraProjectionMatrix", projectionMatrix);
        // renderer.sharedMaterial.SetFloat("_VignetteScale", 1.0f);

        // Position the canvas object slightly in front
        // of the real world web camera.
        Vector3 position = cameraToWorldMatrix.GetColumn(3) - cameraToWorldMatrix.GetColumn(2);

        // Rotate the canvas object so that it faces the user.
        Quaternion rotation = Quaternion.LookRotation(-cameraToWorldMatrix.GetColumn(2), cameraToWorldMatrix.GetColumn(1));

        canvas.transform.position = position;
        canvas.transform.rotation = rotation;
        canvas.SetActive(false);
        Debug.Log("Took picture!");
    }

    private async void GestureRecognizer_Tapped(TappedEventArgs args)
    {
        // Take a photo
        VisionCaptureResult result = await visionManager.CaptureAndRecognizeAsync(recoOptions);

        // Visualize the result
        VisualizeResult2D(result);
    }

    #region Unity Behavior Overrides
    private void Start()
    {
        Initialize();
    }
    #endregion // Unity Behavior Overrides

    public static Vector3 UnProjectVector(Matrix4x4 proj, Vector3 to)
    {
        Vector3 from = new Vector3(0, 0, 0);
        var axsX = proj.GetRow(0);
        var axsY = proj.GetRow(1);
        var axsZ = proj.GetRow(2);
        from.z = to.z / axsZ.z;
        from.y = (to.y - (from.z * axsY.z)) / axsY.y;
        from.x = (to.x - (from.z * axsX.z)) / axsX.x;
        return from;
    }

}
