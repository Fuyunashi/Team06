using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControll : MonoBehaviour {

    /// <summary>
    ///メインカメラを入れるプロパティ
    /// </summary>
    public GameObject MainCamerad { get; set; }
    /// <summary>
    /// サブカメラをいれるプロパティ
    /// </summary>
    public GameObject SubCamera { get; set; }

    GameObject obj;
    SceneControll sceneControll;
	void Start () {
        //シーンの情報を取得したいのでSceneControllをおいておく
        //sceneControll = obj.GetComponent<SceneCotroll>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
