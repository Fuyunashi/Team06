using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadStage : MonoBehaviour
{
    [SerializeField]
    GameObject[] stage;
    void Start()
    {
        if (GameObject.Find("SceneController").GetComponent<SceneControll>().CurrentScene == SceneName.PlayScene ||
            GameObject.Find("SceneController").GetComponent<SceneControll>().CurrentScene == SceneName.TutorialScene)
        {
            gameObject.SetActive(false);
        }
        DontDestroyOnLoad(this);
    }

    void Update()
    {

    }
}
