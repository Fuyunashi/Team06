using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChildSceneCameraControll : MonoBehaviour
{
    [SerializeField]
    GameObject MainCam;
    [SerializeField]
    GameObject stageObj;

    GameObject scene;
    SceneControll sceneControll;
    void Start()
    {
        //アクティブ出ないシーンはカメラを着る
        MainCam.SetActive(false);
        scene = GameObject.Find("SceneController");
        sceneControll = scene.GetComponent<SceneControll>();
        
        //現在のステージと自身が存在するシーンが異なればレイヤー設定
        if (GetSceneContainObject(gameObject) == sceneControll.CurrentStage.ToString() + "ChildScene")
        {
            stageObj.gameObject.layer = LayerMask.NameToLayer("Production");
        }
        Debug.Log("今俺がいるシーンは：" + GetSceneContainObject(gameObject));
    }

    void Update()
    {

    }
    private string GetSceneContainObject(GameObject i_object)
    {
        GameObject rootObject = i_object.transform.root.gameObject;

        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            foreach (var sceneRootobject in scene.GetRootGameObjects())
            {
                if (sceneRootobject == rootObject)
                    return scene.name;
            }
        }
        return null;
    }
}
