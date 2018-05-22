using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraChange : MonoBehaviour {

    //プレイヤー
    public GameObject FpsObj_;
    public GameObject TpsObj_;
    //カメラ
    public Camera FpsCamera_;
    public Camera TpsCamera_;

    private Player player_;

    public bool isFps_ { get; set; }
    
    GameObject obj, obj2;

    void Awake()
    {
        obj = FpsObj_;
        obj2 = TpsObj_;
        obj.SetActiveRecursively(false);
        obj2.SetActiveRecursively(false);
        isFps_ = false;
    }
    
	// Update is called once per frame
	void Update () {

        if (isFps_)
        {
            ShowFirstPersonView();
        }
        else
        {
            ShowThirdPersonView();
        }

	}

    public void ShowFirstPersonView()
    {
        //現在の座標を取得して受け渡す
        var curTpsCam = TpsCamera_.transform.position;
        var curTpsPos = TpsObj_.transform.position;

        FpsObj_.transform.position = curTpsPos;
        FpsCamera_.transform.position = curTpsCam;

        FpsObj_.SetActive(true);
        TpsObj_.SetActive(false);
    }

    public void ShowThirdPersonView()
    {
        //現在の座標を取得して受け渡す
        var curFpsCam = FpsCamera_.transform.position;
        var curFpsPos = FpsObj_.transform.position;

        TpsObj_.transform.position = curFpsPos;
        TpsCamera_.transform.position = curFpsCam;

        FpsObj_.SetActive(false);
        TpsObj_.SetActive(true);
    }
    
}
