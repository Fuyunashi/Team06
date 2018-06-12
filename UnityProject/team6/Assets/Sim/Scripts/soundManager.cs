using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class soundManager : MonoBehaviour {
    public AudioClip enemyDead;
    public AudioClip enemyToPlayer;
    public AudioClip laserHit;

    AudioSource simAudio; 
    public static soundManager instance; 

    void Awake()  
    {
        if (soundManager.instance == null)  
            soundManager.instance = this;  
    }
    // Use this for initialization
    void Start()
    {
        simAudio = GetComponent<AudioSource>();  
    }

    public void EnemyDead()
    {
        simAudio.PlayOneShot(enemyDead);
    }
    public void EnemyToPlayer()
    {
        simAudio.PlayOneShot(enemyToPlayer);
    }
    public void LaserHit()
    {
        simAudio.PlayOneShot(laserHit);
    }
    // Update is called once per frame
    void Update()
    {

    }
}