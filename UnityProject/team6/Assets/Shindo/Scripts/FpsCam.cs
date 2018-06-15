using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class FpsCam : MonoBehaviour {

    public float spinSpeed = 0.5f;
    float distance = 2f;

    Vector3 pos = Vector3.zero;
    public Vector2 mouse = Vector2.zero;

    public Transform target_;
    public float distance_;
    public float cameraHeight_;

    Player player_;

    //Xinput関連
    private bool playerInputSet_ = false;
    private PlayerIndex playerIndex_;
    private GamePadState padState_;
    private GamePadState prevState_;

    // Use this for initialization
    void Start () {
        mouse.x = -0.5f;
        mouse.y = 0.5f;

        player_ = GameObject.Find("FPSPlayer").GetComponent<Player>();
    }
	
	// Update is called once per frame
	void Update () {

        //カメラ停止
        if(player_.isStop_ == true)
        {
            return;
        }

        //Xinput関連
        if (!playerInputSet_ || !prevState_.IsConnected)
        {
            playerIndex_ = (PlayerIndex)0;
            playerInputSet_ = true;
        }
        prevState_ = padState_;
        padState_ = GamePad.GetState(playerIndex_);

        // マウスの移動の取得
        if (padState_.ThumbSticks.Right.X >= 0.8f || padState_.ThumbSticks.Right.X <= 0.8f || 
            padState_.ThumbSticks.Right.Y >= 0.8f || padState_.ThumbSticks.Right.Y <= 0.8f)
        {
            mouse += new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * Time.deltaTime * spinSpeed;
        }

        var lookAt = target_.position + Vector3.up * cameraHeight_;
        this.transform.position = lookAt - transform.forward * distance_;

        mouse.y = Mathf.Clamp(mouse.y, -0.3f + 0.5f, 0.3f + 0.5f);

        // 球面座標系変換
        pos.x = distance * Mathf.Sin(mouse.y * Mathf.PI) * Mathf.Cos(mouse.x * Mathf.PI);
        pos.y = -distance * Mathf.Cos(mouse.y * Mathf.PI);
        pos.z = -distance * Mathf.Sin(mouse.y * Mathf.PI) * Mathf.Sin(mouse.x * Mathf.PI);

        // 座標の更新
        transform.LookAt(pos + this.transform.position);
    }
}
