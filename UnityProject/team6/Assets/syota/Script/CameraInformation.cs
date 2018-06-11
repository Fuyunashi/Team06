using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraInformation : MonoBehaviour
{

    public Vector3 CameraPos { get; set; }

    public Quaternion CameraRota { get; set; }
    void Start()
    {
        //シーンをまたいで情報を共有するため
        DontDestroyOnLoad(this);
        //情報の初期化
        CameraPos = new Vector3(0, 0, 0);
        CameraRota = new Quaternion(0, 0, 0, 0);
    }    
}
