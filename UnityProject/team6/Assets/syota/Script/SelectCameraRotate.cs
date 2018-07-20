using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectCameraRotate : MonoBehaviour
{
    enum CameraState
    {
        Op,
        Select,
        Next,
        None,
    }
    enum RotaDic
    {
        Right,
        Left,
        None,
    }
    [SerializeField]
    public Vector3 ObserverPos;
    [SerializeField]
    public GameObject SurveillanceCamera;
    //GameObject camera;
    //カメラの移動する向き
    RotaDic cameraMoveDic;
    //カメラの行える動作
    CameraState cameraState;

    //CRTスクリプトをもらう
    GameObject obj;
    CRTnoise crtnoise;
    //次のシーンを管理しているオブジェクトをもらう
    GameObject obj_;
    StageInstructs stageInstructs;

    //正方形なのでむいてる場所を０から３で番号をつける
    readonly int dirctionNumLength = 4;
    //今向いている方向の変数(同時に初期化)
    int currrentIndex;

    //カメラの移動スピード
    float cameraSpeed = 4;
    //動くまでのカウントダウン
    float countdown = 2;

    //初期座標のメアスオブジェクト
    GameObject titleTV;
    //次のステージを移しているTVオブジェクト
    GameObject nextStagaeTV;
    void Start()
    {
        cameraState = CameraState.Op;
        currrentIndex = 0;

        //カメラの初期座標を入れる
        titleTV = GameObject.Find("TitleTV");

        /* ほしいスクリプトの中身をもらう */
        obj = GameObject.Find("TitleRoomCamera");
        crtnoise = obj.GetComponent<CRTnoise>();
        obj_ = GameObject.Find("StageConfiguration");
        stageInstructs = obj_.GetComponent<StageInstructs>();
        //transform.position = titleTV.transform.position+new Vector3();
        //年のため向きを初期化
        //transform.rotation = new Quaternion(.0f, .0f, .0f, .0f);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("うごいてるよー" + crtnoise.CRTFlag);
        switch (cameraState)
        {
            case CameraState.Op:
                countdown -= Time.deltaTime;
                crtnoise.CRTFlag = true;
                //crtnoise.cameraName = CRTnoise.CameraName.SelectMainCamera;
                if (countdown >= 0)
                    return;
                var pos = ObserverPos - transform.position;
                pos.Normalize();
                if (Vector3.Distance(transform.position, ObserverPos) >= Time.deltaTime * cameraSpeed)
                {
                    transform.position += pos * Time.deltaTime * cameraSpeed;
                    return;
                }
                var aim = SurveillanceCamera.transform.position - transform.position;
                var look = Vector3.RotateTowards(transform.forward, aim, 0.5f * Time.deltaTime, 0f);
                transform.rotation = Quaternion.LookRotation(look);
                if (Input.GetKeyDown(KeyCode.V))
                    cameraState = CameraState.Select;
                break;
            case CameraState.Select:
                if (Input.GetKey(KeyCode.RightArrow))
                {
                    transform.Rotate(0f, 1.0f, 0f);
                }
                else if (Input.GetKey(KeyCode.LeftArrow))
                {
                    transform.Rotate(0f, -1.0f, 0f);
                }
                if (Input.GetKeyDown(KeyCode.K))
                {
                    cameraState = CameraState.Next;

                    nextStagaeTV = GameObject.Find("Tutrial1");
                    Debug.Log("次のシーンは私だ：" + nextStagaeTV);
                    //nextStagaeTV.transform.forward;
                }
                break;
            case CameraState.Next:
                //var a = GameObject.Find(nextStagaeTV.name);
                //var b = a.GetComponent<MonitorTest>();
                //var offset = b.back * 0.1f; ;
                //var c = new Vector3(nextStagaeTV.transform.position.x, nextStagaeTV.transform.position.y, offset.z);
                var offset = nextStagaeTV.transform.position;
                Debug.Log("ほしい座標情報：" + offset);
                var aim_ = nextStagaeTV.transform.localPosition - transform.localPosition;
                var look_ = Vector3.RotateTowards(transform.forward, aim_, 0.5f * Time.deltaTime, 0f);
                transform.rotation = Quaternion.LookRotation(look_);
                transform.position = Vector3.MoveTowards(transform.position, offset, 2 * Time.deltaTime);

                break;
        }
    }
 

}
