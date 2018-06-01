using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMManager : MonoBehaviour {

    [SerializeField]
    //private string[] bgmName_;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        SoundManager.GetInstance.PlayBGM("A");
	}
}
