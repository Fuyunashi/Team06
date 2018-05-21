using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FpsCam : MonoBehaviour {

    public float spinSpeed = 0.5f;
    float distance = 2f;

    Vector3 pos = Vector3.zero;
    public Vector2 mouse = Vector2.zero;

    public Transform target_;
    public float distance_;
    public float cameraHeight_;
    
    // Use this for initialization
    void Start () {
        mouse.x = -0.5f;
        mouse.y = 0.5f;
    }
	
	// Update is called once per frame
	void Update () {
        // マウスの移動の取得
        if (Input.GetMouseButton(1))
        {
            mouse += new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * Time.deltaTime * spinSpeed;
        }

        var lookAt = target_.position + Vector3.up * cameraHeight_;
        this.transform.position = lookAt - transform.forward * distance_;

        mouse.y = Mathf.Clamp(mouse.y, -0.3f + 0.5f, 0.3f + 0.5f);

        // 球面座標系変換
        pos.x = distance * Mathf.Sin(mouse.y * Mathf.PI) * Mathf.Cos(mouse.x * Mathf.PI);
        pos.y = -distance * Mathf.Cos(mouse.y * Mathf.PI);
        pos.z = -distance * Mathf.Sin(mouse.y * Mathf.PI) * Mathf.Sin(mouse.x * Mathf.PI);

        // 座標の更新
        transform.LookAt(pos + this.transform.position);
    }
}
