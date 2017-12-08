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
    private const double HEAD_SIZE = 0.25;          // meters
    private VisionRecognitionOptions recoOptions;
    #endregion // Member Variables

    #region Unity Inspector Fields
    [SerializeField]
    private Shader photoShader;
    [SerializeField]
    private Shader cubeShader;

    [SerializeField]
    private VisionManager visionManager;
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
        renderer.material = new Material(photoShader);

        // Get the Matrix
        Matrix4x4 cameraToWorldMatrix;
        result.PhotoFrame.TryGetCameraToWorldMatrix(out cameraToWorldMatrix);
        Matrix4x4 worldToCameraMatrix = cameraToWorldMatrix.inverse;

        Matrix4x4 projectionMatrix;
        result.PhotoFrame.TryGetProjectionMatrix(out projectionMatrix);

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
    }

    private void VisualizeResult3D(VisionCaptureResult result)
    {
        // Create a cube to mark the object
        GameObject containCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        containCube.name = "ContainObjectCube";

        Renderer renderer = containCube.GetComponent<Renderer>() as Renderer;
        renderer.material = new Material(cubeShader);

        // Get the Matrix
        Matrix4x4 cameraToWorldMatrix;
        result.PhotoFrame.TryGetCameraToWorldMatrix(out cameraToWorldMatrix);
        Matrix4x4 worldToCameraMatrix = cameraToWorldMatrix.inverse;

        Matrix4x4 projectionMatrix;
        result.PhotoFrame.TryGetProjectionMatrix(out projectionMatrix);
        
        // TODO Position the cube where the face currently is + distance away
        Vector3 position = cameraToWorldMatrix.GetColumn(3) - cameraToWorldMatrix.GetColumn(2);
        containCube.transform.position = position;
        containCube.transform.localScale = new Vector3(LocatedBounds.HEAD_SIZE, LocatedBounds.HEAD_SIZE, LocatedBounds.HEAD_SIZE);

        // Rotate the canvas object so that it properly contains the object it's containing
        //Quaternion rotation = Quaternion.LookRotation(-cameraToWorldMatrix.GetColumn(2), cameraToWorldMatrix.GetColumn(1));
        //containCube.transform.rotation = rotation;
    }

    private async void GestureRecognizer_Tapped(TappedEventArgs args)
    {
        // Take a photo
        VisionCaptureResult result = await visionManager.CaptureAndRecognizeAsync(recoOptions);

        // Visualize the result
        VisualizeResult2D(result);
        VisualizeResult3D(result);
    }



    #region Unity Behavior Overrides
    private void Start()
    {
        Initialize();
    }
    #endregion // Unity Behavior Overrides
}
