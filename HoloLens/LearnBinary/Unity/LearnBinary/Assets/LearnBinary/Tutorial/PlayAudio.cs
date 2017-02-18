using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAudio : MonoBehaviour {
    public List<AudioSource> audioToPlay;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void AudioToPlay(int whichAudioToPlay)
    {
        audioToPlay[whichAudioToPlay].Play();
    }
}
