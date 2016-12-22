using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour
{
	#region Member Variables
	private GazeManager gazeManager;
	private bool isUsingEnvironment = false;
	#endregion // Member Variables

	#region Inspector Fields
	[Tooltip("The container for all default scene lights.")]
	public GameObject DefaultLightsContainer;

	[Tooltip("The container where environment lights will be added.")]
	public GameObject EnvironmentLightsContainer;

	[Tooltip("Used for speaking notifications.")]
	public TextToSpeechManager TextToSpeechManager;
	#endregion

	#region Internal Methods
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
		// Confirm verbally
		if (speak)
		{
			TextToSpeechManager.SpeakText((environment ? "Environment lights" : "Default lights"));
		}
		
		// Make sure changing
		if (isUsingEnvironment == environment) { return; }

		// Update state flag
		isUsingEnvironment = environment;

		// Enable / Disable lighting containers
		// The order is important due to the way
		// Unity decides how normals are baked at runtime
		if (environment)
		{
			DefaultLightsContainer.SetActive(false);
			EnvironmentLightsContainer.SetActive(true);
		}
		else
		{
			EnvironmentLightsContainer.SetActive(false);
			DefaultLightsContainer.SetActive(true);
		}
	}
	#endregion // Internal Methods

	#region Behavior Overrides
	void Start()
	{
		// Shortcut to managers
		gazeManager = GazeManager.Instance;
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
		light.range = 10;
		light.intensity = 1.85f;

		// Notify
		TextToSpeechManager.SpeakText("Light added");

		// Now that we have at least one environment light, 
		// switch to environment mode
		UseLights(environment: true, speak:false);
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
