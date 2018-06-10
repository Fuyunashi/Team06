using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class modeldirection : MonoBehaviour {

    [SerializeField]
    Camera camera_;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {


        this.transform.forward = camera_.transform.forward;
	}
}
