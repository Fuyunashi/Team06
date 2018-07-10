using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XInputDotNetPure;

public class TutorialTextManager : MonoBehaviour {

    [SerializeField]
    private Text[] tutorial_1_text;
    [SerializeField]
    private Text[] tutorial_2_text;

    [SerializeField]
    private Image[] tutorialImages_;

    [SerializeField]
    private Image[] controllerGuide_;

    [SerializeField]
    private Image ButtonImage_;

    tutorialMoveObjGet moveObjget_;
    SceneControll scene_;
    KeyRestriction key_;
    Shooter shooter_;

    int textCount_ = 0;
   
    private bool isTextEnable_;
    
    private Player player_;

    private TutorialControll tutorialcontroll_;

    //Xinput関連
    private bool playerInputSet_ = false;
    private PlayerIndex playerIndex_;
    private GamePadState padState_;
    private GamePadState prevState_;

    void Start()
    {

        //すべてEnableにする
        for (int i = 0; i < tutorialImages_.Length; i++)
        {
            //Debug.Log("消しました");
            tutorialImages_[i].enabled = false;
        }
        for (int i = 0; i < tutorial_1_text.Length; i++)
        {
            //Debug.Log("消しました");
            tutorial_1_text[i].enabled = false;
        }
        for (int i = 0; i < tutorial_2_text.Length; i++)
        {
            //Debug.Log("消しました");
            tutorial_2_text[i].enabled = false;
        }
        for(int i = 0; i < controllerGuide_.Length; i++)
        {
            controllerGuide_[i].enabled = false;
        }
        ButtonImage_.enabled = false;

        //スクリプト読み込み
        scene_ = GameObject.Find("SceneController").GetComponent<SceneControll>();
        player_ = GameObject.Find("FPSPlayer").GetComponent<Player>();
        key_ = player_.GetComponent<KeyRestriction>();
        shooter_ = player_.GetComponent<Shooter>();
        moveObjget_ = GameObject.Find("moveObjGet").GetComponent<tutorialMoveObjGet>();
        tutorialcontroll_ = GameObject.Find("TutorialControll").GetComponent<TutorialControll>();
        textCount_ = 0;
        
        //debug
        player_.isStop_ = true;
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
        
        //更新
        TutorialImageEnabled();

        //プレイヤー
        if (player_.isStop_)
        {
            isTextEnable_ = true;
            if (prevState_.Buttons.B == ButtonState.Released && padState_.Buttons.B == ButtonState.Pressed){
                textCount_++;
            }
            
        }

        if (player_.isStop_ && (scene_.CurrentStage == NextStage.Tutrial1 || scene_.CurrentStage == NextStage.Tutrial2))
        {
            ButtonImage_.enabled = true;
        }
        else
        {
            ButtonImage_.enabled = false;
        }

        if (scene_.PuseFrag || tutorialcontroll_.changeSceneFrag)
        {
            tutorial_1_text[textCount_].enabled = false;
            tutorial_2_text[textCount_].enabled = false;
            tutorialImages_[0].enabled = false;
            tutorialImages_[1].enabled = false;
            controllerGuide_[0].enabled = false;
            controllerGuide_[1].enabled = false;
            controllerGuide_[2].enabled = false;
            controllerGuide_[3].enabled = false;
            ButtonImage_.enabled = false;
        }
        

        //Debug.Log(player_.isStop_);
        //Debug.Log(scene_.PuseFrag);
        Debug.Log(textCount_);
        Debug.Log(key_.currentUseKey);
        //Debug.Log(tutorial_1_text.Length);
        
    }

    //チュートリアルのウィンドウ切り替え
    void TutorialImageEnabled()
    {
        if (scene_.CurrentStage == NextStage.Tutrial1 && isTextEnable_ &&  !tutorialcontroll_.changeSceneFrag)
        {
            key_.currentUseKey = UseKey.TriggersLeft;
            tutorialImages_[0].enabled = true;
            Tutorial_1_TextMng();
            Tutorial1KeyContoroll();
            
            if (textCount_ >= tutorial_1_text.Length)
            {
                tutorialImages_[0].enabled = false;
                //textCount_ = 0;
                player_.isStop_ = false;
                isTextEnable_ = false;
            }
        }

        if (scene_.CurrentStage == NextStage.Tutrial2 && isTextEnable_ && !tutorialcontroll_.changeSceneFrag)
        {
                
            tutorialImages_[1].enabled = true;
            Tutorial_2_TextMng();
            Tutorial2KeyContoroll();
            

            if (textCount_ >= tutorial_2_text.Length)
            {
                tutorialImages_[1].enabled = false;
                //textCount_ = 0;
                player_.isStop_ = false;
                isTextEnable_ = false;
            }
        }

        if(scene_.CurrentScene == SceneName.SelectScene || tutorialcontroll_.changeSceneFrag)
        {
            textCount_ = 0;
        }
    }

    void Tutorial1KeyContoroll()
    {
        
        if (textCount_ == 3)
        {
            player_.isStop_ = false;
            //key_.currentUseKey = UseKey.None;
            if(shooter_.GetRayObj() == moveObjget_.tutorial1_moveObj_3)
            {
                key_.currentUseKey = UseKey.TriggersLeft;
                if (shooter_.GetIsShot())
                {
                    player_.isStop_ = true;
                    //key_.currentUseKey = UseKey.None;
                    textCount_ += 1;
                }

            }
            
        }
        else if(textCount_ == 4)
        {
            player_.isStop_ = false;
            //key_.currentUseKey = UseKey.None;
            if(shooter_.GetRayObj() == moveObjget_.tutorial1_moveObj_2)
            {
                key_.currentUseKey = UseKey.TriggersRight;
                if (shooter_.GetIsShot())
                {
                    player_.isStop_ = true;
                    //key_.currentUseKey = UseKey.None;
                    textCount_ += 1;
                }
            }
        }
        else if(textCount_ == 5)
        {
            player_.isStop_ = true;
        }
        else if(textCount_ == 6)
        {
            player_.isStop_ = true;
        }
        else if(textCount_ == 7)
        {
            player_.isStop_ = false;
            key_.currentUseKey = UseKey.None;
            if (shooter_.GetRayObj() == moveObjget_.tutorial1_moveObj_3 || shooter_.GetRayObj() == moveObjget_.tutorial1_moveObj_2)
            {
                key_.currentUseKey = UseKey.TriggersLeft;
                if (shooter_.GetIsShot())
                {
                    key_.currentUseKey = UseKey.RightShoulder;
                    textCount_ += 1;
                }
            }

        }
        else if(textCount_ == 8)
        {

            bool isAxisX = false;
            player_.isStop_ = false;
            //key_.currentUseKey = UseKey.RightShoulder;
            if (shooter_.GetAxisType() != "Z")
            {
                key_.currentUseKey = UseKey.RightShoulder;
                controllerGuide_[3].enabled = true;
                controllerGuide_[2].enabled = false;
            }
            else if (shooter_.GetAxisType() == "Z")
            {
                isAxisX = true;
                controllerGuide_[2].enabled = true;
                controllerGuide_[3].enabled = false;
                key_.currentUseKey = UseKey.TriggersRight;
            }
            else
            {
                isAxisX = false;
                controllerGuide_[3].enabled = true;
                controllerGuide_[2].enabled = false;
            }

            if (isAxisX && shooter_.GetRayObj() == moveObjget_.tutorial1_moveObj_1)
            {
                if (shooter_.GetIsShot())
                {
                    controllerGuide_[3].enabled = false;
                    Debug.Log("来てます");
                    player_.isStop_ = true;
                    isAxisX = false;
                    textCount_ += 1;
                }
            }


        }
        else if (textCount_ == 9)
        {
            player_.isStop_ = true;
            controllerGuide_[2].enabled = false;

        }
        else if(textCount_ == 10)
        {
            player_.isStop_ = true;
            key_.currentUseKey = UseKey.None;
        }


    }

    void Tutorial2KeyContoroll()
    {
        key_.currentUseKey = UseKey.None;
        if(textCount_ == 1)
        {
            player_.isStop_ = false;
            //key_.currentUseKey = UseKey.LeftShoulder;
            if(padState_.Buttons.LeftShoulder == ButtonState.Pressed && prevState_.Buttons.LeftShoulder == ButtonState.Released)
            {
                player_.isStop_ = true;
                textCount_ += 1;
            }
        }
        else if (textCount_ == 2)
        {
            player_.isStop_ = true;
        }
        else if(textCount_ == 3)
        {
            player_.isStop_ = false;
            if(padState_.Buttons.RightShoulder == ButtonState.Pressed && prevState_.Buttons.RightShoulder == ButtonState.Released)
            {
                player_.isStop_ = true;
                textCount_ += 1;
            }
        }
        else if(textCount_ == 4)
        {
            player_.isStop_ = true;
        }
        
        
    }

    //チュートリアル1
    void Tutorial_1_TextMng()
    {
        switch (textCount_)
        {
            case 0:
                    tutorial_1_text[0].enabled = true; break;
            case 1:
                    tutorial_1_text[0].enabled = false;
                    tutorial_1_text[1].enabled = true; break;
            case 2:
                    tutorial_1_text[1].enabled = false;
                    tutorial_1_text[2].enabled = true; break;
            case 3:
                    tutorial_1_text[2].enabled = false;
                    tutorial_1_text[3].enabled = true;
                    controllerGuide_[0].enabled = true;
                    break;
            case 4:
                    tutorial_1_text[3].enabled = false;
                    tutorial_1_text[4].enabled = true;
                    controllerGuide_[0].enabled = false;
                    controllerGuide_[2].enabled = true;
                    break;
            case 5:
                    tutorial_1_text[4].enabled = false;
                    tutorial_1_text[5].enabled = true;
                    controllerGuide_[2].enabled = false;
                     break;
            case 6:
                    tutorial_1_text[5].enabled = false;
                    tutorial_1_text[6].enabled = true; break;
            case 7:
                    tutorial_1_text[6].enabled = false;
                    tutorial_1_text[7].enabled = true;
                    controllerGuide_[0].enabled = true; break;
            case 8:
                    tutorial_1_text[7].enabled = false;
                    tutorial_1_text[8].enabled = true;
                    controllerGuide_[0].enabled = false; break;
            case 9:
                    tutorial_1_text[8].enabled = false;
                    tutorial_1_text[9].enabled = true; break;
            case 10:
                    tutorial_1_text[9].enabled = false;
                    tutorial_1_text[10].enabled = true; break;
            case 11:
                    tutorial_1_text[10].enabled = false;
                    tutorial_1_text[11].enabled = true; break;
            case 12:
                    tutorial_1_text[11].enabled = false; break;
        }
        
    }
    //チュートリアル2
    void Tutorial_2_TextMng()
    {
        switch (textCount_)
        {
            case 0:
                tutorial_2_text[0].enabled = true; break;
            case 1:      
                tutorial_2_text[0].enabled = false;
                tutorial_2_text[1].enabled = true;
                controllerGuide_[1].enabled = true;
                break;
            case 2:      
                tutorial_2_text[1].enabled = false;
                tutorial_2_text[2].enabled = true;
                controllerGuide_[1].enabled = false;
                break;
            case 3:      
                tutorial_2_text[2].enabled = false;
                tutorial_2_text[3].enabled = true;
                controllerGuide_[3].enabled = true;
                break;
            case 4:      
                tutorial_2_text[3].enabled = false;
                tutorial_2_text[4].enabled = true;
                controllerGuide_[3].enabled = false;
                break;
            case 5:      
                tutorial_2_text[4].enabled = false;
                tutorial_2_text[5].enabled = true; break;
            case 6:      
                tutorial_2_text[5].enabled = false;
                tutorial_2_text[6].enabled = true; break;
            case 7:
                tutorial_2_text[6].enabled = false;
                tutorial_2_text[7].enabled = true; break;
            case 8:
                tutorial_2_text[7].enabled = false; break;
        }

    }
    
}
