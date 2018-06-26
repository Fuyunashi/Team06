using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;
using UnityEngine.UI;
using Cinemachine;

public class PlayControll : MonoBehaviour
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
    Image[] PouseRogo;

    [SerializeField]
    GameObject outBlack;

    GameObject portalPosObj;

    //必要なスクリプトを保持
    GameObject obj_sceneControll;
    SceneControll sceneControll;
    GameObject obj_portal;
    DistortPortal distortPortal;
    GameObject obj_cameraInformation;
    CameraInformation cameraInformation;
    GameObject obj_crtNoise;
    CRTnoise crtNoise;

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

    bool lastStageClearFrag { get; set; }
    bool outBlackAlpha;
    void Start()
    {
        //必要なスクリプトを所持
        obj_sceneControll = GameObject.Find("SceneController");
        sceneControll = obj_sceneControll.GetComponent<SceneControll>();
        obj_portal = GameObject.Find("PlayCamera");
        distortPortal = obj_portal.GetComponent<DistortPortal>();
        obj_crtNoise = GameObject.Find("PlayCamera");
        crtNoise = obj_crtNoise.GetComponent<CRTnoise>();
        obj_cameraInformation = GameObject.Find("CameraInformation");
        cameraInformation = obj_cameraInformation.GetComponent<CameraInformation>();
        //ゴールorPortalのメアスオブジェ
        portalPosObj = GameObject.FindGameObjectWithTag("GoleObject");
        //キャンバスを最初は消しておく
        foreach (var image in PouseRogo)
            image.enabled = false;

        //フラグ関係の初期化
        changeSceneFrag = false;
        stageClearFrag = false;
        playerDeadFrag = false;
        outBlackAlpha = true;

        lastStageClearFrag = false;

        pouseSelect = PouseSelect.ToContinue;

        //シーン移行の際につかう黒い画像なので最初は表示しない
        // LeanTween.colorText(outBlack.GetComponent<RectTransform>(), new Color(1, 1, 1, 0), 0.5f);
    }


    void Update()
    {
        if (outBlack.GetComponent<Image>().color.a >= 0.0f && outBlackAlpha)
        {
            outBlack.GetComponent<Image>().color = new Color(1, 1, 1, outBlack.GetComponent<Image>().color.a - 0.1f);
        }


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
        if (Input.GetKeyDown(KeyCode.L)) stageClearFrag = true;


        //次ステージにはポウズ中には行けない
        if (!sceneControll.PuseFrag)
        {
            if (stageClearFrag)
            {
                SoundManager.GetInstance.PlaySE("Goal_SE");
                GameObject.Find("FPSPlayer").GetComponent<Player>().isStop_ = true;
                distortPortal.portalPos = portalPosObj.transform.position;
                if (sceneControll.CurrentStage == NextStage.Stage8)
                    sceneControll.AddToScene.Add((sceneControll.CurrentStage-1).ToString() + AddToScene.ChildScene);
                else
                    sceneControll.AddToScene.Add((sceneControll.CurrentStage + 1).ToString() + AddToScene.ChildScene);
                distortPortal.portalPos = portalPosObj.transform.position;
                distortPortal.PortalFlag = true;
                changeSceneFrag = true;
                stageClearFrag = false;
                //Debug.Log("クリアしたよ" + sceneControll.CurrentStage);
            }
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
                SoundManager.GetInstance.PlaySE("input_SE3");

                //キャンバスを最初は消しておく
                foreach (var image in PouseRogo)
                    image.enabled = false;


                switch (pouseSelectIndex)
                {
                    //最初からやり直す
                    case 0:
                        //ポウズを戻す
                        Time.timeScale = 1;
                        sceneControll.PuseFrag = false;
                        break;
                    //最初から
                    case 1:
                        //Debug.Log("リスタート白や：" + (PouseSelect)pouseSelectIndex);
                        //sceneControll.PuseFrag = false;
                        Time.timeScale = 1;
                        //ノイズが行われてたらシーン移行フラグを入れる
                        crtNoise.CRTFlag = true;
                        changeSceneFrag = true;

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

            //シーンのフラグが入り、ノイズが終わった報告があったらしーんを移行する
        }
        if (!crtNoise.CRTFlag && changeSceneFrag && playerDeadFrag)
        {
            //画面を暗くする
            outBlack.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            outBlackAlpha = false;
            sceneControll.PuseFrag = false;
            playerDeadFrag = false;

            sceneControll.NextScene = SceneName.PlayCurrentScene;
            sceneControll.AddToScene.Add(sceneControll.CurrentStage.ToString() + AddToScene.ChildScene);

            changeSceneFrag = false;
        }
        //セレクトシーンに移行の際の演出処理
        if (!crtNoise.CRTFlag && changeSceneFrag && sceneControll.PuseFrag)
        {
            //画面を暗くする
            outBlack.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            outBlackAlpha = false;
            sceneControll.PuseFrag = false;
            switch (pouseSelectIndex)
            {
                case 1:
                    sceneControll.NextScene = SceneName.PlayCurrentScene;
                    sceneControll.AddToScene.Add(sceneControll.CurrentStage.ToString() + AddToScene.ChildScene);
                    break;
                case 2:
                    sceneControll.NextScene = SceneName.SelectScene;
                    sceneControll.AddToScene.Add(sceneControll.CurrentStage.ToString() + AddToScene.ChildScene);
                    break;
            }

            changeSceneFrag = false;
        }
        if (distortPortal.portalTime <= 1.2f && changeSceneFrag)
        {
            //最後のステージの際は
            if (sceneControll.CurrentStage == NextStage.Stage8)
            {
                crtNoise.CRTFlag = true;
                changeSceneFrag = false;
                distortPortal.PortalFlag = false;
                lastStageClearFrag = true;
            }
        }
        if (!crtNoise.CRTFlag  && lastStageClearFrag)
        {
            Debug.Log("セレクトしーんへ遷移白");
            outBlack.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            sceneControll.NextScene = SceneName.SelectScene;
            sceneControll.AddToScene.Add(sceneControll.CurrentStage.ToString() + AddToScene.ChildScene);
            lastStageClearFrag = false;
        }
        //次ステージへ移動する際の演出処理
        if (distortPortal.portalTime <= 0 && changeSceneFrag)
        {
            if (sceneControll.CurrentStage != NextStage.Stage8)
            {
                sceneControll.NextScene = SceneName.PlayCurrentScene;
                sceneControll.AddToScene.Add((sceneControll.CurrentStage + 1).ToString() + AddToScene.ChildScene);
                sceneControll.CurrentStage = sceneControll.CurrentStage + 1;
                changeSceneFrag = false;
            }
        }
        if (distortPortal.portalTime <= .7f&& sceneControll.CurrentStage != NextStage.Stage8)
        {
            outBlack.GetComponent<Image>().color = new Color(1, 1, 1, outBlack.GetComponent<Image>().color.a + 0.2f);
        }

        //プレイアーが死んだらリスタート
        if (playerDeadFrag)
        {
            if (!crtNoise.CRTFlag)
                crtNoise.CRTFlag = true;
            changeSceneFrag = true;
        }
    }
    /// <summary>
    /// ポウズ中に行うシーン選択
    /// </summary>
    private void PouseOperation()
    {
        if (prevState_.DPad.Up == ButtonState.Released && padState_.DPad.Up == ButtonState.Pressed)
        {
            SoundManager.GetInstance.PlaySE("input_SE1");

            pouseSelectIndex--;
            pouseSelectIndex += pouseCount;
            pouseSelectIndex = pouseSelectIndex % pouseCount;

        }
        else if (prevState_.DPad.Down == ButtonState.Released && padState_.DPad.Down == ButtonState.Pressed)
        {
            SoundManager.GetInstance.PlaySE("input_SE1");

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
            //キャンバスを最初は消しておく
            foreach (var image in PouseRogo)
                image.enabled = true;

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
                PouseRogo[1].GetComponent<Image>().color = Color.green;
                PouseRogo[2].GetComponent<Image>().color = Color.white;
                PouseRogo[3].GetComponent<Image>().color = Color.white;

                break;
            //続きから始める
            case 1:
                PouseRogo[2].GetComponent<Image>().color = Color.green;
                PouseRogo[1].GetComponent<Image>().color = Color.white;
                PouseRogo[3].GetComponent<Image>().color = Color.white;
                break;
            //セレクトシーンへ戻る
            case 2:
                PouseRogo[3].GetComponent<Image>().color = Color.green;
                PouseRogo[2].GetComponent<Image>().color = Color.white;
                PouseRogo[1].GetComponent<Image>().color = Color.white;
                break;
        }
    }
}


