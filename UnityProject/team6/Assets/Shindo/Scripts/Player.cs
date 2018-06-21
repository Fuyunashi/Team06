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
    private float fowardSpeed_;
    //左右の速さ
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

    [SerializeField]
    private float deathTime_;
    private float deathTimer_;
    private float distance_;
    private float fallPos_;
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

    public Camera camera_;
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
        rb_.useGravity = false;

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

        isStop_ = false;
        isJump_ = false;
        lastStepTime_ = DateTime.Now;
        fallPos_ = transform.position.y;
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
        
        if (!isGround_)
        {
            rb_.AddForce(new Vector3(0, -Gravity_ * rb_.mass, 0));
            
        }
        
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
        
        var cameraForward = Vector3.Scale(camera_.transform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 targetVelocity = cameraForward * Input.GetAxis("Vertical") + camera_.transform.right * Input.GetAxis("Horizontal");

        //移動
        if (targetVelocity.magnitude > 0.01)
        {
            FootSound();
            rb_.velocity = targetVelocity * fowardSpeed_;
        }
        else
        {
            //初期化
            targetVelocity = Vector3.zero;
            rb_.velocity = Vector3.zero;
        }
        
        //ジャンプ
        if (isGround_ && !isJump_ && (Input.GetButton("Jump") || Input.GetKey(KeyCode.Space)))
        {
            Debug.Log("ジャンプしてます");
            SoundManager.GetInstance.PlaySE("Janp_SE");
            rb_.velocity += new Vector3(0, jumpPower_, 0);
            isGround_ = false;
            isJump_ = true;
        }
        if (isJump_ && isGround_)
        {
            SoundManager.GetInstance.PlaySE("Landing_SE");
            isJump_ = false;
        }
        
    }

    void isGround()
    {
        RaycastHit hit;
        if (Physics.SphereCast(charaRay_.transform.position, 0.2f, -Vector3.up, out hit, rayRange_))
        {
            if (hit.collider.CompareTag("stage") || hit.collider.CompareTag("ChangeObject") || hit.collider.CompareTag("GravityObj"))
            {
                isGround_ = true;
            }
            else
            {
                isGround_ = false;
            }
        }
        Debug.Log("接地判定" + isGround_);
    }

    bool isDead()
    {
        //死亡判定用
        if (!isGround_)
        {
            distance_ = fallPos_ - transform.position.y;
            deathTimer_ += Time.deltaTime;

            if (distance_ >= deathDistance_)
            {
                Debug.Log("死亡しました");
                return true;
            }
            //Debug.Log(distance_ + "m 落ちました");
        }
        else
        {
            deathTimer_ = 0.0f;
            return false;
        }

        if (deathTimer_ >= deathTime_)
        {
            Debug.Log("死亡しました");
            return true;
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
        if (Physics.Raycast(camera_.transform.position, camera_.transform.forward, out hit, rayRange_))
        {

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

    }
}
