using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class PlayControll : MonoBehaviour
{
    enum PouseSelect
    {
        Tocontinue,
        Restart,
        Select,
    }

    //Xinput関連
    private bool playerInputSet_ = false;
    private PlayerIndex playerIndex_;
    private GamePadState padState_;
    private GamePadState prevState_;

    GameObject MianCam;
    GameObject SubCam;

    GameObject obj_sceneControll;
    SceneControll sceneControll;
    GameObject obj_portal;
    DistortPortal distortPortal;


    PouseSelect pouseSelect;
    int pouseCount = 3;
    int pouseSelectIndex = 0;
    bool changeSceneFrag;

    void Start()
    {
        obj_sceneControll = GameObject.Find("SceneController");
        sceneControll = obj_sceneControll.GetComponent<SceneControll>();
        obj_portal = GameObject.Find("MainCamera");
        distortPortal = obj_portal.GetComponent<DistortPortal>();

        changeSceneFrag = false;
    }

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

        if (prevState_.Buttons.B == ButtonState.Released && padState_.Buttons.B == ButtonState.Pressed)
        {
            Debug.Log("ポータル時間" + distortPortal.portalTime);
            distortPortal.PortalFlag = true;
        }
        //ポウズ中の処理
        if (sceneControll.PuseFrag)
        {
            Debug.Log("次のステージは：" + (PouseSelect)pouseSelectIndex);
            Debug.Log("ポータル時間" + distortPortal.portalTime);
            PouseOperation();
        }
        if (distortPortal.portalTime <= 0)
        {
        }

    }

    private void PouseOperation()
    {

        if (prevState_.DPad.Up == ButtonState.Released && padState_.DPad.Up == ButtonState.Pressed)
        {
            pouseSelectIndex--;
            pouseSelectIndex += pouseCount;
            pouseSelectIndex = pouseSelectIndex % pouseCount;

        }
        else if (prevState_.DPad.Down == ButtonState.Released && padState_.DPad.Down == ButtonState.Pressed)
        {
            pouseSelectIndex++;
            pouseSelectIndex = pouseSelectIndex % pouseCount;
        }
    }
}
