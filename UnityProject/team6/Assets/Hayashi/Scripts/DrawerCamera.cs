using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawerCamera : MonoBehaviour
{
    private GameObject target;
    private Quaternion targetRotate;
    private GameObject player;
    private bool isDrawEnd;
    // Use this for initialization
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        isDrawEnd = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            targetRotate = Quaternion.LookRotation(target.transform.position - this.transform.position);
            transform.localRotation = Quaternion.Slerp(transform.rotation, targetRotate, 0.05f);
            transform.position = Vector3.Lerp(transform.position, target.transform.position + new Vector3(0, 0, -2f), 0.03f);
        }
        if (isDrawEnd == true)
        {
            transform.position = Vector3.Lerp(transform.position, player.transform.position + new Vector3(0, 1f,0), 0.2f);
            if (Vector3.Distance(transform.position, player.transform.position + new Vector3(0, 1f, 0)) <= 0.3f)
            {
                Destroy(this.gameObject);
            }
        }
    }

    public void SetDrawerObj(GameObject drawer)
    {
        target = drawer;
    }

    public void TrackingEnd()
    {
        isDrawEnd = true;
    }
}
