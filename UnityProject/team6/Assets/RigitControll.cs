using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigitControll : MonoBehaviour {

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            gameObject.GetComponent<Rigidbody>().isKinematic = true;

        if (collision.gameObject.CompareTag("ChangeObject"))
            gameObject.GetComponent<Rigidbody>().isKinematic = false;
    }
}
