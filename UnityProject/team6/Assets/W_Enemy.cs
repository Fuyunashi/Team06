using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class W_Enemy : MonoBehaviour {

    private Transform target;
    private float accelaration;
    private Vector3 targetPoint;
    public float velocity= 5.0f;
    public float accel;
    public float detectionRange;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		MoveToPlayer();
	}
    public void MoveToPlayer()
    {
        target = GameObject.Find("DummyPlayer").transform;
        targetPoint = target.position - transform.position;
        
        accelaration = accel;
        velocity += accelaration * Time.deltaTime;

        if (targetPoint.sqrMagnitude <= detectionRange * detectionRange)
        {
            transform.LookAt(target);
            transform.Translate(Vector3.forward * velocity);
            Debug.Log("In");
        }
        else
        {
            velocity = 0.0f;
            Debug.Log("Out");
        }
    }
}
