using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class Floating : MonoBehaviour {

    public float amplitude = 0.01f; // 振幅
    private int frameCnt = 0; // フレームカウント
    
    void FixedUpdate()
    {
        frameCnt += 1;
        if (15000 <= frameCnt)
        {
            frameCnt = 0;
        }
        if (0 == frameCnt % 2)
        {
            // 上下に振動させる（ふわふわを表現）
            float posYSin = Mathf.Sin(2f * Mathf.PI * (float)(frameCnt % 300) / (300.0f - 1.0f));
          iTween.MoveAdd(gameObject, new Vector3(0, amplitude * posYSin, 0), 0.0f);
        }
    }
}
