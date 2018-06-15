using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class TitleCameraMove : MonoBehaviour
{
    enum CameraMoveType
    {
        Rotate,
        Move,
        Up,
        None,
    }
    //Xinput関連
    private bool playerInputSet_ = false;
    private PlayerIndex playerIndex_;
    private GamePadState padState_;
    private GamePadState prevState_;
    //シーン切替時の演出タイプ
    CameraMoveType cameraMoveType;
    //シーンを切り替えるフラグ
    public bool sceneChangeFlag;

    //監視カメラオブジェクト
    public GameObject SurveillanceCamera;
    //カメラが見るべき相手のおべジェクト
    public GameObject Player;

    //演出前の地震のポジションプロパティ
    Vector3 CameraPos { get; set; }
    //演出前の地震の回転度プロパティ
    Quaternion CameraRota { get; set; }

    //CRTの指示をだすためにスクリプトをもらう
    CRTnoise crtNoise;

    NoiseParticle noiseParticle;
    void Start()
    {
        /** いろいろ初期化 **/
        sceneChangeFlag = false;
        cameraMoveType = CameraMoveType.Rotate;

        //CRTスクリプトの確保
        crtNoise = GetComponent<CRTnoise>();
        noiseParticle = GetComponent<NoiseParticle>();
        //crtNoise.cameraName = CRTnoise.CameraName.TitleMainCamera;

    }
    float time = 0;
    void Update()
    {
        //インプット関連
        if (!playerInputSet_ || !prevState_.IsConnected)
        {
            playerIndex_ = (PlayerIndex)0;
            playerInputSet_ = true;
        }
        prevState_ = padState_;
        padState_ = GamePad.GetState(playerIndex_);
        //インプット関連


        if (prevState_.Buttons.B == ButtonState.Released && padState_.Buttons.B == ButtonState.Pressed ||
        prevState_.Buttons.A == ButtonState.Released && padState_.Buttons.A == ButtonState.Pressed ||
        prevState_.Buttons.X == ButtonState.Released && padState_.Buttons.X == ButtonState.Pressed ||
            prevState_.Buttons.Y == ButtonState.Released && padState_.Buttons.Y == ButtonState.Pressed)
        {
            if (!noiseParticle.skipFrag) { noiseParticle.skipFrag = true; return; }

            sceneChangeFlag = true;
        }

        if (sceneChangeFlag)
        {
            CameraPos = transform.position;
            switch (cameraMoveType)
            {
                case CameraMoveType.Rotate:
                    var aim = SurveillanceCamera.transform.position - CameraPos;
                    // var look = Vector3.RotateTowards(transform.forward, aim, 0.5f * Time.deltaTime, 0f);
                    //transform.rotation = Quaternion.LookRotation(look);

                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(aim), time);
                    time += Time.deltaTime;
                    if (time >= 1)
                    {
                        cameraMoveType = CameraMoveType.Move;
                    }
                    //Debug.Log("カメラ回転中：" + (transform.position - look) + "見たい数：" + CameraRota);
                    break;
                case CameraMoveType.Move:
                    transform.position = SurveillanceCamera.transform.position;
                    var target = Player.transform.position - CameraPos;
                    var look_ = Quaternion.LookRotation(target);
                    transform.localRotation = look_;
                    time += Time.deltaTime * 0.5f;

                    if (time >= 1.5)
                    {
                        cameraMoveType = CameraMoveType.Up;
                    }
                    break;
                case CameraMoveType.Up:
                    crtNoise.CRTFlag = true;
                    cameraMoveType = CameraMoveType.None;
                    break;
                case CameraMoveType.None:
                    break;
            }
        }



    }
}
