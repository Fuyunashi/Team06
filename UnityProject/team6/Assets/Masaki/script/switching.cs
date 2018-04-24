using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class switching : MonoBehaviour {
    public bool checkSwitch = false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            Debug.Log("switch");
            checkSwitch = !checkSwitch;
        }
    }
}
