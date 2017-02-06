using HoloToolkit.Unity;
using HoloToolkit.Unity.SpatialMapping;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LearningState
{
	Placing,
	Interacting,
}

public class LearningController : Singleton<LearningController>
{
	#region Member Variables
	private LearningState state;
	#endregion // Member Variables

	#region Inspector Fields
	[Tooltip("The bit manager the scene.")]
	public BitManager bitManager;

	[Tooltip("The cursor to be used with the Directional Indicator.")]
	public GameObject cursor;

	[Tooltip("The TapToPlace used to place the whole scene.")]
	public TapToPlaceEx scenePlacement;
	#endregion // Inspector Fields

	#region Behavior Overrides
	void Start()
	{
		// Validate components
		if (bitManager == null)
		{
			Debug.LogErrorFormat("The {0} inspector field is not set and is required. {1} did not load completely.", "bitManager", this.GetType().Name);
			return;
		}
		if (cursor == null)
		{
			Debug.LogErrorFormat("The {0} inspector field is not set and is required. {1} did not load completely.", "cursor", this.GetType().Name);
			return;
		}
		if (scenePlacement == null)
		{
			Debug.LogErrorFormat("The {0} inspector field is not set and is required. {1} did not load completely.", "scenePlacement", this.GetType().Name);
			return;
		}

		// Subscribe to events
		bitManager.TotalValueChanged += BitManager_TotalValueChanged;
		scenePlacement.PlacingCompleted += ScenePlacement_PlacingCompleted;
	}

	private void BitManager_TotalValueChanged(object sender, System.EventArgs e)
	{
		// Update the total block
	}

	// Update is called once per frame
	void Update()
	{

	}
	#endregion // Behavior Overrides

	#region Overrides / Event Handlers
	private void ScenePlacement_PlacingCompleted(object sender, System.EventArgs e)
	{
		// If we're in placing, transition to interacting
		if (state == LearningState.Placing)
		{
			state = LearningState.Interacting;
		}
	}
	#endregion // Overrides / Event Handlers

	#region Public Properties
	/// <summary>
	/// Gets the current state of the learning controller.
	/// </summary>
	public LearningState State
	{
		get
		{
			return state;
		}
	}
	#endregion // Public Properties
}
