using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BitController : MonoBehaviour {
    private bool _isOn = false;
    public float power;
    private float value;


    public Light pointLight;
    public Animator onOff;

    public Material on;
    public Material off;
    public GameObject filament;

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        if(Input.GetKeyDown("space"))
        {
            onOff.SetBool("toggleSwitch", !onOff.GetBool("toggleSwitch"));
            Debug.Log(IsOn);
        }
    }

    public bool IsOn
    {
        get { return _isOn; }
        set
        {
            if(_isOn != onOff.GetBool("toggleSwitch"))
            {
                Debug.Log("Triggered");
                _isOn = onOff.GetBool("toggleSwitch");
                if (_isOn)
                {
                    CalculateValue();
                    filament.GetComponent<Renderer>().material = on;
                }
                else
                {
                    this.value = 0f;
                    filament.GetComponent<Renderer>().material = off;
                }
            }
        }
    }

    public void CalculateValue()
    {
        Value = Mathf.Pow(2f, power);
    }

    public float Value
    {
        get
        {
            return value;
        }
        private set
        {
            this.value = value;
            if (ValueChanged != null)
            {
                ValueChanged(this, EventArgs.Empty);
            }
        }

    }

    public event EventHandler ValueChanged;
}
