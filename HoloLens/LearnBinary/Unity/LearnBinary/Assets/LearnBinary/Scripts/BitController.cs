using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BitController : MonoBehaviour {
    private bool _isOn = false;
    public float power;
    public float value;


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

    }

    public bool isOnValue
    {
        get { return _isOn; }
        set
        {
            if(_isOn != onOff.GetBool("toggleSwitch"))
            {
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
        value = Mathf.Pow(2f, power);
    }
}
