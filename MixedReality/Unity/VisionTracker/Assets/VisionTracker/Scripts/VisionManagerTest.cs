using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.WSA.Input;
using Microsoft.UnitySamples.Vision;

public class VisionManagerTest : MonoBehaviour
{
    #region Member Variables
    private GestureRecognizer gestureRecognizer;
    #endregion // Member Variables

    #region Unity Inspector Fields
    [SerializeField]
    private Shader shader;

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
        result.PhotoFrame.TryGetProjectionMatrix(out projectionMatrix);

        // Create a texture to hold the image data
        Texture2D texture = new Texture2D(result.PhotoWidth, result.PhotoHeight, TextureFormat.BGRA32, false);

        // Have the frame fill in the texture
        result.PhotoFrame.UploadImageDataToTexture(texture);
        texture.wrapMode = TextureWrapMode.Clamp;

        renderer.sharedMaterial.SetTexture("_MainTex", texture);
        renderer.sharedMaterial.SetMatrix("_WorldToCameraMatrix", worldToCameraMatrix);
        renderer.sharedMaterial.SetMatrix("_CameraProjectionMatrix", projectionMatrix);
        renderer.sharedMaterial.SetFloat("_VignetteScale", 1.0f);

        // Position the canvas object slightly in front
        // of the real world web camera.
        Vector3 position = cameraToWorldMatrix.GetColumn(3) - cameraToWorldMatrix.GetColumn(2);

        // Rotate the canvas object so that it faces the user.
        Quaternion rotation = Quaternion.LookRotation(-cameraToWorldMatrix.GetColumn(2), cameraToWorldMatrix.GetColumn(1));

        canvas.transform.position = position;
        canvas.transform.rotation = rotation;

        Debug.Log("Took picture!");
    }


    private async void GestureRecognizer_Tapped(TappedEventArgs args)
    {
        // Take a photo
        VisionCaptureResult result = await visionManager.CaptureAndRecognizeAsync();

        // Visualize the result
        VisualizeResult2D(result);
    }



    #region Unity Behavior Overrides
    private void Start()
    {
        Initialize();
    }
    #endregion // Unity Behavior Overrides
}
