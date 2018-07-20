using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using XInputDotNetPure;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class Player : MonoBehaviour
{

    //前後の速さ
    [SerializeField]
    private float forwardSpeed_;
    [SerializeField]
    private float sideSpeed_;
    //接地判定
    [SerializeField]
    private bool isGround_;
    [SerializeField]
    private Transform charaRay_;
    [SerializeField]
    private float rayRange_;
    [SerializeField]
    private float jumpPower_;

    private Rigidbody rb_;

    private float distance_;
    private Vector3 fallPos_;
    [SerializeField]
    private float deathDistance_;
    [SerializeField]
    private bool isDead_;
    [SerializeField]
    private float Gravity_;
    private float totalFallTime_;

    [SerializeField]
    private const float interval_sec_ = 0.4f;
    private DateTime lastStepTime_;
    [SerializeField]
    private int randomRange_ = 3;
    private bool isJump_;
    private float LerpTimer_;
    private bool inAir = false;

    public Camera camera_;
    private bool isMaxHeight_;
    public bool isStop_ { get; set; }

    PlayControll playControll_;
    GameObject obj_playControll_;
    TutorialControll tutorialControll_;
    GameObject obj_tutorialControll_;

    //Xinput関連
    private bool playerInputSet_ = false;
    private PlayerIndex playerIndex_;
    private GamePadState padState_;
    private GamePadState prevState_;

    void Awake()
    {
        rb_ = GetComponent<Rigidbody>();
    }

    // Use this for initialization
    void Start()
    {
        rb_.freezeRotation = true;
        rb_.useGravity = true;

        //プレイヤーの認識
        if (SceneManager.GetActiveScene().name == SceneName.PlayScene.ToString())
        {
            obj_playControll_ = GameObject.Find("PlayControll");
            playControll_ = obj_playControll_.GetComponent<PlayControll>();

        }
        else if (SceneManager.GetActiveScene().name == SceneName.TutorialScene.ToString())
        {
            obj_tutorialControll_ = GameObject.Find("TutorialControll");
            tutorialControll_ = obj_tutorialControll_.GetComponent<TutorialControll>();
        }

        //isStop_ = false;
        isJump_ = false;
        lastStepTime_ = DateTime.Now;
        isMaxHeight_ = false;
        //fallPos_ = transform.position.y;

    }

    // Update is called once per frame
    void Update()
    {

        if (isStop_) return;

        //Xinput関連
        if (!playerInputSet_ || !prevState_.IsConnected)
        {
            playerIndex_ = (PlayerIndex)0;
            playerInputSet_ = true;
        }
        prevState_ = padState_;
        padState_ = GamePad.GetState(playerIndex_);

        isGround();
        isGoalFlag();
        isDead();

        if (isJump_ && isGround_) SoundManager.GetInstance.PlaySE("Landing_SE");

        //死亡したら
        if (isDead())
        {
            SoundManager.GetInstance.PlaySE("FallDead_SE");
            if (SceneManager.GetActiveScene().name == SceneName.PlayScene.ToString())
            {
                playControll_.playerDeadFrag = true;
            }
            else if (SceneManager.GetActiveScene().name == SceneName.PlayScene.ToString())
            {
                tutorialControll_.playerDeadFrag = true;
            }
        }
    }

    void FixedUpdate()
    {
        Vector3 targetVelocity = Vector3.zero;
        float halfForward = forwardSpeed_ / 2;
        float halfSide = sideSpeed_ / 2;
        if (isStop_) return;
        
        //地面にいるとき
        if (padState_.ThumbSticks.Left.Y >= 0.3f && isGround_)
        {
            //前
            targetVelocity = camera_.transform.forward * (padState_.ThumbSticks.Left.Y * forwardSpeed_);

        }
        else if (padState_.ThumbSticks.Left.Y <= -0.3f && isGround_)
        {
            //後
            targetVelocity = -camera_.transform.forward * (-padState_.ThumbSticks.Left.Y * forwardSpeed_);

        }
        else if (padState_.ThumbSticks.Left.X >= 0.3f && isGround_)
        {
            //右
            targetVelocity = camera_.transform.right * (padState_.ThumbSticks.Left.X * sideSpeed_);
        }
        else if (padState_.ThumbSticks.Left.X <= -0.3f && isGround_)
        {
            //左
            targetVelocity = -camera_.transform.right * (-padState_.ThumbSticks.Left.X * halfSide);
        }
        //空中にいるとき
        if (padState_.ThumbSticks.Left.Y >= 0.3f && !isGround_)
        {
            //前
            targetVelocity = camera_.transform.forward * (padState_.ThumbSticks.Left.Y * halfForward);

        }
        else if (padState_.ThumbSticks.Left.Y <= -0.3f && !isGround_)
        {
            //後
            targetVelocity = -camera_.transform.forward * (-padState_.ThumbSticks.Left.Y * halfForward);

        }
        else if (padState_.ThumbSticks.Left.X >= 0.3f && !isGround_)
        {
            //右
            targetVelocity = camera_.transform.right * (padState_.ThumbSticks.Left.X * halfSide);
        }
        else if (padState_.ThumbSticks.Left.X <= -0.3f && !isGround_)
        {
            //左
            targetVelocity = -camera_.transform.right * (-padState_.ThumbSticks.Left.X * sideSpeed_);
        }

        //地面にいるとき
        //左前
        if (padState_.ThumbSticks.Left.Y >= 0.3f && padState_.ThumbSticks.Left.X <= -0.3f && isGround_)
        {
            targetVelocity = (camera_.transform.forward * (padState_.ThumbSticks.Left.Y * forwardSpeed_) + -camera_.transform.right * (-padState_.ThumbSticks.Left.X * sideSpeed_));
        }
        //右前
        if (padState_.ThumbSticks.Left.Y >= 0.3f && padState_.ThumbSticks.Left.X >= 0.3f && isGround_)
        {
            targetVelocity = (camera_.transform.forward * (padState_.ThumbSticks.Left.Y * forwardSpeed_) + camera_.transform.right * (padState_.ThumbSticks.Left.X * sideSpeed_));
        }
        //左後
        if(padState_.ThumbSticks.Left.Y <= -0.3f && padState_.ThumbSticks.Left.X <= -0.3f && isGround_)
        {
            targetVelocity = (-camera_.transform.forward * (-padState_.ThumbSticks.Left.Y * forwardSpeed_) + -camera_.transform.right * (-padState_.ThumbSticks.Left.X * sideSpeed_));
        }
        //右後
        if(padState_.ThumbSticks.Left.Y <= -0.3f && padState_.ThumbSticks.Left.X >= 0.3f && isGround_)
        {
            targetVelocity = (-camera_.transform.forward * (-padState_.ThumbSticks.Left.Y * forwardSpeed_) + camera_.transform.right * (padState_.ThumbSticks.Left.X * sideSpeed_));
        }
        //空中にいるとき
        //左前
        if (padState_.ThumbSticks.Left.Y >= 0.3f && padState_.ThumbSticks.Left.X <= -0.3f && !isGround_)
        {
            targetVelocity = (camera_.transform.forward * (padState_.ThumbSticks.Left.Y * halfForward) + -camera_.transform.right * (-padState_.ThumbSticks.Left.X * sideSpeed_));
        }
        //右前
        if (padState_.ThumbSticks.Left.Y >= 0.3f && padState_.ThumbSticks.Left.X >= 0.3f && !isGround_)
        {
            targetVelocity = (camera_.transform.forward * (padState_.ThumbSticks.Left.Y * halfForward) + camera_.transform.right * (padState_.ThumbSticks.Left.X * sideSpeed_));
        }
        //左後
        if (padState_.ThumbSticks.Left.Y <= -0.3f && padState_.ThumbSticks.Left.X <= -0.3f && !isGround_)
        {
            targetVelocity = (-camera_.transform.forward * (-padState_.ThumbSticks.Left.Y * halfForward) + -camera_.transform.right * (-padState_.ThumbSticks.Left.X * sideSpeed_));
        }
        //右後
        if (padState_.ThumbSticks.Left.Y <= -0.3f && padState_.ThumbSticks.Left.X >= 0.3f && !isGround_)
        {
            targetVelocity = (-camera_.transform.forward * (-padState_.ThumbSticks.Left.Y * halfForward) + camera_.transform.right * (padState_.ThumbSticks.Left.X * sideSpeed_));
        }

        //移動
        if (targetVelocity.magnitude > 0.01)
        {
            FootSound();

            Move(targetVelocity);
        }
        else
        {
            //初期化
            targetVelocity = new Vector3(0, targetVelocity.y, 0);
            rb_.velocity = new Vector3(0, rb_.velocity.y, 0);
        }

        //ジャンプ
        if (isGround_ && (Input.GetButton("Jump") || Input.GetKey(KeyCode.Space)))
        {
            //Debug.Log("ジャンプしてます");
            SoundManager.GetInstance.PlaySE("Janp_SE");
            rb_.velocity = new Vector3(0, jumpPower_, 0);
            //totalFallTime_ = 0.0f;
            isGround_ = false;
            
        }

        //totalFallTime_ = Time.deltaTime;
        //rb_.velocity = Physics.gravity;

    }

    void isGround()
    {
        RaycastHit hit;
        if (Physics.SphereCast(charaRay_.transform.position, 0.2f, -Vector3.up, out hit, rayRange_))
        {
            if (hit.collider.CompareTag("stage") || hit.collider.CompareTag("ChangeObject") || hit.collider.CompareTag("GravityObj") || hit.collider.CompareTag("GravityObject"))
            {
                isGround_ = true;
                inAir = false;
                
            }
        }
        else
        {
            isGround_ = false;
            
            if (inAir == false)
            {
                fallPos_ = transform.position;
                inAir = true;
            }
        }
        //Debug.Log("接地判定" + isGround_);
    }

    public bool isDead()
    {

        //死亡判定用
        if (!isGround_)
        {

            if (Mathf.Abs(fallPos_.y - transform.position.y) >= 4.5f)
            {
                //Debug.Log("通った2");
                return true;
            }
        }
        else
        {
            return false;
        }

        return false;
    }

    void FootSound()
    {
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
    }

    void isGoalFlag()
    {
        RaycastHit hit;
        if (Physics.Raycast(camera_.transform.position, camera_.transform.forward, out hit, 1f))
        {

            //Debug.Log(hit.collider.gameObject);
            if (hit.collider.CompareTag("GoleObject") || hit.collider.CompareTag("GoalObject"))
            {

                if (SceneManager.GetActiveScene().name == SceneName.PlayScene.ToString())
                {
                    playControll_.stageClearFrag = true;
                    SoundManager.GetInstance.PlaySE("Goal_SE");
                }
                else if (SceneManager.GetActiveScene().name == SceneName.TutorialScene.ToString())

                {
                    tutorialControll_.stageClearFrag = true;
                    SoundManager.GetInstance.PlaySE("Goal_SE");
                }

            }

        }
        //Debug.DrawRay(camera_.transform.position, camera_.transform.forward * rayRange_,Color.red);

    }

    public void Move(Vector3 moveVelocity)
    {
        rb_.velocity = new Vector3(moveVelocity.x, 0.0f + rb_.velocity.y, moveVelocity.z);
    }

}
