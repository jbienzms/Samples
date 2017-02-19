using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
	#region Member Variables
	private int numberToGuess;
	#endregion // Member Variables

	#region Inspector Variables
	[Tooltip("The audio source used to play answer sounds.")]
	public AudioSource answerAudioSource;

	[Tooltip("The object that is shown when the answer is correct.")]
	public GameObject answerCorrectObject;

	[Tooltip("The sound that is played when the answer is correct.")]
	public AudioClip answerCorrectSound;

	[Tooltip("The object that is shown when the answer is incorrect.")]
	public GameObject answerIncorrectObject;

	[Tooltip("The sound that is played when the answer is incorrect.")]
	public AudioClip answerIncorrectSound;

	[Tooltip("The text label that provides captions.")]
	public Text captionsText;

	[Tooltip("The GameObject that can be used to check the answer.")]
	public GameObject checkAnswerObject;

	[Tooltip("The text label that represents total values.")]
	public Text totalText;

	[Tooltip("The bit manager the scene.")]
	public BitManager bitManager;
	#endregion // Inspector Variables

	#region Behavior Overrides
	// Use this for initialization
	void Start()
	{
		
	}

	void OnDisable()
	{
		// Hide things game specific
		checkAnswerObject.SetActive(false);
	}

	void OnEnable()
	{
		// Show things that are game specific
		checkAnswerObject.SetActive(true);

		// Set game description
		captionsText.text = "Try to make this number:";

		// Start a new game
		NewGame();
	}
	#endregion // Behavior Overrides

	#region Public Methods
	/// <summary>
	/// Checks the answer to see if it's correct.
	/// </summary>
	public void CheckAnswer()
	{
		// Set audio source and show image
		if (numberToGuess == bitManager.TotalValue)
		{
			answerAudioSource.clip = answerCorrectSound;
			answerCorrectObject.SetActive(true);
		}
		else
		{
			answerAudioSource.clip = answerIncorrectSound;
			answerIncorrectObject.SetActive(true);
		}

		// Play the sound
		answerAudioSource.Play();

		// Wait 2 seconds and start a new game
		Invoke("NewGame", 2);
	}

	/// <summary>
	/// Starts a new game
	/// </summary>
	public void NewGame()
	{
		// Reset all bits
		bitManager.ResetAllBits();

		// Reset answer state
		answerCorrectObject.SetActive(false);
		answerIncorrectObject.SetActive(false);

		// Generate a number to guess
		numberToGuess = Random.Range(1, 255);

		// Show it in the total box
		totalText.text = "" + numberToGuess;
	}
	#endregion // Public Methods
}
