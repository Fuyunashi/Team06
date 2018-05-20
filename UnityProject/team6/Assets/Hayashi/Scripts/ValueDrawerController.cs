using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ValueDrawerController : MonoBehaviour {

    private GameObject target;
    private Quaternion targetRotate;
    private GameObject drawObj;

	// Use this for initialization
	void Start () {
        target = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {
        if (target != null)
        {
            targetRotate = Quaternion.LookRotation(target.transform.position - transform.localPosition);
            transform.localRotation = Quaternion.Slerp(transform.rotation, targetRotate, 1);
        }
        transform.position = new Vector3(drawObj.transform.localPosition.x, drawObj.transform.localPosition.y + 1.0f, drawObj.transform.localPosition.z);

	}

    public void GetDrawBaseObj(GameObject m_obj)
    {
        drawObj = m_obj;
    }


}
