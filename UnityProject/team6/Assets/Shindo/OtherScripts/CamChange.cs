using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamChange : MonoBehaviour {

    public bool isFps_ { get; set; }

	// Use this for initialization
	void Start () {

        isFps_ = false;
	}
	
	// Update is called once per frame
	void Update () {

        if (isFps_)
        {
            GetComponent<FpsCam>().enabled = true;
            GetComponent<TPVCamera>().enabled = false;
        }
        else
        {
            GetComponent<FpsCam>().enabled = false;
            GetComponent<TPVCamera>().enabled = true;
        }
	}
}
