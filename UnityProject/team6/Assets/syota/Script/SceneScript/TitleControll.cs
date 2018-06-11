using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;



public class TitleControll : MonoBehaviour
{
    /* */
    GameObject MainCam;
    GameObject SubCam;

    //ほしいスクリプトを保持
    GameObject obj_scene;
    SceneControll sceneControll;
    GameObject obj_portal;
    DistortPortal distortPortal;
    GameObject sceneChangeIcon;
    GameObject obj_crtNoise;
    CRTnoise crtNoise;
    GameObject obj_cameraInformation;
    CameraInformation cameraInformation;
    bool sceneChangeFrag;

    public bool playerDeadFrag { get; set; }

    //string name;
    void Start()
    {
        //持っているカメラ情報を取得
        MainCam = GameObject.Find("TitleMainCamera");
        SubCam = GameObject.Find("TitleSubCamera");

        //使用するスクリプトを保持
        //シーン情報を取得
        obj_scene = GameObject.Find("SceneController");
        sceneControll = obj_scene.GetComponent<SceneControll>();
        //ポータルに支持を出すための変数      
        distortPortal = MainCam.GetComponent<DistortPortal>();
        obj_crtNoise = GameObject.Find("TitleMainCamera");
        crtNoise = obj_crtNoise.GetComponent<CRTnoise>();
        obj_cameraInformation = GameObject.Find("CameraInformation");
        cameraInformation = obj_cameraInformation.GetComponent<CameraInformation>();

        //演出の関係上必要になったフラグ
        sceneChangeFrag = false;

        //SoundManager.GetInstance.PlayBGM("再生したいファイル名（完全一致）");
    }
    void Update()
    {
        sceneChangeIcon = GameObject.Find("SceneChangeIcon");


        //ノイズが行われてたらシーン移行フラグを入れる
        if (crtNoise.CRTFlag)
            sceneChangeFrag = true;
        //シーンのフラグが入り、ノイズが終わった報告があったらしーんを移行する
        if (!crtNoise.CRTFlag && sceneChangeFrag)
        {
            //カメラ情報の譲渡
            cameraInformation.CameraPos = MainCam.transform.position;
            cameraInformation.CameraRota = MainCam.transform.rotation;
            sceneControll.NextScene = SceneName.SelectScene;
            sceneControll.AddToScene.Add(SceneName.TitleRoom.ToString());
            sceneChangeFrag = false;

        }
    }
    private void AddChildScene()
    {
        //sceneControll.AddToScene.Add(AddToScene.SelectScene);
    }


}
