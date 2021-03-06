﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public enum UseKey
{
    TriggersRight,
    TriggersLeft,
    RightShoulder,
    LeftShoulder,
    AllKey,
    None,
}
public class KeyRestriction : MonoBehaviour
{

    //Xinput関連
    private bool playerInputSet_ = false;
    private PlayerIndex playerIndex_;
    private GamePadState padState_;
    private GamePadState prevState_;

    public UseKey currentUseKey;
    // Use this for initialization
    void Start()
    {
        currentUseKey = UseKey.None;
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
        
    }
    public bool Restriction()
    {
        if(currentUseKey == UseKey.None)
        {
            return true;
        }
        //セット
        if (padState_.Triggers.Right >= 0.8f)
        {
            if (currentUseKey != UseKey.TriggersRight) return false;
        }


        //ゲット
        if (padState_.Triggers.Left >= 0.8f)
        {
            if (currentUseKey != UseKey.TriggersLeft) return false;
        }


        //切り替えボタン
        if (prevState_.Buttons.RightShoulder == ButtonState.Released && padState_.Buttons.RightShoulder == ButtonState.Pressed)
        {
            if (currentUseKey != UseKey.RightShoulder) return  false;
        }

        //チェンジ
        if (prevState_.Buttons.LeftShoulder == ButtonState.Released && padState_.Buttons.LeftShoulder == ButtonState.Pressed)
        {
            if (currentUseKey != UseKey.LeftShoulder) return false;
        }
        
        return true;
    }


}
