﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewCamera : MonoBehaviour {

    public GameObject player_;
    public GameObject camera_;
    public float speed_;
    private Transform playerTransform_;
    private Transform cameraTransform_;

    private Vector3 velocity_;

	// Use this for initialization
	void Start () {

        playerTransform_ = transform.parent;
        cameraTransform_ = GetComponent<Transform>();
	}
	
	// Update is called once per frame
	void Update () {

        float x_rotation = Input.GetAxis("Mouse X");
        float y_rotation = Input.GetAxis("Mouse Y");
        playerTransform_.transform.Rotate(0, x_rotation, 0);
        cameraTransform_.transform.Rotate(-y_rotation, 0, 0);

        float angleDir = playerTransform_.transform.eulerAngles.y * (Mathf.PI / 180.0f);
        Vector3 dir1 = new Vector3(Mathf.Sin(angleDir), 0, Mathf.Cos(angleDir));
        Vector3 dir2 = new Vector3(-Mathf.Cos(angleDir), 0, Mathf.Sin(angleDir));

        if (Input.GetKey(KeyCode.W))
        {
            velocity_.z += 1;
            //playerTransform_.transform.position += dir1 * speed_ * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            velocity_.x -= 1;
            //playerTransform_.transform.position += dir2 * speed_ * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            velocity_.z -= 1;
            //playerTransform_.transform.position += -dir2 * speed_ * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            velocity_.x -= 1;
            //playerTransform_.transform.position += -dir1 * speed_ * Time.deltaTime;
        }
    }
}