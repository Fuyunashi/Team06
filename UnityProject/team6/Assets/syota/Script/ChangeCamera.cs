using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


public class ChangeCamera : MonoBehaviour
{
    [SerializeField]
    public CinemachineVirtualCamera[] v_camrea;

    GameObject obj_stageInstructs;
    StageInstructs stageInstructs;

    void Start()
    {
        obj_stageInstructs = GameObject.Find("StageConfiguration");
        stageInstructs = obj_stageInstructs.GetComponent<StageInstructs>();

        for (int i = 1; i < v_camrea.Length; i++)
        {
            v_camrea[i].Priority = 9;
        }
        v_camrea[0].Priority = 11;
    }

    // Update is called once per frame
    void Update()
    {
        if ((int)stageInstructs.CurrentStage >= 0 && (int)stageInstructs.CurrentStage <= 1)
        {
            v_camrea[1].Priority = 12;
            v_camrea[2].Priority = 10;
            v_camrea[3].Priority = 10;
            v_camrea[4].Priority = 10;

        }
        if ((int)stageInstructs.CurrentStage >= 2 && (int)stageInstructs.CurrentStage <= 7)
        {
            v_camrea[2].Priority = 12;

            v_camrea[1].Priority = 10;
            v_camrea[3].Priority = 10;
            v_camrea[4].Priority = 10;
        }
        if ((int)stageInstructs.CurrentStage >= 8 && (int)stageInstructs.CurrentStage <= 13)
        {
            v_camrea[3].Priority = 12;

            v_camrea[1].Priority = 10;
            v_camrea[2].Priority = 10;
            v_camrea[4].Priority = 10;
        }
        if ((int)stageInstructs.CurrentStage >= 14 && (int)stageInstructs.CurrentStage <= 19)
        {
            v_camrea[4].Priority = 12;

            v_camrea[1].Priority = 10;
            v_camrea[2].Priority = 10;
            v_camrea[3].Priority = 10;
        }

    }
}
