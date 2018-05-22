using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Animator))]
public class Player : MonoBehaviour
{
    public enum PlayerState{
        Arive,
        Dead,
    }
    public PlayerState playerState_;//{ get; private set; }
    private Animator animaotor_;
    //移動方向
    [SerializeField]
    private Vector3 velocity_;
    //移動速度
    [SerializeField, Tooltip("移動速度")]
    private float moveSpeed_ = 5.0f;
    //地面に達しているかどうか
    private bool isGround_;
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

    private Vector3 currentPos_;
    private Vector3 previewPos_;
    
    //カメラ取得
    CamChange cc;
    
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
        isGround_ = true;
        rb_ = GetComponent<Rigidbody>();
        
        playerState_ = PlayerState.Arive;
        cc = Camera.main.GetComponent<CamChange>();
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

        if (!isGround_)
        {
            rb_.useGravity = true;
        }
        else
        {
            rb_.useGravity = false;
        }

        if (isGround_)
        {
            velocity_ = Vector3.zero;

            //地面に設置しているときは初期化
            if (isGround_)
            {
                //animaotor_.SetBool("Jump", false);
                rb_.useGravity = true;
                previewPos_ = this.transform.position;
            }
            else
            {
                velocity_ = new Vector3(0f, velocity_.y, 0f);
            }

            //三人称
            var cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
            Vector3 direction = cameraForward * Input.GetAxis("Vertical") + Camera.main.transform.right * Input.GetAxis("Horizontal");
            

            //方向キーの入力量を計測
            if (direction.magnitude > 0.01f)
            {
                //animaotor_.SetFloat("Speed", direction.magnitude);

                transform.LookAt(transform.position + direction);

                Debug.Log("移動してます");

                
                velocity_ += direction * moveSpeed_;
                
            }
        }

        //ジャンプ
        if (isGround_ && (Input.GetKeyDown(KeyCode.Space) || (prevState_.Buttons.A == ButtonState.Released && padState_.Buttons.A == ButtonState.Pressed)))
        {
            Debug.Log("飛んでます");
            isGround_ = false;
            //animaotor_.SetBool("Jump", true);
            velocity_.y += jumpPower_;
            rb_.useGravity = false;
            
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

            //currentPos_ = this.transform.position;
        }
        else
        {
            deathTimer_ = 0.0f;
        }
        if (deathTimer_ >= deathTime_)
        {
            //死亡時の処理
            Debug.Log("死にました");
            playerState_ = PlayerState.Dead;
            SceneManager.LoadScene("AlphaScene");
            Destroy(gameObject);
        }
        

        //キー入力
        if (Input.GetMouseButtonDown(1) || (padState_.Triggers.Left >= 0.7f))
        {
            Debug.Log("一人称");
            cc.isFps_ = true;
            
        }
        if (Input.GetMouseButtonUp(1) || (padState_.Triggers.Left <= 0.7f))
        {
            Debug.Log("三人称");
            cc.isFps_ = false;
        }
        
    }

    void FixedUpdate()
    {
         //キャラクターを移動させる処理
         rb_.MovePosition(transform.position + velocity_ * Time.deltaTime);
            
    }

    void OnCollisionEnter(Collision collision)
    {
        
        if((collision.gameObject.tag == ("stage") || collision.gameObject.tag == ("ChangeObject")) && !isGround_)
        {
            isGround_ = true;

        }

        if(collision.gameObject.tag == ("DeathArea"))
        {
            SceneManager.LoadScene("AlphaScene");
        }
    }

}