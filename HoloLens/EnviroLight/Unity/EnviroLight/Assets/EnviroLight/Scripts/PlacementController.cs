using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementController : MonoBehaviour
{
    #region Unity Inspector Variables
    [SerializeField]
    [Tooltip("Add an additional offset to apply when placing is completed.")]
    private Vector3 additionalOffset;

    [SerializeField]
    [Tooltip("Add an additional rotation to apply when placing is completed.")]
    private Vector3 additionalRotation;

    [SerializeField]
    [Tooltip("If true, placing will begin automatically.")]
    private bool autoStart = false;

    [SerializeField]
    [Tooltip("The GameObject that will be placed.")]
    private GameObject objectToPlace = null;

    [SerializeField]
    [Tooltip("The GameObject that will be used as the placement indicator.")]
    private GameObject placementIndicator = null;

    [SerializeField]
    [Tooltip("The TapToPlace component that will be used to move the placement indicator.")]
    private TapToPlace tapToPlace = null;
    #endregion // Unity Inspector Variables

    #region Unity Overrides
    // Start is called before the first frame update
    protected virtual void Start()
    {
        if (autoStart)
        {
            StartPlacement();
        }
    }

    #endregion // Unity Overrides

    #region Overrides / Event Handlers
    /// <summary>
    /// Occurs when the TapToPlace has finished placing.
    /// </summary>
    protected virtual void OnPlacingStopped()
    {
        // Unsubscribe
        tapToPlace.AutoStart = false; // HACK: to deal with StartPlacement not working if Start hasn't completed
        tapToPlace.OnPlacingStopped.RemoveListener(OnPlacingStopped);

        // Disable the indicator
        placementIndicator.SetActive(false);

        // Temporarily disable the object to avoid physics updates
        objectToPlace.SetActive(false);

        // Move the object
        objectToPlace.transform.position = placementIndicator.transform.position + additionalOffset;

        // Rotate the object
        objectToPlace.transform.rotation = placementIndicator.transform.rotation * Quaternion.Euler(additionalRotation);

        // Re-enable the object
        objectToPlace.SetActive(true);
    }
    #endregion // Overrides / Event Handlers

    #region Public Methods
    /// <summary>
    /// Attempts to start placing.
    /// </summary>
    public void StartPlacement()
    {
        // Validate objects
        if (objectToPlace == null) { throw new InvalidOperationException($"{nameof(ObjectToPlace)} hasn't been supplied."); }
        if (placementIndicator == null) { throw new InvalidOperationException($"{nameof(PlacementIndicator)} hasn't been supplied."); }

        // Try to get a TapToPlace
        if (tapToPlace == null) { tapToPlace = placementIndicator.GetComponent<TapToPlace>(); }

        // If already being placed, ignore
        if (tapToPlace.IsBeingPlaced) { return; }

        // Validate TapToPlace
        if (placementIndicator == null) { throw new InvalidOperationException($"{nameof(PlacementIndicator)} doesn't have a {nameof(TapToPlace)} and none has been supplied."); }

        // Subscribe to events
        tapToPlace.OnPlacingStopped.AddListener(OnPlacingStopped);

        // Enable the indicator
        placementIndicator.SetActive(true);

        // Start placing
        tapToPlace.AutoStart = true; // HACK: to deal with StartPlacement not working if Start hasn't completed
        tapToPlace.StartPlacement();
    }
    #endregion // Public Methods

    #region Public Properties
    /// <summary>
    /// Add an additional offset to apply when placing is completed.
    /// </summary>
    public Vector3 AdditionalOffset
    {
        get => additionalOffset;
        set => additionalOffset = value;
    }

    /// <summary>
    /// Add an additional rotation to apply when placing is completed.
    /// </summary>
    public Vector3 AdditionalRotation
    {
        get => additionalRotation;
        set => additionalRotation = value;
    }

    /// <summary>
    /// If true, placing will begin automatically.
    /// </summary>
    public bool AutoStart
    {
        get => autoStart;
        set => autoStart = value;
    }

    /// <summary>
    /// Gets or sets the GameObject that will be placed.
    /// </summary>
    public GameObject ObjectToPlace
    {
        get => objectToPlace;
        set => objectToPlace = value;
    }

    /// <summary>
    /// Gets or sets the GameObject that will be used as the placement indicator.
    /// </summary>
    public GameObject PlacementIndicator
    {
        get => placementIndicator;
        set => placementIndicator = value;
    }

    /// <summary>
    /// If true, placing will begin automatically.
    /// </summary>
    public TapToPlace TapToPlace
    {
        get => tapToPlace;
        set => tapToPlace = value;
    }
    #endregion // Public Properties
}
