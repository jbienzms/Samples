using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{


    private int number;
    [Tooltip("The text label that provides captions.")]
    public Text captionsText;

    [Tooltip("The text label that represents total values.")]
    public Text totalText;

    [Tooltip("The bit manager the scene.")]
    public BitManager bitManager;

    AudioSource audioSource;

    public Image checkImage;
    public AudioClip correctAudio;
    public Sprite correctSprite;
    public AudioClip incorrectAudio;
    public Sprite incorrectSprite;

    // Use this for initialization
    void Start()
    {
        number = 0;
        audioSource = GetComponent<AudioSource>(); 
        
        startGame();
        captionsText.text = "Try to find this number:";
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnDestroy()
    {
      
    }

    public void startGame()
    {
        bitManager.ResetAllBits();
        checkImage.enabled=false;
        number = Random.Range(1, 255);
        totalText.text = "" + number;
    }

    public void checkValue()
    {
        if (number == bitManager.TotalValue)
        {
            audioSource.clip = correctAudio;
            checkImage.sprite = correctSprite;
        }
        else
        {
            audioSource.clip = incorrectAudio;
            checkImage.sprite = incorrectSprite;
        }
        audioSource.Play();
        checkImage.enabled = true;
        Invoke("startGame", 2);

    }


}
