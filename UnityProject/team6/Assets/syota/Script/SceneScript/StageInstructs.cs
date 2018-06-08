using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;


public class StageInstructs : MonoBehaviour
{
    //Xinput関連
    private bool playerInputSet_ = false;
    private PlayerIndex playerIndex_;
    private GamePadState padState_;
    private GamePadState prevState_;

    [System.Serializable]
    public struct MovingStage
    {
        [SerializeField]
        public NextStage currentStage;

        [SerializeField]
        public NextStage up_tv;

        [SerializeField]
        public NextStage right_tv;

        [SerializeField]
        public NextStage left_tv;

        [SerializeField]
        public NextStage dwon_tv;
    }
    [SerializeField, Header("全セレクト数")]
    public MovingStage[] movingStage;

    public NextStage CurrentStage;

  
    void Start()
    {       
    }

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
        //Xinput関連


        //Debug.Log("次のシーン：" + CurrentStage);
        if (prevState_.DPad.Up == ButtonState.Released && padState_.DPad.Up == ButtonState.Pressed)
        {
            foreach (var stage in movingStage)
            {
                if (stage.currentStage != CurrentStage || stage.up_tv == NextStage.None) continue;
                CurrentStage = stage.up_tv;
                return;
            }
        }
        if (prevState_.DPad.Right == ButtonState.Released && padState_.DPad.Right == ButtonState.Pressed)
        {
            foreach (var stage in movingStage)
            {
                //Debug.Log("おらがめあすだ：" + stage.currentStage + "みぎ：" + stage.right_tv);
                if (stage.currentStage != CurrentStage || stage.right_tv == NextStage.None) continue;
                CurrentStage = stage.right_tv;
                return;
            }
        }
        if (prevState_.DPad.Left == ButtonState.Released && padState_.DPad.Left == ButtonState.Pressed)
        {
            foreach (var stage in movingStage)
            {
                if (stage.currentStage != CurrentStage || stage.left_tv == NextStage.None) continue;
                CurrentStage = stage.left_tv;
                return;
            }
        }
        if (prevState_.DPad.Down == ButtonState.Released && padState_.DPad.Down == ButtonState.Pressed)
        {
            foreach (var stage in movingStage)
            {
                if (stage.currentStage != CurrentStage || stage.dwon_tv == NextStage.None) continue;
                CurrentStage = stage.dwon_tv;
                return;
            }
        }
    }
}
