using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class StageCangeCamera : MonoBehaviour
{

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
        selectControll = GetComponent<SelectControll>();
    }

    // Update is called once per frame
    void Update()
    {
        if (selectControll.ChangeSceneFrag)
        {
            if (nextStage == stageInstructs.CurrentStage)
                v_camera.Priority = 20;
        }
    }
}
