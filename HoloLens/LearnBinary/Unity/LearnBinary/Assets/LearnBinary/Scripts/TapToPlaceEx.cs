// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity.InputModule;
using System;
using UnityEngine;
using UnityEngine.VR.WSA.Input;

namespace HoloToolkit.Unity.SpatialMapping
{
    /// <summary>
    /// The TapToPlace class is a basic way to enable users to move objects 
    /// and place them on real world surfaces.
    /// Put this script on the object you want to be able to move. 
    /// Users will be able to tap objects, gaze elsewhere, and perform the
    /// tap gesture again to place.
    /// This script is used in conjunction with GazeManager, GestureManager,
    /// and SpatialMappingManager.
    /// TapToPlace also adds a WorldAnchor component to enable persistence.
    /// </summary>

    public class TapToPlaceEx : MonoBehaviour, IInputClickHandler
    {
		#region Member Variables
		/// <summary>
		/// Manages persisted anchors.
		/// </summary>
		private WorldAnchorManager anchorManager;

		/// <summary>
		/// Manages global input.
		/// </summary>
		private InputManager inputManager;

		/// <summary>
		/// Controls spatial mapping.  In this script we access spatialMappingManager
		/// to control rendering and to access the physics layer mask.
		/// </summary>
		private SpatialMappingManager spatialMappingManager;

		/// <summary>
		/// Keeps track of if the user is moving the object or not.
		/// </summary>
		private bool placing;
		#endregion // Member Variables

		#region Inspector Variables
		[Tooltip("Additional layers where holograms may be placed.")]
		public LayerMask AdditionalPlacementLayers = 0;

		[Tooltip("Whether or not the object should be rotated to face the user while placing.")]
		public bool FaceWhilePlacing = true;

		[Tooltip("Whether or not the behavior starts in placement mode.")]
		public bool PlacingOnStart = false;

		[Tooltip("Supply a friendly name for the anchor as the key name for the WorldAnchorStore.")]
		public string SavedAnchorFriendlyName = string.Empty;

		[Tooltip("Whether or not to show the surface mesh while in placement mode.")]
		public bool ShowMeshWhilePlacing = false;

		[Tooltip("Whether or not to create and use a world anchor for the placement.")]
		public bool UseWorldAnchor = false;
		#endregion // Inspector Variables


		private void TryAttachAnchor()
		{
			if (string.IsNullOrEmpty(SavedAnchorFriendlyName))
			{
				Debug.LogError("UseWorldAnchor is true but SavedAnchorFriendlyName is not valid.");
			}
			else
			{
				if (anchorManager != null)
				{
					anchorManager.AttachAnchor(this.gameObject, SavedAnchorFriendlyName);
				}
			}
		}

		private void Start()
        {
			// Make sure we have all the components in the scene we need.
			anchorManager = WorldAnchorManager.Instance;
            if ((UseWorldAnchor) && (anchorManager == null))
            {
                Debug.LogError("This script requires that you have a WorldAnchorManager component in your scene in order to use world anchors.");
            }

			inputManager = InputManager.Instance;
			if (inputManager == null)
			{
				Debug.LogError("This script expects that you have an InputManager component in your scene.");
			}

			spatialMappingManager = SpatialMappingManager.Instance;
            if (spatialMappingManager == null)
            {
                Debug.LogError("This script expects that you have a SpatialMappingManager component in your scene.");
            }

            if (spatialMappingManager != null)
            {
				if ((UseWorldAnchor) && (anchorManager != null))
				{
					TryAttachAnchor();
				}

				// Start placing?
				if (PlacingOnStart)
				{
					StartPlacing();
				}
			}
            else
            {
                // If we don't have what we need to proceed, we may as well remove ourselves.
                Destroy(this);
            }
        }

        private void Update()
        {
            // If the user is in placing mode,
            // update the placement to match the user's gaze.
            if (placing)
            {
                // Do a raycast into the world that will only hit the Spatial Mapping mesh.
                var headPosition = Camera.main.transform.position;
                var gazeDirection = Camera.main.transform.forward;

                RaycastHit hitInfo;
                if (Physics.Raycast(headPosition, gazeDirection, out hitInfo,
                    30.0f, (spatialMappingManager.LayerMask | AdditionalPlacementLayers.value)))
                {
                    // Move this object to where the raycast
                    // hit the Spatial Mapping mesh.
                    // Here is where you might consider adding intelligence
                    // to how the object is placed.  For example, consider
                    // placing based on the bottom of the object's
                    // collider so it sits properly on surfaces.
                    this.transform.position = hitInfo.point;

					// Rotate this object to face the user?
					if (FaceWhilePlacing)
					{
						// Get camera location
						Vector3 target = Camera.main.transform.position;

						// Calculate target position but limit to y rotation
						Vector3 targetPostition = new Vector3(target.x, this.transform.position.y, target.z);

						// Rotate
						this.transform.LookAt(targetPostition);
					}
                }
            }
        }

        public void OnInputClicked(InputClickedEventData eventData)
        {
            // On each tap gesture, toggle whether the user is in placing mode.
            if (placing)
            {
				StopPlacing();
            }
            else
            {
				StartPlacing();
            }
        }

		/// <summary>
		/// Starts placing the object.
		/// </summary>
		public void StartPlacing()
		{
			// If already placing, ignore
			if (placing) { return; }

			// Set placing flag
			placing = true;

			// Show meshes
			if (ShowMeshWhilePlacing)
			{
				spatialMappingManager.DrawVisualMeshes = true;
			}

			// Remove world anchor
			if (UseWorldAnchor)
			{
				Debug.Log(gameObject.name + " : Removing existing world anchor if any.");
				if (anchorManager != null)
				{
					anchorManager.RemoveAnchor(gameObject);
				}
			}

			// Push ourselves as a fallback listener so the gaze doesn't 
			// have to be directly on us to finish placement
			inputManager.PushFallbackInputHandler(this.gameObject);

			// Notify
			if (PlacingStarted != null)
			{
				PlacingStarted(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Stops placing the object.
		/// </summary>
		public void StopPlacing()
		{
			// If not placing, ignore
			if (!placing) { return; }

			// Pop ourselves as a global listener so we don't get
			// further events unless we are the focus of the gaze.
			inputManager.PopFallbackInputHandler();

			// Turn off visual meshes
			if (ShowMeshWhilePlacing)
			{
				spatialMappingManager.DrawVisualMeshes = false;
			}

			// Add world anchor when object placement is done.
			if (UseWorldAnchor)
			{
				TryAttachAnchor();
			}

			// Clear placing flag
			placing = false;

			// Notify
			if (PlacingCompleted != null)
			{
				PlacingCompleted(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets a value that indicates if the object is being placed.
		/// </summary>
		public bool IsPlacing
		{
			get
			{
				return placing;
			}
		}

		/// <summary>
		/// Occurs when placing has started.
		/// </summary>
		public event EventHandler PlacingStarted;

		/// <summary>
		/// Occurs when placing has completed.
		/// </summary>
		public event EventHandler PlacingCompleted;
	}
}
