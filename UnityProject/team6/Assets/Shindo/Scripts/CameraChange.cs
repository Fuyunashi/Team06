using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraChange : MonoBehaviour {

    public GameObject FpsObj_;
    public GameObject TpsObj_;

    public Camera FpsCamera_;
    public Camera TpsCamera_;

    

    private Player player_;

    public bool isFps_ { get; set; }
    // Use this for initialization

    GameObject obj, obj2;

    void Awake()
    {
        obj = FpsObj_;
        obj2 = TpsObj_;
        obj.SetActiveRecursively(false);
        obj2.SetActiveRecursively(true);
    }

    void Start () {
        ShowThirdPersonView();
	}
	
	// Update is called once per frame
	void Update () {

        if (isFps_)
        {
            ShowFirstPersonView();
        }
        else
        {
            ShowThirdPersonView();
        }

	}

    public void ShowFirstPersonView()
    {
        FpsObj_.SetActive(true);
        TpsObj_.SetActive(false);
        
    }

    public void ShowThirdPersonView()
    {
        FpsObj_.SetActive(false);
        TpsObj_.SetActive(true);
    }
    
}
