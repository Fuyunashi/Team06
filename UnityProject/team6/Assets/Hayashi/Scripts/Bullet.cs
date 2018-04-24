using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    [SerializeField]
    private float speed=2.0f;

    private GameObject hitObject;
    private bool isHit;

    private Shooter m_test;

	// Use this for initialization
	void Start () {
        isHit = false;
        m_test = GameObject.Find("GameManager").GetComponent<Shooter>();
        Destroy(this.gameObject, 2.0f);
	}
	
	// Update is called once per frame
	void Update () {
        transform.position += transform.forward * speed * Time.deltaTime;
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "ChangeObject")
        {
            hitObject = other.gameObject;
            m_test.HitBullet(hitObject);
        }
        Destroy(this.gameObject);
    }

}
