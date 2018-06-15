using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class modeldirection : MonoBehaviour {

    [SerializeField]
    Camera camera_;
    [SerializeField]
    GameObject target_;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //カメラに追従
        this.transform.position = target_.transform.position;
        //カメラの向きをとる
        this.transform.forward = camera_.transform.TransformDirection(Vector3.forward);
	}
}
