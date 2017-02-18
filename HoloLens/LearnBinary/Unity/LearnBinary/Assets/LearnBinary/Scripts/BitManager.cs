using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages an array of bit blocks.
/// </summary>
public class BitManager : MonoBehaviour
{
	#region Member Variables
	private int totalValue;
	#endregion // Member Variables

	#region Inspector Fields
	[Tooltip("The total bits used in the scene.")]
	public BitController[] bits;
	#endregion // Inspector Fields

	#region Behavior Overrides
	void Start()
	{
		// Validate
		if ((bits == null) || (bits.Length < 1))
		{
			Debug.LogErrorFormat("The {0} inspector field is not set and is required. {1} did not load completely.", "bits", this.GetType().Name);
			return;
		}

		// Subscribe to events
		foreach (BitController bit in bits)
		{
			bit.ValueChanged += Bit_ValueChanged;
		}
	}

	private void Bit_ValueChanged(object sender, EventArgs e)
	{
		// Figure out total value
		totalValue = 0;
		foreach (BitController bit in bits)
		{
			totalValue += (int)bit.Value;
		}

		// Notify
		if (TotalValueChanged != null) { TotalValueChanged(this, EventArgs.Empty); }
	}
    #endregion // Behavior Overrides

    #region Public Methods
    /// <summary>
    /// Resets all bits managed by the BitManager to zero.
    /// </summary>
    public void ResetAllBits()
    {
        foreach (BitController bit in bits)
        {
            bit.AnimateSwitch(false);
        }
    }
    #endregion // Public Methods

    #region Public Properties
    /// <summary>
    /// Gets the total value of all current bits.
    /// </summary>
    public int TotalValue
	{
		get
		{
			return totalValue;
		}
		private set
		{
			if (totalValue != value)
			{
				totalValue = value;
				if (TotalValueChanged != null) { TotalValueChanged(this, EventArgs.Empty); }
			}
		}
	}
	#endregion // Public Properties

	#region Public Events
	/// <summary>
	/// Occurs whenever the <see cref="TotalValue"/> property has changed.
	/// </summary>
	public event EventHandler TotalValueChanged;
	#endregion // Public Events
}
