using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChildSceneCameraControll : MonoBehaviour
{
    [SerializeField]
    GameObject MainCam;

    GameObject obj;
    SceneControll sceneControll;
    void Start()
    {
        MainCam.SetActive(false);
    }

    void Update()
    {

    }
}
