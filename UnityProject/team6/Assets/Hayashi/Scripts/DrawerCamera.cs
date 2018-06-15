using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawerCamera : MonoBehaviour
{
    private GameObject target;
    private Quaternion targetRotate;
    private GameObject player;
    private bool isDrawEnd;
    void Awake()
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
            transform.localRotation = Quaternion.Slerp(transform.rotation, targetRotate, 2.0f*Time.deltaTime);
            transform.position = Vector3.Lerp(transform.position, target.transform.position + new Vector3(0, 0, -2f), 2.0f*Time.deltaTime);
        }
        if (isDrawEnd == true)
        {
            transform.position = Vector3.Lerp(transform.position, player.transform.position + new Vector3(0, 1f, 0), 5.0f*Time.deltaTime);
            if (Vector3.Distance(transform.position, player.transform.position + new Vector3(0, 1f, 0)) <= 0.3f)
            {
                player.GetComponent<Player>().isStop_ = false;
                Destroy(this.gameObject);
            }
        }
    }

    public void SetDrawerObj(GameObject drawer)
    {
        target = drawer;
        player.GetComponent<Player>().isStop_ = true;
    }

    public void TrackingEnd()
    {
        isDrawEnd = true;
    }
}
