using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMove : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 10f;

    private Vector3 value;
    private bool isPositionMove;
    private bool isScaleMove;
    private bool isHitObj;

    // Use this for initialization
    void Start()
    {
        isPositionMove = false;
        isHitObj = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isPositionMove == true)
        {
            PositionMove();
        }
        if (isScaleMove == true)
        {
            ScaleMove();
        }
    }

    public void PositionShiftStart(Vector3 originValue)
    {
        value = originValue;
        isPositionMove = true;
    }

    private void PositionMove()
    {
        if (isHitObj == false)
        {
            transform.parent.transform.parent.position = Vector3.MoveTowards(transform.parent.transform.parent.position, value, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.parent.transform.parent.position, value) == 0) isPositionMove = false;
        }
    }

    public void ScaleShiftStart(Vector3 originValue)
    {
        value = originValue;
        isScaleMove = true;
    }

    private void ScaleMove()
    {
        if (isHitObj == false)
        {
            transform.parent.localScale = Vector3.MoveTowards(transform.parent.localScale, value, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.parent.transform.parent.localScale, value) == 0) isPositionMove = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("toota");
        if (collision.gameObject.tag != "moveObject")
        {
            isHitObj = true;
            Destroy(transform.parent.transform.parent.gameObject);
        }
    }


}
