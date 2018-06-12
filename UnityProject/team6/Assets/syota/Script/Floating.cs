using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class Floating : MonoBehaviour
{

    public float amplitude = 0.01f; // 振幅
    private int frameCnt = 0; // フレームカウント
    GameObject obj_stageInstructs;
    StageInstructs stageInstructs;

    private void Start()
    {
        obj_stageInstructs = GameObject.Find("StageConfiguration");
        stageInstructs = obj_stageInstructs.GetComponent<StageInstructs>();
    }
    void FixedUpdate()
    {
        if (transform.name == "EXITBoard (1)")
        {
            Debug.Log("haadfaldjfla" + stageInstructs.CurrentStage);
            if (stageInstructs.CurrentStage != NextStage.Exit) return;

            transform.position = new Vector3(transform.position.x, Mathf.PingPong(Time.time, 1) + 4, transform.position.z);
            return;
        }
        frameCnt += 1;
        if (1000 <= frameCnt)
        {
            frameCnt = 0;
        }
        if (0 == frameCnt % 2)
        {
            // 上下に振動させる（ふわふわを表現）
            float posYSin = Mathf.Sin(2f * Mathf.PI * (float)(frameCnt % 200) / (200.0f - 1.0f));
            iTween.MoveAdd(gameObject, new Vector3(0, amplitude * posYSin, 0), 0.0f);
        }
    }
}
