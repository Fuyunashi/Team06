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

    SceneControll scene_;

    int textCount_ = 0;

    private bool isTextEnable_;
    private bool isTextEnd_;

    private float deleteTimer_;

    private Player player_;

    //Xinput関連
    private bool playerInputSet_ = false;
    private PlayerIndex playerIndex_;
    private GamePadState padState_;
    private GamePadState prevState_;

    void Awake()
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
        
    }

    void Start()
    {
        scene_ = GameObject.Find("SceneController").GetComponent<SceneControll>();
        player_ = GameObject.Find("FPSPlayer").GetComponent<Player>();
        textCount_ = 0;
        deleteTimer_ = 0.0f;
        //debug
        player_.isStop_ = true;
        isTextEnd_ = false;
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
        
        //
        if (player_.isStop_)
        {
            isTextEnable_ = true;
            if (prevState_.Buttons.B == ButtonState.Released && padState_.Buttons.B == ButtonState.Pressed){
                textCount_++;
            }
        }
        else
        {
            
        }

        if (textCount_ >= tutorial_1_text.Length) deleteTimer_ += 0.1f;
        if (textCount_ >= tutorial_2_text.Length) deleteTimer_ += 0.1f;

        //if(tutorialImages_[0].enabled == false || tutorialImages_[1].enabled == false) deleteTimer_ = 0; 

        Debug.Log(player_.isStop_);
        //Debug.Log(textCount_);
        Debug.Log(deleteTimer_);
        Debug.Log(tutorial_2_text.Length);
        Debug.Log(scene_.CurrentStage);
    }

    //チュートリアルのウィンドウ切り替え
    void TutorialImageEnabled()
    {
        if (scene_.CurrentStage == NextStage.Tutrial1 && isTextEnable_)
        {
            tutorialImages_[0].enabled = true;
            Tutorial_1_TextMng();

            if (textCount_ >= tutorial_1_text.Length && deleteTimer_ >= 1.0f)
            {
                tutorialImages_[0].enabled = false;
                textCount_ = 0;
                //deleteTimer_ = 0;
                player_.isStop_ = false;
                isTextEnable_ = false;
            }
        }

        if (scene_.CurrentStage == NextStage.Tutrial2)
        {
            tutorialImages_[1].enabled = true;
            Tutorial_2_TextMng();

            if (textCount_ >= tutorial_2_text.Length && deleteTimer_ >= 1.0f)
            {
                tutorialImages_[1].enabled = false;
                textCount_ = 0;
                //deleteTimer_ = 0;
                player_.isStop_ = false;
                isTextEnable_ = false;
            }
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
                    tutorial_1_text[3].enabled = true; break;
            case 4:
                    tutorial_1_text[3].enabled = false;
                    tutorial_1_text[4].enabled = true; break;
            case 5:
                    tutorial_1_text[4].enabled = false;
                    tutorial_1_text[5].enabled = true; break;
            case 6:
                    tutorial_1_text[5].enabled = false;
                    tutorial_1_text[6].enabled = true; break;
            case 7:
                    tutorial_1_text[6].enabled = false;
                    tutorial_1_text[7].enabled = true; break;
            case 8:
                    tutorial_1_text[7].enabled = false;
                    tutorial_1_text[8].enabled = true; break;
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
                tutorial_2_text[1].enabled = true; break;
            case 2:      
                tutorial_2_text[1].enabled = false;
                tutorial_2_text[2].enabled = true; break;
            case 3:      
                tutorial_2_text[2].enabled = false;
                tutorial_2_text[3].enabled = true; break;
            case 4:      
                tutorial_2_text[3].enabled = false;
                tutorial_2_text[4].enabled = true; break;
            case 5:      
                tutorial_2_text[4].enabled = false;
                tutorial_2_text[5].enabled = true; break;
            case 6:      
                tutorial_2_text[5].enabled = false;
                tutorial_2_text[6].enabled = true; break;
            case 7:
                tutorial_2_text[6].enabled = false; break;
        }

    }
    
}
