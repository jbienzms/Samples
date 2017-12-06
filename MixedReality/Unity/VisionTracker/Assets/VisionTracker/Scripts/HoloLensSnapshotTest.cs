using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class HoloLensSnapshotTest : MonoBehaviour
{
    private UnityEngine.XR.WSA.Input.GestureRecognizer m_GestureRecognizer;
    private GameObject m_Canvas = null;
    private Renderer m_CanvasRenderer = null;
    private UnityEngine.XR.WSA.WebCam.PhotoCapture m_PhotoCaptureObj;
    private UnityEngine.XR.WSA.WebCam.CameraParameters m_CameraParameters;
    private bool m_CapturingPhoto = false;
    private Texture2D m_Texture = null;

    /// <summary>
    /// Have a direct reference to the shader so it isn't stripped.
    /// </summary>
    [SerializeField]
    private Shader m_Shader;

    void Start()
    {
        Initialize();
    }

    void SetupGestureRecognizer()
    {
        m_GestureRecognizer = new UnityEngine.XR.WSA.Input.GestureRecognizer();
        m_GestureRecognizer.SetRecognizableGestures(UnityEngine.XR.WSA.Input.GestureSettings.Tap);
        m_GestureRecognizer.TappedEvent += OnTappedEvent;
        m_GestureRecognizer.StartCapturingGestures();

        m_CapturingPhoto = false;
    }

    void Initialize()
    {
        Debug.Log("Initializing...");
        List<Resolution> resolutions = new List<Resolution>(UnityEngine.XR.WSA.WebCam.PhotoCapture.SupportedResolutions);
        Resolution selectedResolution = resolutions[0];

        m_CameraParameters = new UnityEngine.XR.WSA.WebCam.CameraParameters(UnityEngine.XR.WSA.WebCam.WebCamMode.PhotoMode);
        m_CameraParameters.cameraResolutionWidth = selectedResolution.width;
        m_CameraParameters.cameraResolutionHeight = selectedResolution.height;
        m_CameraParameters.hologramOpacity = 0.0f;
        m_CameraParameters.pixelFormat = UnityEngine.XR.WSA.WebCam.CapturePixelFormat.BGRA32;

        m_Texture = new Texture2D(selectedResolution.width, selectedResolution.height, TextureFormat.BGRA32, false);

        UnityEngine.XR.WSA.WebCam.PhotoCapture.CreateAsync(false, OnCreatedPhotoCaptureObject);
    }

    void OnCreatedPhotoCaptureObject(UnityEngine.XR.WSA.WebCam.PhotoCapture captureObject)
    {
        m_PhotoCaptureObj = captureObject;
        m_PhotoCaptureObj.StartPhotoModeAsync(m_CameraParameters, OnStartPhotoMode);
    }

    void OnStartPhotoMode(UnityEngine.XR.WSA.WebCam.PhotoCapture.PhotoCaptureResult result)
    {
        SetupGestureRecognizer();

        Debug.Log("Ready!");
        Debug.Log("Air Tap to take a picture.");
    }

    void OnTappedEvent(UnityEngine.XR.WSA.Input.InteractionSourceKind source, int tapCount, Ray headRay)
    {
        if (m_CapturingPhoto)
        {
            return;
        }

        m_CapturingPhoto = true;
        Debug.Log("Taking picture...");
        m_PhotoCaptureObj.TakePhotoAsync(OnPhotoCaptured);
    }

    void OnPhotoCaptured(UnityEngine.XR.WSA.WebCam.PhotoCapture.PhotoCaptureResult result, UnityEngine.XR.WSA.WebCam.PhotoCaptureFrame photoCaptureFrame)
    {
        if (m_Canvas == null)
        {
            m_Canvas = GameObject.CreatePrimitive(PrimitiveType.Quad);
            m_Canvas.name = "PhotoCaptureCanvas";
            m_CanvasRenderer = m_Canvas.GetComponent<Renderer>() as Renderer;
            //could use Shader.Find("AR/HolographicImageBlend") but using hard reference to m_shader
            //so the shader isn't stripped out as there's nothing directly in the scene using it
            m_CanvasRenderer.material = new Material(m_Shader);
        }

        Matrix4x4 cameraToWorldMatrix;
        photoCaptureFrame.TryGetCameraToWorldMatrix(out cameraToWorldMatrix);
        Matrix4x4 worldToCameraMatrix = cameraToWorldMatrix.inverse;

        Matrix4x4 projectionMatrix;
        photoCaptureFrame.TryGetProjectionMatrix(out projectionMatrix);

        photoCaptureFrame.UploadImageDataToTexture(m_Texture);
        m_Texture.wrapMode = TextureWrapMode.Clamp;

        m_CanvasRenderer.sharedMaterial.SetTexture("_MainTex", m_Texture);
        m_CanvasRenderer.sharedMaterial.SetMatrix("_WorldToCameraMatrix", worldToCameraMatrix);
        m_CanvasRenderer.sharedMaterial.SetMatrix("_CameraProjectionMatrix", projectionMatrix);
        m_CanvasRenderer.sharedMaterial.SetFloat("_VignetteScale", 1.0f);

        // Position the canvas object slightly in front
        // of the real world web camera.
        Vector3 position = cameraToWorldMatrix.GetColumn(3) - cameraToWorldMatrix.GetColumn(2);

        // Rotate the canvas object so that it faces the user.
        Quaternion rotation = Quaternion.LookRotation(-cameraToWorldMatrix.GetColumn(2), cameraToWorldMatrix.GetColumn(1));

        m_Canvas.transform.position = position;
        m_Canvas.transform.rotation = rotation;

        Debug.Log("Took picture!");
        m_CapturingPhoto = false;
    }
}
