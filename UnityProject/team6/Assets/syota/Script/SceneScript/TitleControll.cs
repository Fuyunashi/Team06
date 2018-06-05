using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;



public class TitleControll : MonoBehaviour
{
    /* シーン遷移の際に絶対に必要変数*/
    GameObject MainCam;
    GameObject SubCam;
    GameObject scene;
    SceneControll sceneControll;
    GameObject portal;
    DistortPortal distortPortal;
    /* シーン遷移の際に絶対に必要変数*/

    //-------//
    GameObject sceneChangeIcon;
    GameObject obj;
    CRTnoise crtNoise;
    bool sceneChangeFrag;

    //string name;
    void Start()
    {
        //持っているカメラ情報を取得
        MainCam = GameObject.Find("TitleMainCamera");
        SubCam = GameObject.Find("TitleSubCamera");
        //シーン情報を取得
        scene = GameObject.Find("SceneController");
        sceneControll = scene.GetComponent<SceneControll>();

        //ポータルに支持を出すための変数      
        distortPortal = MainCam.GetComponent<DistortPortal>();
        //現在のアクティブなシーンを取得

        obj = GameObject.Find("TitleMainCamera");
        crtNoise = obj.GetComponent<CRTnoise>();
        //演出の関係上必要になったフラグ
        sceneChangeFrag = false;
    }
    void Update()
    {
        sceneChangeIcon = GameObject.Find("SceneChangeIcon");
      
                
        //ノイズが行われてたらシーン移行フラグを入れる
        if (crtNoise.CRTFlag)
            sceneChangeFrag = true;
        //シーンのフラグが入り、ノイズが終わった報告があったらしーんを移行する
        if (!crtNoise.CRTFlag && sceneChangeFrag)
            sceneControll.NextScene = SceneName.SelectScene;
    }
    private void AddChildScene()
    {
        //sceneControll.AddToScene.Add(AddToScene.SelectScene);
    }


}
