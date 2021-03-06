using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using Microsoft.MixedReality.Toolkit.LightingTools;

public class MainController : MonoBehaviour
{
    #region Constants
    private const string ARTICLE_URL = "http://www.roadtomr.com/2020/06/16/2835/envirolight-hl2/";
    private const string ARTIST_URL = "https://www.digitalangel3d.com";
    private const string DEVELOPER_URL = "https://jared.bienz.com/";
    #endregion // Constants

    #region Member Variables
    private bool isSwitchingLightModes;
    private List<GameObject> screens = new List<GameObject>();
    #endregion // Member Variables

    #region Inspector Fields
    [Tooltip("Toggle for enabling default lights.")]
    public Toggle DefaultLightsToggle;

    [Tooltip("Toggle for enabling environment lights.")]
    public Toggle EnvironmentLightsToggle;

    [Tooltip("Used for placing the whole scene.")]
    public LightCapture LightCapture;

    [Tooltip("Used for placing the whole scene.")]
    public PlacementController ScenePlacementController;

    [Tooltip("The GameObject that represents the whole scene.")]
    public GameObject SceneRoot;

    [Tooltip("Used for speaking notifications.")]
    public TextToSpeech TextToSpeech;

    [Tooltip("The About UI Panel.")]
    public GameObject UIAbout;

    [Tooltip("The Home UI Panel.")]
    public GameObject UIHome;

    [Tooltip("The Light Sources UI Panel.")]
    public GameObject UILights;

    [Tooltip("The Placement UI Panel.")]
    public GameObject UIPlace;
    #endregion

    #region Internal Methods
    /// <summary>
    /// Navigates to the specified UI screen.
    /// </summary>
    /// <param name="screen">
    /// The screen to navigate to.
    /// </param>
    private void GoToScreen(GameObject screen)
    {
        // Disable all but current
        foreach (GameObject s in screens)
        {
            // Activate or deactivate
            s.SetActive(s == screen);
        }
    }

    /// <summary>
    /// Opens the specified URL in the system browser.
    /// </summary>
    /// <param name="url">
    /// The URL to open.
    /// </param>
    private void OpenUrl(string url)
    {
        Debug.Log($"OpenUrl: Opening {url}");
        TextToSpeech.StartSpeaking("Opening browser...");

        #if WINDOWS_UWP
        UnityEngine.WSA.Application.InvokeOnUIThread(async () =>
        {
            bool result = await global::Windows.System.Launcher.LaunchUriAsync(new System.Uri(url));
            if (!result)
            {
                Debug.LogError("Launching URI failed to launch.");
            }
        }, false);
        #else
        Application.OpenURL(url);
        #endif
    }

    /// <summary>
    /// Switches light modes.
    /// </summary>
    /// <param name="environment">
    /// Should we should to default or environment?
    /// </param>
    /// <param name="speak">
    /// Should we speak the notification to the user.
    /// </param>
    private void UseLights(bool environment, bool speak=true)
    {
        // Avoid reentrance
        if (isSwitchingLightModes) { return; }

        // Confirm verbally
        if (speak)
        {
            TextToSpeech.StartSpeaking((environment ? "Environment lights" : "Default lights"));
        }

        // Make sure changing
        if (LightCapture.enabled == environment) { return; }
        isSwitchingLightModes = true;

        // Enable / Disable lighting containers
        // The order is important due to the way
        // Unity decides how normals are baked at runtime
        if (environment)
        {
            LightCapture.enabled = true;
            DefaultLightsToggle.isOn = false;
            EnvironmentLightsToggle.isOn = true;
        }
        else
        {
            LightCapture.enabled = false;
            EnvironmentLightsToggle.isOn = false;
            DefaultLightsToggle.isOn = true;
        }

        // Done
        isSwitchingLightModes = false;
    }
    #endregion // Internal Methods

    #region Overrides / Event Handlers
    private void OnLightsToggle(Toggle sender)
    {
        if (!isSwitchingLightModes)
        {
            UseLights(sender == EnvironmentLightsToggle);
        }
    }

    private void OnPlacingStopped()
    {
        // Go to home screen
        GoToScreen(UIHome);
    }
    #endregion // Overrides / Event Handlers

    #region Behavior Overrides
    void Start()
    {
        // Store UI screens
        screens.Add(UIAbout);
        screens.Add(UIHome);
        screens.Add(UILights);
        screens.Add(UIPlace);

        // Subscribe event handlers
        DefaultLightsToggle.onValueChanged.AddListener((b) => { if (b) OnLightsToggle(DefaultLightsToggle); });
        EnvironmentLightsToggle.onValueChanged.AddListener((b) => { if (b) OnLightsToggle(EnvironmentLightsToggle); });
        ScenePlacementController.TapToPlace.OnPlacingStopped.AddListener(OnPlacingStopped);

        // Hide the scene on start
        SceneRoot.SetActive(false);

        // Start placing
        ScenePlacementController.StartPlacement();
    }
    #endregion // Behavior Overrides

    #region Public Methods
    /// <summary>
    /// Resets the environment scan.
    /// </summary>
    public void ResetEnvironmentLights()
    {
        LightCapture.Clear();
    }

    /// <summary>
    /// Starts placement of the scene.
    /// </summary>
    public void StartPlacing()
    {
        GoToScreen(UIPlace);
        ScenePlacementController.StartPlacement();
    }

    /// <summary>
    /// Navigates to the About UI screen.
    /// </summary>
    public void UIGoAbout()
    {
        GoToScreen(UIAbout);
    }

    /// <summary>
    /// Navigates to the Home UI screen.
    /// </summary>
    public void UIGoHome()
    {
        GoToScreen(UIHome);
    }

    /// <summary>
    /// Navigates to the Lights UI screen.
    /// </summary>
    public void UIGoLights()
    {
        GoToScreen(UILights);
    }

    /// <summary>
    /// Uses default lighting.
    /// </summary>
    public void UseDefaultLights()
    {
        // Switch mode
        UseLights(false);
    }

    /// <summary>
    /// Uses environment lighting.
    /// </summary>
    public void UseEnvironmentLights()
    {
        // Switch mode
        UseLights(true);
    }

    /// <summary>
    /// Visit the article for the sample.
    /// </summary>
    public void VisitArticle()
    {
        OpenUrl(ARTICLE_URL);
    }

    /// <summary>
    /// Visit the artists home page.
    /// </summary>
    public void VisitArtist()
    {
        OpenUrl(ARTIST_URL);
    }

    /// <summary>
    /// Visit the developers home page.
    /// </summary>
    public void VisitDeveloper()
    {
        OpenUrl(DEVELOPER_URL);
    }
    #endregion // Public Methods

    #region Public Properties
    /// <summary>
    /// Gets a value that indicates if environment lights are being used.
    /// </summary>
    /// <value>
    /// <c>true</c> if environment lights are being used; otherwise <c>false</c>.
    /// </value>
    public bool IsUsingEnvironment
    {
        get
        {
            return LightCapture.enabled;
        }
    }
    #endregion // Public Properties
}
