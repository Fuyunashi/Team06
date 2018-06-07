using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class Player : MonoBehaviour
{
    
    private Animator animaotor_;
    //移動方向
    [SerializeField]
    private Vector3 velocity_;
    //移動速度
    [SerializeField, Tooltip("移動速度")]
    private float moveSpeed_ = 5.0f;
    //地面に達しているかどうか
    private bool isGround_;
    [SerializeField, Tooltip("Rayを出す高さ")]
    private Transform charaRay;
    //ジャンプの強さ
    [SerializeField, Tooltip("ジャンプの強さ")]
    private float jumpPower_ = 5f;
    //rigidbody
    private Rigidbody rb_;

    //死ぬ秒数（60fps）
    [SerializeField, Tooltip("死ぬ秒数")]
    private float deathTime_ = 2.0f;
    //空中に浮いている時間
    private float deathTimer_ = 0.0f;
    //落ちた距離
    private float distance_;
    //落ちた地点
    private float fallPosition_;
    //死ぬ高さ
    [SerializeField, Tooltip("死ぬ高さ")]
    private float deadDistance_ = 5f;

    //足音を鳴らす間隔
    [SerializeField, Tooltip("足音を鳴らす間隔")]
    private const float interval_sec_ = 0.4f;
    //最後になった時間
    private DateTime lastStepTime_;
    //
    [SerializeField]
    private int randomRange_ = 3;

    //Xinput関連
    private bool playerInputSet_ = false;
    private PlayerIndex playerIndex_;
    private GamePadState padState_;
    private GamePadState prevState_;
    
    // Use this for initialization
    void Start()
    {
        //animaotor_ = GetComponent<Animator>();
        velocity_ = Vector3.zero;
        isGround_ = true;
        rb_ = GetComponent<Rigidbody>();

        
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

        RaycastHit hit;
        if (Physics.SphereCast(charaRay.transform.position, 0.2f, -transform.up, out hit, 0.2f, LayerMask.GetMask("Wall")))
        {
            Debug.Log("当たってます");
            isGround_ = true;
        }
        else
        {
            Debug.Log("落ちてます");
            isGround_ = false;
        }

        if (isGround_)
        {
            velocity_ = Vector3.zero;

            //地面に設置しているときは初期化
            if (isGround_)
            {
                rb_.useGravity = true;
            }
            else
            {
                velocity_ = new Vector3(0f, velocity_.y, 0f);
            }

            //進行方向を向く
            var cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
            Vector3 direction = cameraForward * Input.GetAxis("Vertical") + Camera.main.transform.right * Input.GetAxis("Horizontal");

            //方向キーの入力量を計測
            if (direction.magnitude > 0.01f)
            {
                transform.LookAt(transform.position + direction);

                Debug.Log("移動してます");

                velocity_ += direction * moveSpeed_;


                int num_ = UnityEngine.Random.Range(0, randomRange_);
                //足音
                if ((DateTime.Now - lastStepTime_).TotalSeconds < interval_sec_)
                {
                    return;
                }
                else
                {
                    switch (num_)
                    {
                        case 0:
                            Debug.Log("1");
                            SoundManager.GetInstance.PlaySE("Walk_SE_1");
                            break;
                        case 1:
                            Debug.Log("2");
                            SoundManager.GetInstance.PlaySE("Walk_SE_2");
                            break;
                        case 2:
                            Debug.Log("3");
                            SoundManager.GetInstance.PlaySE("Walk_SE_3");
                            break;
                    }
                    lastStepTime_ = DateTime.Now;
                }
            }
        }

        //ジャンプ
        if (isGround_ && (Input.GetKeyDown(KeyCode.Space) || (prevState_.Buttons.A == ButtonState.Released && padState_.Buttons.A == ButtonState.Pressed)))
        {
            Debug.Log("飛んでます");
            isGround_ = false;
            velocity_.y += jumpPower_;
            rb_.useGravity = false;
            SoundManager.GetInstance.PlaySE("Janp_SE");
        }

        if (!isGround_)
        {
            velocity_.y += Physics.gravity.y * Time.deltaTime;
        }

        //死亡処理
        if (!isGround_)
        {
            Debug.Log("あと" + deathTimer_ + "で死にます");
            deathTimer_ += Time.deltaTime;
            //高さで死ぬ処理
            fallPosition_ = Mathf.Max(fallPosition_, transform.position.y);

            //落下したら死ぬ
            if (Physics.Linecast(charaRay.position + new Vector3(0f, 0.4f, 0f), Vector3.down * deadDistance_, LayerMask.GetMask("Wall")))
            {
                distance_ = fallPosition_ - transform.position.y;
                Debug.Log(distance_);
                if (distance_ >= deadDistance_)
                {
                    Debug.Log("死にました");
                    SoundManager.GetInstance.PlaySE("FallDead_SE");
                    Destroy(gameObject);
                }
            }
            //時間がたったら死ぬ
            if (deathTimer_ >= deathTime_)
            {
                //死亡時の処理
                Debug.Log("死にました");
                SoundManager.GetInstance.PlaySE("FallDead_SE");
                Destroy(gameObject);
            }
        }
        else
        {
            deathTimer_ = 0.0f;
        }

        //キー入力
        if (Input.GetMouseButtonDown(1) || (padState_.Triggers.Left >= 0.7f))
        {
            Debug.Log("オン");
            GameObject.Find("Reticle").GetComponent<Image>().enabled = true;

        }
        if (Input.GetMouseButtonUp(1) || (padState_.Triggers.Left <= 0.7f))
        {
            Debug.Log("オフ");
        }
    }

    void FixedUpdate()
    {
         //キャラクターを移動させる処理
         rb_.MovePosition(transform.position + velocity_ * Time.deltaTime);
         
    }

    void OnDrawGizmos()
    {
        RaycastHit hit;
        
        var radius = transform.lossyScale.x * 0.2f;

        var isHit = Physics.SphereCast(charaRay.position, radius, -transform.up * 10, out hit, 0.4f);
        if (isHit)
        {
            Gizmos.DrawRay(transform.position, -transform.up * hit.distance);
            Gizmos.DrawWireSphere(transform.position + -transform.up * (hit.distance), radius);
        }
        else
        {
            Gizmos.DrawRay(transform.position, -transform.up * 100);
        }
    }

    
}