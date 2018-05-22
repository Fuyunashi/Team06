using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TpsCam : MonoBehaviour {

    public Transform target;
    public float spinSpeed = 1f;
    float distance = 2f;

    Vector3 nowPos;
    Vector3 pos = Vector3.zero;
    public Vector2 mouse = Vector2.zero;

    // Use this for initialization
    void Start () {
        // 初期位置の取得
        nowPos = transform.position;
    }
	
	// Update is called once per frame
	void Update () {
        // マウスの移動の取得
        if (Input.GetMouseButton(0))
        {
            mouse += new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * Time.deltaTime * spinSpeed;
        }

        mouse.y = Mathf.Clamp(mouse.y, -0.3f + 0.5f, 0.3f + 0.5f);

        // 球面座標系変換
        pos.x = distance * Mathf.Sin(mouse.y * Mathf.PI) * Mathf.Cos(mouse.x * Mathf.PI);
        pos.y = -distance * Mathf.Cos(mouse.y * Mathf.PI);
        pos.z = -distance * Mathf.Sin(mouse.y * Mathf.PI) * Mathf.Sin(mouse.x * Mathf.PI);

        pos *= nowPos.z;

        pos.y += nowPos.y;

        // 座標の更新
        transform.position = pos + target.position;
        transform.LookAt(target.position);
    }

}

