using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_Raycast_Laser : MonoBehaviour {
    
    public GameObject Raybody; //레이캐스팅을 쏘는 위치
    public GameObject ScaleDistance; //거리에 따른 스케일 변화를 위한 오브젝트 대상
    public GameObject RayResult; //충돌하는 위치에 출력할 결과
    // Use this for initialization
    void Start () {
    
    }
	
    // Update is called once per frame
    void Update () {

        RaycastHit hit; 

        Physics.Raycast(transform.position, transform.forward, out hit, 200);

        ScaleDistance.transform.localScale = new Vector3(1, hit.distance, 1);
        
        RayResult.transform.position = hit.point;

        RayResult.transform.rotation = Quaternion.LookRotation(hit.normal); 
    }
}
