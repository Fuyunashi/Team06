using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class TPVCamera : MonoBehaviour {

    //ターゲット
    public Transform target_;
    //プレイヤーとの距離
    public float distance_ = 2f;
    //カメラの高さ
    public float cameraHeight_ = 1.2f;
    //カメラを横にスライドさせる
    public float slideDistance_ = 0f;
    //回転の感度
    public float rotationSpeed_ = 100f;
    //視点切り替え
    

	// Use this for initialization
	void Start () {
		if(target_ == null)
        {
            Debug.LogError("ターゲットを設定してください");
            Application.Quit();
        }
	}

    void Update()
    {
        //マウスの右ボタンが押されていたら
        if (Input.GetMouseButton(1))
        {
            Debug.Log("一人称");
            distance_ = .0f;
            
        }
        
    }

    void FixedUpdate()
    {
        var rotateX = Input.GetAxis("Mouse X") * Time.deltaTime * rotationSpeed_;
        var rotateY = Input.GetAxis("Mouse Y") * Time.deltaTime * rotationSpeed_;

        var lookAt = target_.position + Vector3.up * cameraHeight_;

        //回転
        transform.RotateAround(lookAt, Vector3.up, rotateX);
        //カメラがプレイヤーの真上や真下にあるときそれ以上回転させない
        if(transform.forward.y > .7f && -rotateY < 0)
        {
            rotateY = 0;
        }
        if(transform.forward.y < -.7f && -rotateY > 0)
        {
            rotateY = 0;
        }
        transform.RotateAround(lookAt, transform.right, -rotateY);

        //カメラとプレイヤーとの間の距離
        transform.position = lookAt - transform.forward * distance_;

        //注視点の設定
        transform.LookAt(lookAt);

        //カメラを横にずらして中央を開ける
        transform.position = transform.position + transform.right * slideDistance_;

    }
}
