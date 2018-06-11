using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class TutorialControll : MonoBehaviour
{

    enum PouseSelect
    {
        ToContinue,
        Restart,
        Select,
    }

    //Xinput関連
    private bool playerInputSet_ = false;
    private PlayerIndex playerIndex_;
    private GamePadState padState_;
    private GamePadState prevState_;

    GameObject MianCam;
    GameObject SubCam;

    [SerializeField]
    GameObject[] PouseRogo;

    GameObject portalPosObj;

    //必要なスクリプトを保持
    GameObject obj_sceneControll;
    SceneControll sceneControll;
    GameObject obj_portal;
    DistortPortal distortPortal;
    GameObject obj_crtNoise;
    CRTnoise crtNoise;
    GameObject obj_cameraInformation;
    CameraInformation cameraInformation;

    PouseSelect pouseSelect;
    int pouseCount = 3;
    int pouseSelectIndex = 0;
    /// <summary>
    /// シーンを移行する際に状態管理のフラグ
    /// </summary>
    bool changeSceneFrag { get; set; }
    /// <summary>
    /// プレイヤーが死亡したかの判断を煽るフラグ
    /// </summary>
    public bool playerDeadFrag { get; set; }
    /// <summary>
    /// ステージがクリアしたかどうかのフラグ
    /// </summary>
    public bool stageClearFrag { get; set; }

    void Start()
    {
        //必要なスクリプトを所持
        obj_sceneControll = GameObject.Find("SceneController");
        sceneControll = obj_sceneControll.GetComponent<SceneControll>();
        obj_portal = GameObject.Find("TutrialCamera");
        distortPortal = obj_portal.GetComponent<DistortPortal>();
        obj_crtNoise = GameObject.Find("TutrialCamera");
        crtNoise = obj_crtNoise.GetComponent<CRTnoise>();
        obj_cameraInformation = GameObject.Find("CameraInformation");
        cameraInformation = obj_cameraInformation.GetComponent<CameraInformation>();

        //ゴールorPortalのメアスオブジェ
        portalPosObj = GameObject.FindGameObjectWithTag("GoleObject");

        //フラグ関係の初期化
        changeSceneFrag = false;
        stageClearFrag = false;
        playerDeadFrag = false;
    }

    void Update()
    {
        //インプット関連
        if (!playerInputSet_ || !prevState_.IsConnected)
        {
            playerIndex_ = (PlayerIndex)0;
            playerInputSet_ = true;
        }
        prevState_ = padState_;
        padState_ = GamePad.GetState(playerIndex_);
        //インプット関連


        if (!sceneControll.PuseFrag)
            PuseDisposal();

        //次ステージにはポウズ中には行けない
        if (!sceneControll.PuseFrag)
        {
            //次ステージに行く際の条件
            if (stageClearFrag)
            {
                distortPortal.portalPos = portalPosObj.transform.position;
                sceneControll.AddToScene.Add((sceneControll.CurrentStage + 1).ToString() + AddToScene.ChildScene);
                distortPortal.portalPos = portalPosObj.transform.position;
                distortPortal.PortalFlag = true;
                changeSceneFrag = true;
                stageClearFrag = false;
            }
        }
        //プレイアーが死んだらリスタート
        if (playerDeadFrag)
        {
            sceneControll.NextScene = SceneName.PlayCurrentScene;
            sceneControll.AddToScene.Add(sceneControll.CurrentStage.ToString() + AddToScene.ChildScene);
        }

        //ポウズ中の処理
        if (sceneControll.PuseFrag)
        {
            Debug.Log("ポウズ");
            Debug.Log("ポウズ中の処理は：" + (PouseSelect)pouseSelectIndex);
            PouseOperation();
            PoseIconColor();
            if (prevState_.Buttons.B == ButtonState.Released && padState_.Buttons.B == ButtonState.Pressed)
            {
                PouseRogo[0].transform.position = new Vector3(-1000, 0, 0);
                switch (pouseSelectIndex)
                {
                    //最初からやり直す
                    case 0:
                        //ポウズを戻す
                        Time.timeScale = 1;
                        sceneControll.PuseFrag = false;
                        break;
                    //続きから始める
                    case 1:
                        sceneControll.PuseFrag = false;
                        Time.timeScale = 1;
                        sceneControll.NextScene = SceneName.TutorialCurrentScene;
                        sceneControll.AddToScene.Add(sceneControll.CurrentStage.ToString() + AddToScene.ChildScene);
                        break;
                    //セレクトシーンへ戻る
                    case 2:
                        //カメラ情報の譲渡
                        cameraInformation.CameraPos = obj_portal.transform.position;
                        cameraInformation.CameraRota = obj_portal.transform.rotation;
                        //ポウズ関係の初期化
                        Time.timeScale = 1;
                        //ノイズが行われてたらシーン移行フラグを入れる
                        crtNoise.CRTFlag = true;
                        changeSceneFrag = true;
                        break;
                }
            }

        }
        //セレクトシーンに移行の際の演出処理
        if (!crtNoise.CRTFlag && changeSceneFrag && sceneControll.PuseFrag)
        {
            sceneControll.PuseFrag = false;
            sceneControll.NextScene = SceneName.SelectScene;
            sceneControll.AddToScene.Add(sceneControll.CurrentStage.ToString() + AddToScene.ChildScene);
            changeSceneFrag = false;
        }
        //次ステージへ移行する際の演出処理
        if (distortPortal.portalTime <= 0 && changeSceneFrag)
        {
            if (sceneControll.CurrentStage == NextStage.Tutrial2)
            {
                sceneControll.NextScene = SceneName.PlayScene;
                sceneControll.AddToScene.Add((sceneControll.CurrentStage + 1).ToString() + AddToScene.ChildScene);
                sceneControll.CurrentStage = sceneControll.CurrentStage + 1;
                changeSceneFrag = false;
                return;
            }
            sceneControll.NextScene = SceneName.TutorialCurrentScene;
            sceneControll.AddToScene.Add((sceneControll.CurrentStage + 1).ToString() + AddToScene.ChildScene);
            sceneControll.CurrentStage = sceneControll.CurrentStage + 1;
            changeSceneFrag = false;
        }

    }
    /// <summary>
    /// ポウズ中に行うシーン選択
    /// </summary>
    private void PouseOperation()
    {
        if (prevState_.DPad.Up == ButtonState.Released && padState_.DPad.Up == ButtonState.Pressed)
        {
            pouseSelectIndex--;
            pouseSelectIndex += pouseCount;
            pouseSelectIndex = pouseSelectIndex % pouseCount;

        }
        else if (prevState_.DPad.Down == ButtonState.Released && padState_.DPad.Down == ButtonState.Pressed)
        {
            pouseSelectIndex++;
            pouseSelectIndex = pouseSelectIndex % pouseCount;
        }
    }
    /// <summary>
    /// ポーズ処理
    /// </summary>
    private void PuseDisposal()
    {
        if (prevState_.Buttons.Start == ButtonState.Released && padState_.Buttons.Start == ButtonState.Pressed && !sceneControll.PuseFrag)
        {
            PouseRogo[0].transform.position = obj_portal.transform.position + new Vector3(0, 0, 0.46f);
            for (int i = 0; i < PouseRogo.Length; i++)
                LeanTween.alpha(PouseRogo[i], 1.0f, 1f);
            Debug.Log("ポウズ");
            Time.timeScale = 0;
            sceneControll.PuseFrag = true;
            return;
        }
    }
    private void PoseIconColor()
    {
        switch (pouseSelectIndex)
        {
            //最初からやり直す
            case 0:
                PouseRogo[1].GetComponent<Renderer>().material.color = Color.red;
                PouseRogo[2].GetComponent<Renderer>().material.color = Color.white;
                PouseRogo[3].GetComponent<Renderer>().material.color = Color.white;

                break;
            //続きから始める
            case 1:
                PouseRogo[2].GetComponent<Renderer>().material.color = Color.red;
                PouseRogo[1].GetComponent<Renderer>().material.color = Color.white;
                PouseRogo[3].GetComponent<Renderer>().material.color = Color.white;
                break;
            //セレクトシーンへ戻る
            case 2:
                PouseRogo[3].GetComponent<Renderer>().material.color = Color.red;
                PouseRogo[2].GetComponent<Renderer>().material.color = Color.white;
                PouseRogo[1].GetComponent<Renderer>().material.color = Color.white;
                break;
        }
    }
}

