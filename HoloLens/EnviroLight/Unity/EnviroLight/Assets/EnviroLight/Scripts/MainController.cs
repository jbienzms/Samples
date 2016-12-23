using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using HoloToolkit.Unity.SpatialMapping;

public class MainController : MonoBehaviour
{
	#region Constants
	private const string ARTICLE_URL = "http://www.wikiholo.net";
	private const string ARTIST_URL = "http://www.pixeldevil3d.com";
	private const string DEVELOPER_URL = "http://www.roadtoholo.com";
	#endregion // Constants

	#region Member Variables
	private GazeManager gazeManager;
	private bool isSwitchingLightModes;
	private bool isUsingEnvironment;
	private List<GameObject> screens = new List<GameObject>();
	#endregion // Member Variables

	#region Inspector Fields
	[Tooltip("The container for all default scene lights.")]
	public GameObject DefaultLightsContainer;

	[Tooltip("Toggle for enabling default lights.")]
	public Toggle DefaultLightsToggle;

	[Tooltip("The container where environment lights will be added.")]
	public GameObject EnvironmentLightsContainer;

	[Tooltip("Toggle for enabling environment lights.")]
	public Toggle EnvironmentLightsToggle;

	[Tooltip("Used for speaking notifications.")]
	public TapToPlaceEx TapToPlace;

	[Tooltip("Used for speaking notifications.")]
	public TextToSpeechManager TextToSpeechManager;

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
		Application.OpenURL(url);
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
			TextToSpeechManager.SpeakText((environment ? "Environment lights" : "Default lights"));
		}
		
		// Make sure changing
		if (isUsingEnvironment == environment) { return; }
		isSwitchingLightModes = true;

		// Update state flag
		isUsingEnvironment = environment;

		// Enable / Disable lighting containers
		// The order is important due to the way
		// Unity decides how normals are baked at runtime
		if (environment)
		{
			DefaultLightsContainer.SetActive(false);
			DefaultLightsToggle.isOn = false;
			EnvironmentLightsContainer.SetActive(true);
			EnvironmentLightsToggle.isOn = true;
		}
		else
		{
			EnvironmentLightsContainer.SetActive(false);
			EnvironmentLightsToggle.isOn = false;
			DefaultLightsContainer.SetActive(true);
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

	private void OnPlacingCompleted(object sender, EventArgs e)
	{
		// Disable TapToPlace so user can interact with PC and other items.
		TapToPlace.enabled = false;

		// Go to home screen
		GoToScreen(UIHome);
	}
	#endregion // Overrides / Event Handlers

	#region Behavior Overrides
	void Start()
	{
		// Shortcut to managers
		gazeManager = GazeManager.Instance;

		// Store UI screens
		screens.Add(UIAbout);
		screens.Add(UIHome);
		screens.Add(UILights);
		screens.Add(UIPlace);

		// Subscribe event handlers
		DefaultLightsToggle.onValueChanged.AddListener((b) => { if (b) OnLightsToggle(DefaultLightsToggle); });
		EnvironmentLightsToggle.onValueChanged.AddListener((b) => { if (b) OnLightsToggle(EnvironmentLightsToggle); });
		TapToPlace.PlacingCompleted += OnPlacingCompleted;

		// Start placing
		StartPlacing();
	}
	#endregion // Behavior Overrides

	#region Public Methods
	/// <summary>
	/// Adds a new environmental light to the scene.
	/// </summary>
	public void AddLight()
	{
		// Can only continue if the user is gazing at a surface
		if (!gazeManager.IsGazingAtObject) { return; }

		// Get gaze hit point
		var hit = gazeManager.HitInfo;

		// Create placeholder GO
		var lightGO = new GameObject("EnvironmentLight");

		// Parent the light GO to the container
		lightGO.transform.SetParent(EnvironmentLightsContainer.transform, worldPositionStays: false);

		// Move it to the right location
		lightGO.transform.position = hit.point;

		// Add point light
		var light = lightGO.AddComponent<Light>();

		// Configure light
		light.type = LightType.Point;
		// light.lightmappingMode = LightmappingMode.Realtime;
		light.range = 20;
		light.intensity = 1.85f;

		// Notify
		TextToSpeechManager.SpeakText("Light added");

		// Now that we have at least one environment light, 
		// switch to environment mode
		UseLights(environment: true, speak:false);
	}

	/// <summary>
	/// Starts placement of the scene.
	/// </summary>
	public void StartPlacing()
	{
		GoToScreen(UIPlace);
		TapToPlace.enabled = true;
		TapToPlace.StartPlacing();
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
			return isUsingEnvironment;
		}
	}
	#endregion // Public Properties
}
