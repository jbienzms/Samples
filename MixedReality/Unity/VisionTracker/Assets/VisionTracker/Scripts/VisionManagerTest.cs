using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.WSA.Input;
using Microsoft.UnitySamples.Vision;
using Microsoft.ProjectOxford.Face.Contract;

public class VisionManagerTest : MonoBehaviour
{
    #region Member Variables
    private GestureRecognizer gestureRecognizer;
    private const double HEAD_SIZE = 0.25;          // meters
    #endregion // Member Variables

    #region Unity Inspector Fields
    [SerializeField]
    private Shader photoShader;
    [SerializeField]
    private Shader cubeShader;

    [SerializeField]
    private VisionManager visionManager;
    #endregion // Unity Inspector Fields

    private void DrawLine(Texture2D tex, int x0, int y0, int x1, int y1, Color col)
    {
        int dy = (int)(y1 - y0);
        int dx = (int)(x1 - x0);
        int stepx, stepy;

        if (dy < 0) { dy = -dy; stepy = -1; }
        else { stepy = 1; }
        if (dx < 0) { dx = -dx; stepx = -1; }
        else { stepx = 1; }
        dy <<= 1;
        dx <<= 1;

        float fraction = 0;

        tex.SetPixel(x0, y0, col);
        if (dx > dy)
        {
            fraction = dy - (dx >> 1);
            while (Mathf.Abs(x0 - x1) > 1)
            {
                if (fraction >= 0)
                {
                    y0 += stepy;
                    fraction -= dx;
                }
                x0 += stepx;
                fraction += dy;
                tex.SetPixel(x0, y0, col);
            }
        }
        else
        {
            fraction = dx - (dy >> 1);
            while (Mathf.Abs(y0 - y1) > 1)
            {
                if (fraction >= 0)
                {
                    x0 += stepx;
                    fraction -= dy;
                }
                y0 += stepy;
                fraction += dx;
                tex.SetPixel(x0, y0, col);
            }
        }
    }

    private void DrawRect(Texture2D tex, FaceRectangle rect, Color col)
    {
        int left = rect.Left;
        int top = rect.Top;
        int right = rect.Left + rect.Width;
        int bottom = rect.Top + rect.Height;

        DrawLine(tex, left, top, right, top, col); // Top
        DrawLine(tex, right, top, right, bottom, col); // Right
        DrawLine(tex, right, bottom, left, bottom, col); // Bottom
        DrawLine(tex, left, bottom, left, top, col); // Left
    }

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
        renderer.material = new Material(photoShader);

        // Get the Matrix
        Matrix4x4 cameraToWorldMatrix;
        result.PhotoFrame.TryGetCameraToWorldMatrix(out cameraToWorldMatrix);
        Matrix4x4 worldToCameraMatrix = cameraToWorldMatrix.inverse;

        Matrix4x4 projectionMatrix;
        result.PhotoFrame.TryGetProjectionMatrix(out projectionMatrix);

        // Draw face rectangles into the texture
        bool drewOne = false;
        foreach (Face face in result.Faces)
        {
            DrawRect(result.PhotoTexture, face.FaceRectangle, Color.red);
            drewOne = true;
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

        Debug.Log("Took picture!");
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

        int headSize = 50;//= LocatedBounds.HEAD_SIZE; // TODO convert to amount of pixels that cover the head

        // Create a texture to hold the image data
        Texture3D texture = new Texture3D(headSize, headSize, headSize, TextureFormat.BGRA32, false);

        // TODO see if this is actually needed
        texture.wrapMode = TextureWrapMode.Clamp;

        renderer.sharedMaterial.SetTexture("_MainTex", texture);
        renderer.sharedMaterial.SetMatrix("_WorldToCameraMatrix", worldToCameraMatrix);
        renderer.sharedMaterial.SetMatrix("_CameraProjectionMatrix", projectionMatrix);

        // TODO Position the cube where the face currently is + distance away
        Vector3 position = cameraToWorldMatrix.GetColumn(3) - cameraToWorldMatrix.GetColumn(2);
        containCube.transform.position = position;

        // Rotate the canvas object so that it properly contains the object it's containing
        //Quaternion rotation = Quaternion.LookRotation(-cameraToWorldMatrix.GetColumn(2), cameraToWorldMatrix.GetColumn(1));
        //containCube.transform.rotation = rotation;

        Debug.Log("Took picture!");
    }

    private async void GestureRecognizer_Tapped(TappedEventArgs args)
    {
        // Take a photo
        VisionCaptureResult result = await visionManager.CaptureAndRecognizeAsync();

        // Visualize the result
        VisualizeResult3D(result);
    }



    #region Unity Behavior Overrides
    private void Start()
    {
        Initialize();
    }
    #endregion // Unity Behavior Overrides
}
