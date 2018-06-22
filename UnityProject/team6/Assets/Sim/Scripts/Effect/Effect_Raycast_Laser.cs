using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Effect_Raycast_Laser : MonoBehaviour {
    
    public GameObject raybody; //레이캐스팅을 쏘는 위치
    public GameObject scaleDistance; //거리에 따른 스케일 변화를 위한 오브젝트 대상
    public GameObject rayResult; //충돌하는 위치에 출력할 결과
    private GameObject hitObject;
    PlayControll playControll_;
    GameObject obj_playControll_;

    private bool isDead;

    // Use this for initialization
    void Start () {
        isDead = false;
        //プレイヤーの認識
        if (SceneManager.GetActiveScene().name == SceneName.PlayScene.ToString())
        {
            obj_playControll_ = GameObject.Find("PlayControll");
            playControll_ = obj_playControll_.GetComponent<PlayControll>();
        }
    }

    // Update is called once per frame
    void Update () {

        RaycastHit hit; 

        Physics.Raycast(transform.position, transform.forward, out hit, 200);

        scaleDistance.transform.localScale = new Vector3(1, hit.distance, 1);
        
        rayResult.transform.position = hit.point;

        rayResult.transform.rotation = Quaternion.LookRotation(hit.normal);

        if (hit.collider.gameObject)
        {
            hitObject = hit.collider.gameObject;
            SoundManager.GetInstance.PlaySE("Laser_SE");
            if (hitObject.tag == "Player" && isDead==false)
            {
                SoundManager.GetInstance.PlaySE("LaserHit_SE");
                if (SceneManager.GetActiveScene().name == SceneName.PlayScene.ToString())
                {
                    playControll_.playerDeadFrag = true;
                }
                isDead = true;
            }
        }
    }
}
