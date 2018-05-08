using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwichScript : MonoBehaviour {


    public GameObject[] targets_;
    
    void Start()
    {
        targets_ = GameObject.FindGameObjectsWithTag("rotateObj");
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            foreach (GameObject obj in targets_)
            {
                obj.transform.Rotate(new Vector3(0.0f,0.0f,90.0f));
            }
            Debug.Log("hit");
        }
        
    }
}
