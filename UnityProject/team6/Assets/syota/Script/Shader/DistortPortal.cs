using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DistortPortal : MonoBehaviour
{
    enum PortalState
    {
        FirstSpread, //一回目途中まで広がる
        Shrink,      //少し縮む
        SecondSpread,//二回目最後まで広がる
        None,
        SubNone,
        LastNone,
    }

    //シェーダーを取得
    [SerializeField]
    Material material;

    //スクリーンに写っているものを取得
    [SerializeField]
    Texture text;

    //広がる半径を入れる
    [SerializeField]
    float radius;

    //広がる半径の初期化
    float currentPortalRadius = 0;

    /* クラス外で設定（あるいは教える）するプロパティ群 */
    //空間を歪める中心値
    public Vector3 portalPos { get; set; }

    //空間を歪めるフラグ
    public bool PortalFlag { get; set; }
    //広がる時間  
    public float portalTime { get; private set; }


    PortalState portalState;
    Camera camera;
    int first_time;
    int shrink_time;
    int second_time;

    bool flag;
    void Start()
    {
        //カメラを使うので用意する
        camera = Camera.main;
        //Debug.Log("カメラだぜ" + camera);
        //Shader用のテクスチャを入れる
        material.SetTexture("_SubTex", text);
        //プロパティの初期化
        portalPos = new Vector3(.0f, .0f, .0f);
        PortalFlag = false;
        //時間関係の設定
        portalTime = 4;

        //状態を初期化
        portalState = PortalState.FirstSpread;

        flag = true;
        //portalState = PortalState.SubNone;
    }


    void Update()
    {
        if (flag)
        {
            DOTween.KillAll();
            DOTween.To(() => 0, SetPortalRadius, currentPortalRadius, .0f).SetEase(Ease.InBack);
            flag = false;
        }
        //シーンからの合図で行動開始
        if (PortalFlag)
        {
            material.SetTexture("_SubTex", text);
            var uv = camera.WorldToViewportPoint(portalPos);
            //Debug.Log("半径" +  + "半径");
            material.SetVector("_Position", uv);
            material.SetFloat("_Aspect", Screen.height / (float)Screen.width);
            OpenPoral();
            portalTime -= Time.deltaTime;
        }
        //現在の半径を入れる

        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    Debug.Log("aa" + uv);
        //    OpenPoral();
        //}
        //else if (Input.GetKeyUp(KeyCode.A))
        //    ClosePortal();
    }
    void OpenPoral()
    {
        /*-------------------------------------
           DOTween.To 
            (
            () => Number,　     //何に
            (n) => Number = n,　//何を
            100,　              //どこまで(最終的な値)
            10.0f               //どれくらいの時間
            );
                 -------------------------------------*/
        //Debug.Log("状態：" + portalState);

        switch (portalState)
        {
            case PortalState.FirstSpread:
                DOTween.KillAll();
                DOTween.To(() => currentPortalRadius, SetPortalRadius, radius / 5, 1.5f).SetEase(Ease.OutBack);
                portalState = PortalState.None;
                break;
            case PortalState.Shrink:
                DOTween.KillAll();
                DOTween.To(() => radius / 5, SetPortalRadius, radius / 10, 1.0f).SetEase(Ease.InBack);
                portalState = PortalState.SubNone;
                break;
            case PortalState.SecondSpread:
                DOTween.KillAll();
                DOTween.To(() => currentPortalRadius, SetPortalRadius, radius, 3.0f).SetEase(Ease.OutBack);
                portalState = PortalState.LastNone;
                break;
        }
        if (portalTime < 2.5f && portalState == PortalState.None)
            portalState = PortalState.Shrink;
        if (portalState == PortalState.SubNone && portalTime < 1.5f)
            portalState = PortalState.SecondSpread;

    }
    public void SetPortalRadius(float radius)
    {
        currentPortalRadius = radius;
        material.SetFloat("_Radius", radius);
    }
    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(src, dest, material);
    }
}
