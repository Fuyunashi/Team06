using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class W_Enemy : MonoBehaviour
{

    private Transform target;
    private Vector3 targetPoint;
    //private Vector3 moveTargetPoint;
    //private int arrCount;
    private float accelaration;
    //private bool movePointHit;

    //public GameObject[] movePoint;
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

    //void MoveToPoint()
    //{
    //    Vector3 movePosition;
    //    Debug.Log(arrCount);
    //    movePosition = movePoint[arrCount].transform.position;
    //    moveTargetPoint = movePosition - transform.position;

    //    if (movePointHit == false)
    //    {
    //        transform.Translate(Vector3.forward * velocity);
    //        velocity += accel * Time.deltaTime;
    //        transform.LookAt(movePosition);
    //    }
    //    if (movePointHit == true)
    //    {
    //        velocity = 0;
    //        arrCount = 1;
    //    }
    //}
    void MoveToPlayer()
    {
        gameObject.GetComponent<EnemyNavi>().enabled = true;

        target = GameObject.FindGameObjectWithTag("Player").transform;
        targetPoint = target.position - transform.position;
        velocity += accel * Time.deltaTime;

        if (targetPoint.sqrMagnitude <= detectionRange * detectionRange)
        {
            gameObject.GetComponent<EnemyNavi>().enabled = false;

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
            Destroy(gameObject);
        }
    }
}
