using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class TPVCamera : MonoBehaviour
{
    //ターゲット
    public Transform target_ = null;
    //カメラの元の位置
    private Vector3 cameraPrePos_;
    private RaycastHit hit_;
    //カメラの移動スピード
    [SerializeField, Tooltip("カメラがめり込んだ時の移動速度")]
    private float cameraMoveSpeed_ = 3.0f;
    //プレイヤーとの距離
    private Vector3 offset_ = Vector3.zero;
    //プレイヤーとの距離
    [Tooltip("カメラとプレイヤーの距離")]
    public float distance_ = 2f;
    //カメラの高さ
    public float cameraHeight_ = 1.2f;
    //カメラを横にスライドさせる
    public float slideDistance_ = 10f;
    //回転の感度
    [Tooltip("カメラの回転速度")]
    public float rotationSpeed_ = 100f;
    //視点
    private bool isChange = false;
    
    // Use this for initialization
    void Start()
    {
        if (target_ == null)
        {
            Debug.LogError("ターゲットを設定してください");
            Application.Quit();
            return;
        }
        //初期位置を設定
        cameraPrePos_ = transform.localPosition;
        
    }

    void Update()
    {
        //マウスの右ボタンが押されていたら
        if (Input.GetMouseButtonDown(1))
        {
            isChange = true;
            Debug.Log("一人称");
        }
        else if (Input.GetMouseButtonUp(1))
        {
            isChange = false;
            Debug.Log("三人称");
        }

        if (isChange == true)
        {
            distance_ = 0.0f;
            cameraHeight_ = 1.6f;
        }
        else{
            distance_ = 2.0f;
            cameraHeight_ = 1.2f;
        }
        
    }

    void FixedUpdate()
    {

        //マウス操作
        var rotateX = Input.GetAxis("Mouse X") * Time.deltaTime * rotationSpeed_;
        var rotateY = Input.GetAxis("Mouse Y") * Time.deltaTime * rotationSpeed_;

        var lookAt = target_.position + Vector3.up * cameraHeight_;

        //回転
        transform.RotateAround(lookAt, Vector3.up, rotateX);
        //カメラがプレイヤーの真上や真下にあるときそれ以上回転させない
        if (transform.forward.y > .7f && -rotateY < 0)
        {
            rotateY = 0;
        }
        if (transform.forward.y < -.7f && -rotateY > 0)
        {
            rotateY = 0;
        }
        transform.RotateAround(lookAt, transform.right, -rotateY);

        //カメラが障害物と接触していたら障害物の場所に移動
        if(Physics.Linecast(target_.position + Vector3.up, transform.position, out hit_, LayerMask.GetMask("Wall")))
        {
            transform.position = Vector3.Lerp(transform.position, hit_.point, cameraMoveSpeed_ * Time.deltaTime);
        }
        //障害物と接触していなければ元のカメラ位置に移動
        else
        {
            //元の位置ではない時だけ元の位置に移動
            if(cameraPrePos_ != offset_)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, cameraPrePos_, cameraMoveSpeed_ * Time.deltaTime);
            }
        }
        Debug.DrawLine(target_.position + Vector3.up, transform.position, Color.red, 0f, false);

        //カメラとプレイヤーとの間の距離
        cameraPrePos_ = lookAt - transform.forward * distance_;
        
        //注視点の設定
        transform.LookAt(lookAt);

        //カメラを横にずらして中央を開ける
        transform.position = transform.position + transform.right * slideDistance_;

    }

}
