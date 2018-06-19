using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class W_Enemy : MonoBehaviour
{

    private Transform target;
    private Vector3 targetPoint;

    private float accelaration;
  
    public float velocity = 2.0f;
    public float accel;
    public float detectionRange;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //MoveToPoint();
        MoveToPlayer();
    }

    void MoveToPlayer()
    {
        gameObject.GetComponent<EnemyNavi>().enabled = true;

        target = GameObject.FindGameObjectWithTag("Player").transform;
        targetPoint = target.position - transform.position;
        velocity += accel * Time.deltaTime;

        if (targetPoint.sqrMagnitude <= detectionRange * detectionRange)
        {
            gameObject.GetComponent<EnemyNavi>().enabled = false;
            //soundManager.instance.EnemyToPlayer();
             
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
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            soundManager.instance.EnemyDead();
            Destroy(gameObject);
        }
    }
}
