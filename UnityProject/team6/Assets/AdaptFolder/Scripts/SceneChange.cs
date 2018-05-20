using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour {

    Player player_ = new Player();
        
	// Use this for initialization
	void Start () {
        //SceneManager.LoadScene("Title");
        //SceneManager.LoadScene("AlphaScene");
        
	}
	
	// Update is called once per frame
	void Update () {
		if(player_.playerState_ == Player.PlayerState.Dead)
        {
            SceneManager.LoadScene("AlphaScene");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            //シーン遷移
            Debug.Log("クリア");
            SceneManager.LoadScene("Clear");
        }
    }
}
