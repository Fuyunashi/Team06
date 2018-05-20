using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    [SerializeField]
    private float speed=2.0f;

    private GameObject hitObject;

    private Shooter m_Shooter;

	// Use this for initialization
	void Start () {
        m_Shooter = GameObject.FindGameObjectWithTag("Player").GetComponent<Shooter>();
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
            m_Shooter.HitBullet(hitObject);
        }
        Destroy(this.gameObject);
    }

}
