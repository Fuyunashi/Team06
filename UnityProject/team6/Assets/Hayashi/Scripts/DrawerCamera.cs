using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawerCamera : MonoBehaviour
{
    private GameObject target;
    private Quaternion targetRotate;
    private GameObject player;
    private GameObject playCamera;
    private Transform m_CameraPos;
    private bool isDrawEnd;
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playCamera = GameObject.FindGameObjectWithTag("PlayCamera");
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
    }

    public void SetDrawerObj(GameObject drawer)
    {
        target = drawer;
        player.GetComponent<Player>().isStop_ = true;
        m_CameraPos = playCamera.transform;
        playCamera.SetActive(false);
    }

    public void TrackingEnd()
    {
        LeanTween.move(this.gameObject, m_CameraPos.transform.position, 1.0f).setOnComplete(() => {
            player.GetComponent<Player>().isStop_ = false;
            playCamera.SetActive(true);
            Destroy(this.gameObject);
        });
    }
}
