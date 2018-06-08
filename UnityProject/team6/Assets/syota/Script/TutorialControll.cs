using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class TutorialControll : MonoBehaviour {

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
    GameObject portalPosObj;

    GameObject obj_sceneControll;
    SceneControll sceneControll;
    GameObject obj_portal;
    DistortPortal distortPortal;

    PouseSelect pouseSelect;
    int pouseCount = 3;
    int pouseSelectIndex = 0;
    bool changeSceneFrag;

    void Start()
    {
        obj_sceneControll = GameObject.Find("SceneController");
        sceneControll = obj_sceneControll.GetComponent<SceneControll>();
        obj_portal = GameObject.Find("MainCamera");
        distortPortal = obj_portal.GetComponent<DistortPortal>();

        changeSceneFrag = false;
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
            if (prevState_.Buttons.B == ButtonState.Released && padState_.Buttons.B == ButtonState.Pressed)
            {
                distortPortal.portalPos = portalPosObj.transform.position;
                sceneControll.AddToScene.Add((sceneControll.CurrentStage + 1).ToString() + AddToScene.ChildScene);
                distortPortal.PortalFlag = true;
                changeSceneFrag = true;
            }
        }

        //ポウズ中の処理
        if (sceneControll.PuseFrag)
        {
            Debug.Log("ポウズ");
            Debug.Log("ポウズ中の処理は：" + (PouseSelect)pouseSelectIndex);
            PouseOperation();
            if (prevState_.Buttons.B == ButtonState.Released && padState_.Buttons.B == ButtonState.Pressed)
            {
                switch (pouseSelectIndex)
                {
                    //最初からやり直す
                    case 0:
                        sceneControll.NextScene = SceneName.TutorialCurrentScene;
                        sceneControll.AddToScene.Add(sceneControll.CurrentStage.ToString() + AddToScene.ChildScene);
                        break;
                    //続きから始める
                    case 1:
                        //ポウズを戻す
                        Time.timeScale = 1;
                        sceneControll.PuseFrag = false;
                        break;
                    //セレクトシーンへ戻る
                    case 2:
                        Time.timeScale = 1;
                        sceneControll.PuseFrag = false;
                        sceneControll.NextScene = SceneName.SelectScene;
                        sceneControll.AddToScene.Add(sceneControll.CurrentStage.ToString() + AddToScene.ChildScene);
                        break;
                }
            }

        }
        if (distortPortal.portalTime <= 0 && changeSceneFrag)
        {
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
            Debug.Log("ポウズ");
            Time.timeScale = 0;
            sceneControll.PuseFrag = true;
            return;
        }
    }
}

