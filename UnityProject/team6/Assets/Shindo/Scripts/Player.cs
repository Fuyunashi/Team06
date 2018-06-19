using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class Player : MonoBehaviour
{
    
    //移動速度
    [SerializeField, Tooltip("移動速度")]
    private float moveSpeed_ = 5.0f;
    [SerializeField, Tooltip("移動速度")]
    private float sideSpeed_ = 4.0f;
    //地面に達しているかどうか
    private bool isGround_;
    [SerializeField]
    private Transform charaRay;
    [SerializeField]
    private float rayRange_;
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
    [SerializeField]
    private float Gravity = 9.8f;

    //足音を鳴らす間隔
    [SerializeField, Tooltip("足音を鳴らす間隔")]
    private const float interval_sec_ = 0.4f;
    //最後になった時間
    private DateTime lastStepTime_;
    [SerializeField, Tooltip("足音の数")]
    private int randomRange_ = 3;

    private bool isJumping_;

    public Camera camera_;
    public bool isStop_ { get; set; }
    
    //シーン関連
    PlayControll playControll;
    GameObject obj_playerContoroll_;
    TutorialControll tutorialControll;
    GameObject obj_tutorialControll_;

    //Xinput関連
    private bool playerInputSet_ = false;
    private PlayerIndex playerIndex_;
    private GamePadState padState_;
    private GamePadState prevState_;

    void Awake()
    {
        GetComponent<Rigidbody>().freezeRotation = true;
        GetComponent<Rigidbody>().useGravity = false;
    }

    // Use this for initialization
    void Start()
    {
        
        if (SceneManager.GetActiveScene().name == SceneName.PlayScene.ToString())
        {
            obj_playerContoroll_ = GameObject.Find("PlayControll");
            playControll = obj_playerContoroll_.GetComponent<PlayControll>();
        }
        else if (SceneManager.GetActiveScene().name == SceneName.TutorialScene.ToString())
        {
            obj_tutorialControll_ = GameObject.Find("TutorialControll");
            tutorialControll = obj_tutorialControll_.GetComponent<TutorialControll>();
        }
        
        isStop_ = false;
        isJumping_ = false;
        lastStepTime_ = DateTime.Now;
        fallPosition_ = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        //Player停止
        if(isStop_ == true) { return; }

        
        isGround();
        isGoalFlag();

        //足音
        int num_ = UnityEngine.Random.Range(0, randomRange_);
        if ((DateTime.Now - lastStepTime_).TotalSeconds < interval_sec_)
        {
            return;
        }
        else
        {
            switch (num_)
            {
                case 0:
                    SoundManager.GetInstance.PlaySE("Walk_SE_1");
                    break;
                case 1:
                    SoundManager.GetInstance.PlaySE("Walk_SE_2");
                    break;
                case 2:
                    SoundManager.GetInstance.PlaySE("Walk_SE_3");
                    break;
            }
            lastStepTime_ = DateTime.Now;
        }

        //高さで死ぬ処理
        fallPosition_ = Mathf.Max(fallPosition_, transform.position.y);

        //死亡処理
        if (!isGround_)
        {
            deathTimer_ += Time.deltaTime;
        }

        if (!isGround_) {

            //Debug.Log("落下しました");
            distance_ = fallPosition_ - transform.position.y;

            if (distance_ >= deadDistance_)
            {
                SoundManager.GetInstance.PlaySE("FallDead_SE");

                if (SceneManager.GetActiveScene().name == SceneName.PlayScene.ToString())
                {
                    playControll.playerDeadFrag = true;
                }
                else if (SceneManager.GetActiveScene().name == SceneName.TutorialScene.ToString())
                {
                    tutorialControll.playerDeadFrag = true;
                }
                Debug.Log("死にました");
                Destroy(gameObject);
            }
        }
        
        //時間がたったら死ぬ
        if (deathTimer_ >= deathTime_)
        {
            //死亡時の処理
            SoundManager.GetInstance.PlaySE("FallDead_SE");
            if (SceneManager.GetActiveScene().name == SceneName.PlayScene.ToString())
            {
                playControll.playerDeadFrag = true;
            }
            else if (SceneManager.GetActiveScene().name == SceneName.TutorialScene.ToString())
            {
                tutorialControll.playerDeadFrag = true;
            }
            Destroy(gameObject);
        }
        else
        {
            deathTimer_ = 0.0f;

        }

        if (isJumping_ && isGround_)
        {
            SoundManager.GetInstance.PlaySE("Landing_SE");
            isJumping_ = false;
        }
        
    }

    void FixedUpdate()
    {

        //Xinput関連
        if (!playerInputSet_ || !prevState_.IsConnected)
        {
            playerIndex_ = (PlayerIndex)0;
            playerInputSet_ = true;
        }
        prevState_ = padState_;
        padState_ = GamePad.GetState(playerIndex_);

        //進行方向を向く
        //Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal") * sideSpeed_, 0, Input.GetAxis("Vertical") * moveSpeed_);
        Vector3 cameraForward = Vector3.Scale(camera_.transform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 targetVelocity = cameraForward * Input.GetAxis("Vertical") + camera_.transform.right * Input.GetAxis("Horizontal");
        targetVelocity = transform.TransformDirection(targetVelocity);

        Vector3 velocity = GetComponent<Rigidbody>().velocity;
        Vector3 velocityChange = (targetVelocity - velocity);
        velocityChange.y = 0;
        GetComponent<Rigidbody>().AddForce(velocityChange, ForceMode.VelocityChange);

        if (!isJumping_ && isGround_ && Input.GetButton("Jump") /*(prevState_.Buttons.A == ButtonState.Released && padState_.Buttons.A == ButtonState.Pressed)*/)
        {
            GetComponent<Rigidbody>().velocity = new Vector3(velocity.x, Mathf.Sqrt(2 * jumpPower_ * Gravity), velocity.z);
            isGround_ = false;
            isJumping_ = true;
        }

        GetComponent<Rigidbody>().AddForce(new Vector3(0, -Gravity * GetComponent<Rigidbody>().mass, 0));

    }

    void isGround()
    {
        RaycastHit hit;
        //if (Physics.Raycast(transform.position, -Vector3.up, rayRange_, LayerMask.GetMask("Wall", "Production")))
        if (Physics.SphereCast(charaRay.transform.position, 0.5f, -Vector3.up, out hit, rayRange_, LayerMask.GetMask("Wall", "Production")))
        {
            if(hit.collider.CompareTag("stage") || hit.collider.CompareTag("ChangeObject") || hit.collider.CompareTag("GravityObj"))
            {
                isGround_ = true;
            }
            else
            {
                isGround_ = false;
            }
        }
        Debug.Log("接地判定" + isGround_);
        //Debug.Log("レイの中身" + hit.collider.tag);
    }

    void isGoalFlag()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.forward, out hit, rayRange_))
        {

            if (hit.collider.CompareTag("GoleObject") || hit.collider.CompareTag("GoalObject"))
            {

                if (SceneManager.GetActiveScene().name == SceneName.PlayScene.ToString())
                {
                    playControll.stageClearFrag = true;
                }
                else if (SceneManager.GetActiveScene().name == SceneName.TutorialScene.ToString())

                {
                    tutorialControll.stageClearFrag = true;
                }

            }

        }
        
    }
    
}