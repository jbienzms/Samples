using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BitController : MonoBehaviour {
    private bool isOn;
    public int power;
    private int value;
    private bool valueOverride;


    private Light bitlight;
    public Animator bitAnimator;
    public Text valueText;
    public Text valueSign;

    public Material on;
    public Material off;
    public GameObject filament;

    // Use this for initialization
    void Start () {

	}

    void OnSelect()
    {
        if (bitAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
        {
            bitAnimator.SetBool("toggleSwitch", !bitAnimator.GetBool("toggleSwitch"));
        }
    }

    // Update is called once per frame
    void Update () {
        if(Input.GetKeyDown("space"))
        {
            if (bitAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
            {
                bitAnimator.SetBool("toggleSwitch", !bitAnimator.GetBool("toggleSwitch"));
            }
        }
    }

    public bool IsOn
    {
        get { return isOn; }
        private set
        {
            isOn = value;
            Debug.Log("Triggered");

            if (isOn)
            {
                CalculateValue();
                filament.GetComponent<Renderer>().material = on;
            }
            else
            {
                this.value = 0;
                filament.GetComponent<Renderer>().material = off;
            }
        }
    }

    public void CalculateValue()
    {
        int realValue = (int)Mathf.Pow(2f, power);
        Value = (isOn ? realValue : 0);
        if (valueText != null)
        {
            valueText.text = (isOn | valueOverride ? realValue.ToString() : "0");
        }

        if (valueSign != null)
        {
            valueSign.text = realValue.ToString();
        }
    }

    public void ToggleSwitch()
    {
        CalculateValue();
        if (isOn)
        {
            filament.GetComponent<Renderer>().material = on;
            isOn = !isOn;
        }
        else
        {
            Value = 0;
            filament.GetComponent<Renderer>().material = off;
            isOn = !isOn;
        }
    }

    public int Value
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

    public bool ValueOverride
    {
        get
        {
            return valueOverride;
        }

        set
        {
            valueOverride = value;
            CalculateValue();
        }
    }

    public Light Bitlight
    {
        get
        {
            return bitlight;
        }

        set
        {
            bitlight = value;
        }
    }

    public event EventHandler ValueChanged;
}
