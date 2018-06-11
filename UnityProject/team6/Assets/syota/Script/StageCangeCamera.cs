using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class StageCangeCamera : MonoBehaviour
{
    [SerializeField]
    CinemachineVirtualCamera v_camera;
    [SerializeField]
    public NextStage nextStage;

    //必要なスクリプト変数
    GameObject obj_selectControll;
    SelectControll selectControll;
    GameObject obj_stageInstructs;
    StageInstructs stageInstructs;

    void Start()
    {
        //スクリプトの確保
        obj_stageInstructs = GameObject.Find("StageConfiguration");
        stageInstructs = obj_stageInstructs.GetComponent<StageInstructs>();
        obj_selectControll = GameObject.Find("SelectControll");
        selectControll = obj_selectControll.GetComponent<SelectControll>();
        v_camera.Priority = 5;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("セレクトコントロールはあるよ" + selectControll.ChangeSceneFrag);
        if (selectControll.ChangeSceneFrag)
        {
            if (nextStage == stageInstructs.CurrentStage)
                v_camera.Priority = 20;
        }
    }
}
