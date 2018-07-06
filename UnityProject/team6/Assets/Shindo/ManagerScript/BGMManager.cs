using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMManager : MonoBehaviour {

   
    SceneControll scene_;


	// Use this for initialization
	void Start () {
        scene_ =  GameObject.Find("SceneController").GetComponent<SceneControll>();
	}
	
	// Update is called once per frame
	void Update () {

        
        //タイトル
        if(scene_.CurrentScene == SceneName.TitleScene)
        {
           // Debug.Log(scene_.CurrentScene + "流れてます");
            SoundManager.GetInstance.PlayBGM("6815");
        }
        
        //ステージ選択
        if(scene_.CurrentScene == SceneName.SelectScene)
        {
            //Debug.Log(scene_.CurrentScene + "流れてます");
            SoundManager.GetInstance.PlayBGM("6815");
        }

        //1~5
        if((scene_.CurrentStage == NextStage.Tutrial1)
            || (scene_.CurrentStage == NextStage.Tutrial2))
        {
            //Debug.Log(scene_.CurrentStage + "流れてます");
            SoundManager.GetInstance.PlayBGM("8811");
        }

        //6~10
        if ((scene_.CurrentStage == NextStage.Stage1)
            || (scene_.CurrentStage == NextStage.Stage2)
            || (scene_.CurrentStage == NextStage.Stage3))
        {
            //Debug.Log(scene_.CurrentStage + "流れてます");
            SoundManager.GetInstance.PlayBGM("8671");
        }

        //11~15
        if ((scene_.CurrentStage == NextStage.Stage4)
            || (scene_.CurrentStage == NextStage.Stage5)
            || (scene_.CurrentStage == NextStage.Stage6))
        {
            //Debug.Log(scene_.CurrentStage + "流れてます");
            SoundManager.GetInstance.PlayBGM("8659");
        }

        //16~12
        if ((scene_.CurrentStage == NextStage.Stage7)
            || (scene_.CurrentStage == NextStage.Stage8))
        {
            //Debug.Log(scene_.CurrentStage + "流れてます");
            SoundManager.GetInstance.PlayBGM("8497");
        }
        
    }
}
