using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectControll : MonoBehaviour
{
    /* シーン遷移の際に絶対に必要変数 */
    GameObject MainCam;
    GameObject SubCam;
    GameObject scene;
    SceneControll sceneControll;
    /* シーン遷移の際に絶対に必要変数 */

    //-------//
    GameObject sceneChangeIcon;

    GameObject obj;
    StageInstructs stageInstructs;
    //SceneName name;
    public bool ChangeSceneFrag;
    void Start()
    {
        //シーン情報を取得
        scene = GameObject.Find("SceneController");
        sceneControll = scene.GetComponent<SceneControll>();
        obj = GameObject.Find("StageConfiguration");
        stageInstructs = obj.GetComponent<StageInstructs>();

        AddChildScene();

        ChangeSceneFrag = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log("次のシーンは：" + stageInstructs.CurrentStage);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
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
        //stageInstructs.CurrentStage;
    }
    //タイトルの部屋をテレビに映すために部屋のオブジェクトだけ持っているSceneをよぶ
    private void AddChildScene()
    {
        sceneControll.AddToScene.Add(AddToScene.TitleRoom.ToString());
    }
}

