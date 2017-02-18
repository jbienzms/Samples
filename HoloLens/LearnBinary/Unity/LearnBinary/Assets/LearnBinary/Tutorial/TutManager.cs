using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutManager : MonoBehaviour {
    public Animator animator;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void RepeatStep()
    {
        animator.SetBool("repeatStep", true);
    }

    public void NextStep()
    {
        animator.SetBool("nextStep", true);
    }

    public void PreviousStep()
    {
        animator.SetBool("prevStep", true);
    }
}
