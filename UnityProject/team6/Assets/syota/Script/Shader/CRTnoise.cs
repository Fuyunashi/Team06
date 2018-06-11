using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

/// <summary>
/// セレクト画面で選択する型
/// </summary>
public enum NextStage
{
    Tutrial1,
    Tutrial2,
    Stage1,
    Stage2,
    Stage3,
    Stage4,
    Stage5,
    Stage6,
    Stage7,
    Stage8,
    Stage9,
    Stage10,
    Stage11,
    Stage12,
    Stage13,
    Stage14,
    Stage15,
    Stage16,
    Stage17,
    Stage18,
    None,
}
[ExecuteInEditMode()]
public class CRTnoise : MonoBehaviour
{
    //演出用のカメラ
    public enum CameraName
    {
        TitleCamera,
        TitleRoomCamera,
        PlayCamera,
        None,
    }
    //ノイズをかけるマテリル（今回はカメラをRenderTextureを使う）
    [SerializeField]
    Material material;

    /* ノイズを調整する変数いろいろ */
    [SerializeField]
    [Range(0, 1)]
    float noiseX;
    public float NoiseX { get { return noiseX; } set { noiseX = value; } }

    [SerializeField]
    [Range(0, 1)]
    float rgbNoise;
    public float RGBNoise { get { return rgbNoise; } set { rgbNoise = value; } }

    [SerializeField]
    [Range(0, 1)]
    float sinNoiseScale;
    public float SinNoiseScale { get { return sinNoiseScale; } set { sinNoiseScale = value; } }

    [SerializeField]
    [Range(0, 10)]
    float sinNoiseWidth;
    public float SinNoiseWidth { get { return sinNoiseWidth; } set { sinNoiseWidth = value; } }

    [SerializeField]
    float sinNoiseOffset;
    public float SinNoiseOffset { get { return sinNoiseOffset; } set { sinNoiseOffset = value; } }

    [SerializeField]
    Vector2 offset;
    public Vector2 Offset { get { return offset; } set { offset = value; } }

    [SerializeField]
    [Range(0, 2)]
    float scanLineTail = 1.5f;
    public float ScanLineTail { get { return scanLineTail; } set { scanLineTail = value; } }

    [SerializeField]
    [Range(-10, 10)]
    float scanLineSpeed = 10;
    public float ScanLineSpeed { get { return scanLineSpeed; } set { scanLineSpeed = value; } }

    [SerializeField]
    [Range(0, 1)]
    float surveillanceCameraOn;
    public float SurveillanceCamera { get { return surveillanceCameraOn; } set { surveillanceCameraOn = value; } }


    //ノイズをかけるかどうか判断するフラグ(public にして外側でも変更可能にする：：むしろこのクラスでは指定しない)
    public bool CRTFlag { get; set; }

    //自身がついているカメラの種類
    [SerializeField, Tooltip("ステージ選択か否か")]
    public bool selectCamera;

    CameraName cameraName;
    NextStage nextStage;

    float NoiseTime;

    GameObject obj;
    StageInstructs stageInstructs;
    void Start()
    {
        //セレクト用のカメラと演出用のカメラをそれぞれ初期化
        cameraName = CameraName.None;
        nextStage = NextStage.None;

        //次のステージが何を選ばれているかを取得
        //ただ、タイトル画面ではステージ選択を行うスクリプトは必要ないので振り分け
        string name = SceneManager.GetActiveScene().name;
        if (selectCamera && transform.tag != CameraName.PlayCamera.ToString())
        {
            //Debug.Log("はいるなよ：" + selectCamera);
            obj = GameObject.Find("StageConfiguration");
            stageInstructs = obj.GetComponent<StageInstructs>();
        }
        //初期化ではShaderをかけない
        CRTFlag = false;
        //全部の値を初期化
        Donothing();

        //演出用のかめらの場合
        if (!selectCamera)
        {
            foreach (var camera_name in Enum.GetValues(typeof(CameraName)))
            {
                //自身のカメラに付けたのと同じ名前の列挙型を取得する
                if (transform.tag == camera_name.ToString())
                {
                    cameraName = (CameraName)camera_name;
                    if (transform.tag == CameraName.TitleRoomCamera.ToString())
                        CRTFlag = true;
                }
            }
        }
        //ステージ選択の場合
        if (selectCamera)
        {
            foreach (var next_stage in Enum.GetValues(typeof(NextStage)))
            {
                //上と同じく自身のカメラに付けたタグと同じものを取得する
                //ステージを選ぶ際と同じ型の列挙型を指定している
                if (transform.tag == next_stage.ToString())
                {
                    //Debug.Log("これが俺がタグの名前だ：" + nextStage);
                    nextStage = (NextStage)next_stage;

                }
            }
        }
        //タイトルでノイズを調整するためのタイム
        NoiseTime = 0;
    }
    void Update()
    {

        //演出用のカメラはフラグを用意して指示をもらう
        if (CRTFlag)
        {
            //Debug.Log("ノイズを起こすぞ：" + NoiseTime);
            switch (cameraName)
            {
                //タイトルシーンで使う
                case CameraName.TitleCamera: TitleCameraNoise(); break;
                //セレクトシーンのタイトル画面を写すTV
                case CameraName.TitleRoomCamera: NonSelectCamera(); break;
                //タイトルシーンで使う
                case CameraName.PlayCamera: TitleCameraNoise(); break;
            }
        }
        //nextStageがNoneの場合演出用のカメラなので下の処理は行わなくて良い
        if (nextStage == NextStage.None) return;

        //Debug.Log("次のシーンはこれだ！" + stageInstructs.CurrentStage);
        //自身の列挙型の型が選んでいるステージと同じであるならフラグを消す
        if (nextStage == stageInstructs.CurrentStage)
        {
            CRTFlag = false;
        }
        else
        {
            //選ばれていないステージの場合ノイズの、Shaderをかけてあげる
            CRTFlag = true;
            //ノイズ値はランダム指定
            NonSelectCamera();
        }

        //Debug.Log("風ラグ：" + CRTFlag);
    }
    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        material.SetFloat("_NoiseX", noiseX);
        material.SetFloat("_RGBNoise", rgbNoise);
        material.SetFloat("_SinNoiseScale", sinNoiseScale);
        material.SetFloat("_SinNoiseWidth", sinNoiseWidth);
        material.SetFloat("_SinNoiseOffset", sinNoiseOffset);
        material.SetFloat("_ScanLineSpeed", scanLineSpeed);
        material.SetFloat("_ScanLineTail", scanLineTail);
        material.SetVector("_Offset", offset);
        material.SetFloat("_FragFloat", surveillanceCameraOn);

        //フラグがONになっている型のみノイズをかける
        if (CRTFlag)
        {
            Graphics.Blit(src, dest, material);
            //Debug.Log("シーンはこれ：" + cameraName);
        }
        //それ以外は普通に描画しろ
        else
            Graphics.Blit(src, dest);
    }
    /// <summary>
    ///    タイトルでシーン移行する際に使用する関数(Noise値を調整)
    /// </summary>
    private void TitleCameraNoise()
    {
        if (NoiseTime % 10 == 0)
        {
            if (NoiseTime > 70)
            {
                ScanLineTail = 0f;
                CRTFlag = false;
                return;
            }
            NoiseX = UnityEngine.Random.Range(0.0f, 1.0f);
            NoiseTime += 10;
            if (NoiseTime > 40)
                ScanLineTail = UnityEngine.Random.Range(.0f, 1.0f);
        }
        NoiseTime++;
    }
    /// <summary>
    /// 選ばれていないステージ画面にランダムでノイズをかける
    /// </summary>
    private void NonSelectCamera()
    {
        //Debug.Log("シーンはこれ：" + cameraName);
        surveillanceCameraOn = 1;
        if (NoiseTime % 30 == 0)
        {
            NoiseX = UnityEngine.Random.Range(0.0f, 0.5f);
            NoiseTime = 0;
        }
        else if (NoiseTime < 10)
        {
            ScanLineTail = UnityEngine.Random.Range(.0f, 1.0f);
            NoiseX = 0;
        }
        if (NoiseTime % 20 == 0)
        {
            ScanLineTail = 2;
            sinNoiseScale = UnityEngine.Random.Range(.0f, 0.5f);
        }
        //NoiseTime += Time.deltaTime;
        ScanLineSpeed = 10;
    }

    /// <summary>
    /// ノイズをかけないためすべての値を初期化
    /// </summary>
    private void Donothing()
    {
        surveillanceCameraOn = 0;
        NoiseX = 0;
        ScanLineTail = 2;
        sinNoiseScale = 0;
        RGBNoise = 0;
        SinNoiseWidth = 0;
        SinNoiseOffset = 0;
        Offset = Vector2.zero;
        ScanLineSpeed = 0;
        SurveillanceCamera = 0;
    }
}
