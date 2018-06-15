using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPointer : MonoBehaviour {
    [SerializeField]
    private LineRenderer laserPointer;
    // Use this for initialization
    void Start () {
		
	}

    private void LateUpdate()
    {

        laserPointer.SetPosition(0, laserPointer.transform.position);
    }

    // Update is called once per frame
    void Update () {
    }
}
