using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using XInputDotNetPure;

public class SelectControll : MonoBehaviour
{
    //Xinput関連
    private bool playerInputSet_ = false;
    private PlayerIndex playerIndex_;
    private GamePadState padState_;
    private GamePadState prevState_;


    /* シーン遷移の際に絶対に必要変数 */
    GameObject MainCam;
    GameObject obj_PerformanceCamera;
    GameObject obj_scene;
    SceneControll sceneControll;
    GameObject obj_cameraInformation;
    CameraInformation cameraInformation;
    /* シーン遷移の際に絶対に必要変数 */

    //-------//
    GameObject sceneChangeIcon;

    GameObject obj_stageInstructs;
    StageInstructs stageInstructs;
    //SceneName name;
    public bool ChangeSceneFrag;
   
    void Start()
    {
        //シーン情報を取得
        obj_PerformanceCamera = GameObject.Find("PerformanceCamera");
        obj_scene = GameObject.Find("SceneController");
        sceneControll = obj_scene.GetComponent<SceneControll>();
        obj_stageInstructs = GameObject.Find("StageConfiguration");
        stageInstructs = obj_stageInstructs.GetComponent<StageInstructs>();
        obj_cameraInformation = GameObject.Find("CameraInformation");
        cameraInformation = obj_cameraInformation.GetComponent<CameraInformation>();

        //演出用のカメラの情報を一つ前のシーンの状態と同じにする
        obj_PerformanceCamera.transform.position = cameraInformation.CameraPos;
        obj_PerformanceCamera.transform.rotation = cameraInformation.CameraRota;


    }

    void Update()
    {
        //Xinput関連
        if (!playerInputSet_ || !prevState_.IsConnected)
        {
            playerIndex_ = (PlayerIndex)0;
            playerInputSet_ = true;
        }
        prevState_ = padState_;
        padState_ = GamePad.GetState(playerIndex_);

        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log("次のシーンは：" + stageInstructs.CurrentStage);
        }
        if (prevState_.Buttons.B == ButtonState.Released && padState_.Buttons.B == ButtonState.Pressed)
        {
            //Debug.Log("ステージ移動する！！");
            NextSceneToDecide();
        }
    }
    /// <summary>
    /// 次に行くシーンを判断する
    /// </summary>
    private void NextSceneToDecide()
    {
        if (stageInstructs.CurrentStage.ToString().Substring(0, 5) == "Stage")
        {
            sceneControll.NextScene = SceneName.PlayScene;
            sceneControll.AddToScene.Add(stageInstructs.CurrentStage.ToString() + AddToScene.ChildScene);
            sceneControll.CurrentStage = stageInstructs.CurrentStage;
        }
        else if (stageInstructs.CurrentStage.ToString().Substring(0, 7) == "Tutrial")
        {
            sceneControll.NextScene = SceneName.TutorialScene;
            sceneControll.AddToScene.Add(stageInstructs.CurrentStage.ToString() + AddToScene.ChildScene);
            sceneControll.CurrentStage = stageInstructs.CurrentStage;
        }
    } 
}

