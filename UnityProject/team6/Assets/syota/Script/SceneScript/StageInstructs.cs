using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StageInstructs : MonoBehaviour
{
    //private struct ObjectANDAxis
    //{
    //    [SerializeField, Header("回転軸")]
    //    public AxisOfRotation m_axis;
    //    [SerializeField, Header("回転方向")]
    //    public RotateDire m_rote_dir;
    //    [SerializeField]
    //    public GameObject m_rotate_obj;
    //}
    [System.Serializable]
    public struct MovingStage
    {
        [SerializeField]
        public NextStage currentStage;

        [SerializeField]
        public NextStage up_tv;

        [SerializeField]
        public NextStage right_tv;

        [SerializeField]
        public NextStage left_tv;

        [SerializeField]
        public NextStage dwon_tv;
    }
    [SerializeField, Header("全セレクト数")]
    public MovingStage[] movingStage;

    public NextStage CurrentStage;

    void Start()
    {
       // CurrentStage = NextStage.Tutrial1;
    }

    void Update()
    {
        //Debug.Log("次のシーン：" + CurrentStage);
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            foreach (var stage in movingStage)
            {
                if (stage.currentStage != CurrentStage || stage.up_tv == NextStage.None) continue;
                CurrentStage = stage.up_tv;
                return;
            }
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            foreach (var stage in movingStage)
            {
                //Debug.Log("おらがめあすだ：" + stage.currentStage + "みぎ：" + stage.right_tv);
                if (stage.currentStage != CurrentStage || stage.right_tv == NextStage.None) continue;
                CurrentStage = stage.right_tv;
                return;
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            foreach (var stage in movingStage)
            {
                if (stage.currentStage != CurrentStage || stage.left_tv == NextStage.None) continue;
                CurrentStage = stage.left_tv;
                return;
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            foreach (var stage in movingStage)
            {
                if (stage.currentStage != CurrentStage || stage.dwon_tv == NextStage.None) continue;
                CurrentStage = stage.dwon_tv;
                return;
            }
        }
    }
}
