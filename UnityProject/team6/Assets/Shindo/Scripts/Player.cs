using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Animator))]
public class Player : MonoBehaviour
{
    private Animator animaotor_;
    //移動方向
    [SerializeField]
    private Vector3 velocity_;
    //移動速度
    [SerializeField , Tooltip("移動速度")]
    private float moveSpeed_ = 5.0f;
    //レイを飛ばす体の位置
    [SerializeField]
    private Transform charaRay_;
    //レイの距離
    [SerializeField]
    private float charaRayRange_ = 0.2f;
    //例が地面に達しているかどうか
    private bool isGround_;
    //入力値
    private Vector3 input_;
    //ジャンプの強さ
    [SerializeField, Tooltip("ジャンプの強さ")]
    private float jumpPower_ = 5f;
    //rigidbody
    private Rigidbody rigid_;
    private bool isGroundCollider_ = false;

    //段差を上るためのレイを飛ばす位置
    [SerializeField]
    private Transform stepRay_;
    //例を飛ばす距離
    [SerializeField]
    private float stepDistance_ = 0.5f;
    //登れる段差
    [SerializeField]
    private float stepOffset_ = 0.3f;
    //登れる角度
    [SerializeField]
    private float slopeLimit_ = 65f;
    //登れる段差の位置から飛ばすレイの距離
    [SerializeField]
    private float slopeDistance_ = 1f;
    //ヒットした情報を入れる場所
    private RaycastHit stepHit_;

    //Xinput関連
    private bool playerInputSet_ = false;
    private PlayerIndex playerIndex_;
    private GamePadState padState_;
    private GamePadState prevState_;
    
    // Use this for initialization
    void Start()
    {
        animaotor_ = GetComponent<Animator>();
        velocity_ = Vector3.zero;
        isGround_ = false;
        rigid_ = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {
        //Xinput関連
        if (!playerInputSet_ || !prevState_.IsConnected)
        {
            playerIndex_ = (PlayerIndex)0;
            playerInputSet_ = true;
        }
        prevState_ = padState_;
        padState_ = GamePad.GetState(playerIndex_);

        //キャラクターが設置していないときはレイを飛ばして確認
        if (!isGroundCollider_)
        {
            if(Physics.Linecast(charaRay_.position, (charaRay_.position - transform.up * charaRayRange_)))
            {
                isGround_ = true;
                rigid_.useGravity = true;
            }
            else
            {
                isGround_ = false;
                rigid_.useGravity = false;
            }
            Debug.DrawLine(charaRay_.position, (charaRay_.position - transform.up * charaRayRange_), Color.red);
        }

        //キャラクターコライダが接地、またはレイが地面に到着している場合
        if (isGroundCollider_ || isGround_)
        {
            velocity_ = Vector3.zero;

            //地面に設置しているときは初期化
            if (isGroundCollider_)
            {
                //着地していたらアニメーションパターンと二段階ジャンプフラグをfalse
                animaotor_.SetBool("Jump", false);
                rigid_.useGravity = true;
            }
            else
            {
                //レイを飛ばして接地確認の場合は重力だけは動かしておく、前後左右は初期化
                velocity_ = new Vector3(0f, velocity_.y, 0f);
            }


            var cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
            Vector3 direction = cameraForward * Input.GetAxis("Vertical") + Camera.main.transform.right * Input.GetAxis("Horizontal");
            //input_ = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")).normalized;

            //方向キーの入力量を計測
            if (direction.magnitude > 0.01f)
            {
                animaotor_.SetFloat("Speed", direction.magnitude);

                transform.LookAt(transform.position + direction);

                //登れる段差を表示
                Debug.DrawLine(transform.position + new Vector3(0f, stepOffset_, 0f), transform.position + new Vector3(0f, stepOffset_, 0f) + transform.forward * slopeDistance_, Color.green);

                //ステップ用のレイが地面に接触しているかどうか
                if(Physics.Linecast(stepRay_.position, stepRay_.position + stepRay_.forward * stepDistance_, out stepHit_, LayerMask.GetMask("Field", "Block")))
                {
                    //進行方向の地面の角度が指定以下、または登れる段差より下だった場合の移動処理
                    if (Vector3.Angle(transform.up, stepHit_.normal) <= slopeLimit_ || (Vector3.Angle(transform.up, stepHit_.normal) > slopeLimit_) && !Physics.Linecast(transform.position + new Vector3(0f, stepOffset_, 0f), transform.position + new Vector3(0f, stepOffset_, 0f) + transform.forward * slopeDistance_, LayerMask.GetMask("Field", "Block")))
                    {
                        velocity_ = new Vector3(0f, ((Quaternion.FromToRotation(Vector3.up, stepHit_.normal) * transform.forward) * moveSpeed_).y, 0f) + transform.forward * moveSpeed_;
                        Debug.Log(Vector3.Angle(transform.up, stepHit_.normal));
                    }
                    else
                    {
                        velocity_ += direction * moveSpeed_;
                    }

                    Debug.Log(Vector3.Angle(Vector3.up, stepHit_.normal));
                }
                else
                {
                    //ステップ用のレイが地面に接触していなければ
                    velocity_ = transform.forward * moveSpeed_ + new Vector3(0f, velocity_.y, 0f);
                }
                
            }
            else
            {
                //入力量が少ない場合動かない
                animaotor_.SetFloat("Speed", 0f);
            }

            //ジャンプ
            if (Input.GetKeyDown(KeyCode.Space) || (prevState_.Buttons.A == ButtonState.Released && padState_.Buttons.A == ButtonState.Pressed))
            {
                animaotor_.SetBool("Jump", true);
                velocity_.y += jumpPower_;
                rigid_.useGravity = false;
            }
        }

        if(!isGroundCollider_ && !isGround_)
        {
            velocity_.y += Physics.gravity.y * Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        //キャラクターを移動させる処理
        rigid_.MovePosition(transform.position + velocity_ * Time.deltaTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.DrawLine(charaRay_.position, charaRay_.position + Vector3.down, Color.blue);

        //ほかのコライダと接触しているときは下向きに例を飛ばし、LayerMaskに当たった時だけ接地とする
        if (Physics.Linecast(charaRay_.position, charaRay_.position + Vector3.down, LayerMask.GetMask("Field", "Block")))
        {
            isGroundCollider_ = true;
        }
        else
        {
            isGroundCollider_ = false;
        }
    }

    //接触していなければ中に浮いている状態
    void OnCollisionExit(Collision collision)
    {
        isGroundCollider_ = false;
    }

}