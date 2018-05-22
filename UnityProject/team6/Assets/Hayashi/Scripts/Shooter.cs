using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XInputDotNetPure;

public class Shooter : MonoBehaviour
{
    //軸のタイプ
    enum AxisType
    {
        X, //x軸
        Y, //y軸
        Z  //z軸
    }
    //変更する値の指定
    enum ChangeType
    {
        Position, //位置
        Scale    //大きさ
    }
    //銃のタイプ指定
    enum ShotType
    {
        Getting,    //取得
        Setting    //転置
    }

    private AxisType axisType = AxisType.X;
    private ChangeType changeType = ChangeType.Position;
    private ShotType shotType = ShotType.Getting;

    private bool playerIndexSet = false;
    private PlayerIndex playerIndex;
    private GamePadState state;
    private GamePadState prevState;

    [SerializeField]
    private GameObject bulletPrefabs;   //弾のプレハブ
    private GameObject bullet;          //弾生成ようオブジェクト
    [SerializeField]
    private Transform bulletShotPos;    //発射位置

    [SerializeField]
    private Text axisText; //テスト用軸テキスト
    [SerializeField]
    private Text changeText; //テスト用変更する値テキスト
    [SerializeField]
    private Text shotText; //テスト用銃テキスト

    [SerializeField]
    private Material outlineMat;
    private GameObject prevOriginObj;    //前回の取得するオブジェクト
    private GameObject originObject;     //数値を取得するオブジェクト
    private GameObject targetObject;     //数値を転置するオブジェクト
    private Vector3 originPositionValue; //取得するオブジェクトの位置の数値
    private Vector3 originScaleValue;    //取得するオブジェクトの大きさの数値

    [SerializeField]
    private GameObject objValPref_ray;
    [SerializeField]
    private GameObject objValPref;
    private GameObject objVal_origin_ray;
    private GameObject objVal_origin;
    private GameObject objVal_target_ray;
    private GameObject objVal_target;

    private bool isTargetMove;      //転置するオブジェクトが動いているか
    private bool isShotBullet;   //Rトリガーが押されたか
    private bool getLTriggerDown;   //Lトリガーが押されたか

    private RaycastHit rayhit;
    [SerializeField]
    private float rayDistance = 20.0f;
    private int layerMask;
    private GameObject rayOriginObj;
    private GameObject rayTargetObj;
    private bool isRayHit;
    private bool isOriginGet;
    private bool isTargetGet;

    // Use this for initialization
    void Start()
    {
        //各変数の初期化
        prevOriginObj = null;
        originObject = null;
        targetObject = null;
        originPositionValue = Vector3.zero;
        originScaleValue = Vector3.zero;

        objVal_origin_ray = null;
        objVal_origin = null;
        objVal_target_ray = null;
        objVal_target = null;
        //isOriginMove = false;
        isTargetMove = false;

        isShotBullet = false;
        getLTriggerDown = false;

        axisText.text = "axis:" + axisType.ToString();
        changeText.text = "change:" + changeType.ToString();
        shotText.text = "shot:" + shotType.ToString();

        layerMask = ~(1 << 9);
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerIndexSet || !prevState.IsConnected)
        {
            playerIndex = (PlayerIndex)0;
            playerIndexSet = true;
        }
        prevState = state;
        state = GamePad.GetState(playerIndex);
        //軸切り替えボタンが押されたら
        if (Input.GetKeyDown(KeyCode.E) || (prevState.Buttons.RightShoulder == ButtonState.Released && state.Buttons.RightShoulder == ButtonState.Pressed))
        {
            //軸の切り替え
            SwitchAxisType();
        }
        //値切り替えボタンが押されたら
        if (Input.GetKeyDown(KeyCode.Q) || (prevState.Buttons.LeftShoulder == ButtonState.Released && state.Buttons.LeftShoulder == ButtonState.Pressed))
        {
            //値の切り替え
            SwitchChangeType();
        }
        //弾が発射されていないかつショットタイプ切り替えボタンが押されたら
        if (bullet == null && (Input.GetKeyDown(KeyCode.R) || (prevState.Buttons.Y == ButtonState.Released && state.Buttons.Y == ButtonState.Pressed)))
        {
            //ショットタイプの切り替え
            SwitchShotType();
        }
        //カメラ切り替え(三人称から一人称へ)
        if (Input.GetMouseButton(1) || state.Triggers.Left >= 0.8f)
        {
            //一人称へ変換処理


            //射撃ボタンが押されたら
            if ((Input.GetMouseButtonDown(0) || state.Triggers.Right >= 0.8f) && isShotBullet == false)
            {
                //弾を発射
                ShotBullet();
                //弾を撃った
                isShotBullet = true;
            }
        }
        if (isShotBullet == true && state.Triggers.Right <= 0.3f) isShotBullet = false;
        //if (Input.GetMouseButtonUp(1) || Input.GetAxisRaw("LRTrigger") < 0.1f) firstParsonMode = false;

        GetObject_Ray();
        if (isRayHit == true)
        {
            switch (shotType)
            {
                case ShotType.Getting:
                    if (isTargetGet == false)
                        ObjectsValueDraw_Ray(rayOriginObj, null);
                    break;
                case ShotType.Setting:
                    if (isOriginGet == false)
                        ObjectsValueDraw_Ray(null, rayTargetObj);
                    else
                        ObjectsValueDraw_Ray(originObject.transform.parent.gameObject, rayTargetObj);
                    break;
            }
        }

        if (isOriginGet == true) ObjectsValueDraw_Origin();
        if (isTargetGet == true) ObjectsValueDraw_Target();
    }

    //弾の発射
    private void ShotBullet()
    {
        //弾が発射されていなければ弾の生成を行う
        if (bullet == null)
            bullet = Instantiate(bulletPrefabs, bulletShotPos.position, bulletShotPos.rotation);
        //既に発射されていれば現在の弾を削除して生成を行う(誤動作を防ぐため)
        else
        {
            Destroy(bullet);
            bullet = Instantiate(bulletPrefabs, bulletShotPos.position, bulletShotPos.rotation);
        }
    }
    //レイで取得、転置オブジェクトを取得
    private void GetObject_Ray()
    {
        if (Physics.SphereCast(bulletShotPos.position, 0.25f, bulletShotPos.forward, out rayhit, rayDistance, layerMask))
        {
            if (rayhit.collider.gameObject.tag == "ChangeObject")
            {
                isRayHit = true;
                switch (shotType)
                {
                    case ShotType.Getting:
                        rayOriginObj = rayhit.collider.transform.parent.gameObject;
                        if (objVal_target_ray != null) Destroy(objVal_target_ray.gameObject);
                        if (objVal_origin_ray == null)
                        {
                            objVal_origin_ray = Instantiate(objValPref_ray,
                                                     new Vector3(
                                                         rayOriginObj.transform.parent.localPosition.x,
                                                         rayOriginObj.transform.parent.localPosition.y + 1.0f,
                                                         rayOriginObj.transform.parent.localPosition.z),
                                                     Quaternion.identity
                                                    );
                            objVal_origin_ray.GetComponent<ValueDrawerController>().GetDrawBaseObj(rayOriginObj.transform.parent.gameObject);
                        }
                        break;
                    case ShotType.Setting:
                        rayTargetObj = rayhit.collider.transform.parent.gameObject;
                        if (objVal_origin_ray != null) Destroy(objVal_origin_ray.gameObject);
                        if (objVal_target_ray == null)
                        {
                            objVal_target_ray = Instantiate(objValPref_ray,
                                                     new Vector3(
                                                         rayTargetObj.transform.parent.localPosition.x,
                                                         rayTargetObj.transform.parent.localPosition.y + 1.0f,
                                                         rayTargetObj.transform.parent.localPosition.z),
                                                     Quaternion.identity
                                                    );
                            objVal_target_ray.GetComponent<ValueDrawerController>().GetDrawBaseObj(rayTargetObj.transform.parent.gameObject);
                        }
                        break;
                }
            }
            else
            {
                if (objVal_origin_ray != null) Destroy(objVal_origin_ray.gameObject);
                if (objVal_target_ray != null) Destroy(objVal_target_ray.gameObject);
                isRayHit = false;
            }
        }
        else
        {
            if (objVal_origin_ray != null) Destroy(objVal_origin_ray.gameObject);
            if (objVal_target_ray != null) Destroy(objVal_target_ray.gameObject);
            isRayHit = false;
        }

        Debug.DrawRay(bulletShotPos.position, bulletShotPos.forward * rayDistance, Color.red);
    }
    //取得、転置オブジェクトの数値の表示
    private void ObjectsValueDraw_Ray(GameObject originObj, GameObject targetObj)
    {
        switch (changeType)
        {
            case ChangeType.Position:
                switch (axisType)
                {
                    case AxisType.X:
                        if (originObj != null && objVal_origin_ray != null)
                            objVal_origin_ray.transform.Find("Canvas").Find("ValueText").GetComponent<Text>().text = originObj.transform.parent.localPosition.x.ToString("f2");
                        if (targetObj != null && objVal_target_ray != null)
                            objVal_target_ray.transform.Find("Canvas").Find("ValueText").GetComponent<Text>().text = targetObj.transform.parent.localPosition.x.ToString("f2");
                        break;
                    case AxisType.Y:
                        if (originObj != null && objVal_origin_ray != null)
                            objVal_origin_ray.transform.Find("Canvas").Find("ValueText").GetComponent<Text>().text = originObj.transform.parent.localPosition.y.ToString("f2");
                        if (targetObj != null && objVal_target_ray != null)
                            objVal_target_ray.transform.Find("Canvas").Find("ValueText").GetComponent<Text>().text = targetObj.transform.parent.localPosition.y.ToString("f2");
                        break;
                    case AxisType.Z:
                        if (originObj != null && objVal_origin_ray != null)
                            objVal_origin_ray.transform.Find("Canvas").Find("ValueText").GetComponent<Text>().text = originObj.transform.parent.localPosition.z.ToString("f2");
                        if (targetObj != null && objVal_target_ray != null)
                            objVal_target_ray.transform.Find("Canvas").Find("ValueText").GetComponent<Text>().text = targetObj.transform.parent.localPosition.z.ToString("f2");
                        break;
                }
                break;
            case ChangeType.Scale:
                switch (axisType)
                {
                    case AxisType.X:
                        if (originObj != null && objVal_origin_ray != null)
                            objVal_origin_ray.transform.Find("Canvas").Find("ValueText").GetComponent<Text>().text = originObj.transform.localScale.x.ToString("f2");
                        if (targetObj != null && objVal_target_ray != null)
                            objVal_target_ray.transform.Find("Canvas").Find("ValueText").GetComponent<Text>().text = targetObj.transform.localScale.x.ToString("f2");
                        break;
                    case AxisType.Y:
                        if (originObj != null && objVal_origin_ray != null)
                            objVal_origin_ray.transform.Find("Canvas").Find("ValueText").GetComponent<Text>().text = originObj.transform.localScale.y.ToString("f2");
                        if (targetObj != null && objVal_target_ray != null)
                            objVal_target_ray.transform.Find("Canvas").Find("ValueText").GetComponent<Text>().text = targetObj.transform.localScale.y.ToString("f2");
                        break;
                    case AxisType.Z:
                        if (originObj != null && objVal_origin_ray != null)
                            objVal_origin_ray.transform.Find("Canvas").Find("ValueText").GetComponent<Text>().text = originObj.transform.localScale.z.ToString("f2");
                        if (targetObj != null && objVal_target_ray != null)
                            objVal_target_ray.transform.Find("Canvas").Find("ValueText").GetComponent<Text>().text = targetObj.transform.localScale.z.ToString("f2");
                        break;
                }
                break;
        }
    }
    private void ObjectsValueDraw_Origin()
    {
        switch (changeType)
        {
            case ChangeType.Position:
                switch (axisType)
                {
                    case AxisType.X:
                        objVal_origin.transform.Find("Canvas").Find("ValueText").GetComponent<Text>().text = originObject.transform.parent.parent.localPosition.x.ToString("f2");
                        break;
                    case AxisType.Y:
                        objVal_origin.transform.Find("Canvas").Find("ValueText").GetComponent<Text>().text = originObject.transform.parent.parent.localPosition.y.ToString("f2");
                        break;
                    case AxisType.Z:
                        objVal_origin.transform.Find("Canvas").Find("ValueText").GetComponent<Text>().text = originObject.transform.parent.parent.localPosition.z.ToString("f2");
                        break;
                }
                break;
            case ChangeType.Scale:
                switch (axisType)
                {
                    case AxisType.X:
                        objVal_origin.transform.Find("Canvas").Find("ValueText").GetComponent<Text>().text = originObject.transform.parent.localScale.x.ToString("f2");
                        break;
                    case AxisType.Y:
                        objVal_origin.transform.Find("Canvas").Find("ValueText").GetComponent<Text>().text = originObject.transform.parent.localScale.y.ToString("f2");
                        break;
                    case AxisType.Z:
                        objVal_origin.transform.Find("Canvas").Find("ValueText").GetComponent<Text>().text = originObject.transform.parent.localScale.z.ToString("f2");
                        break;
                }
                break;
        }
    }
    private void ObjectsValueDraw_Target()
    {
        switch (changeType)
        {
            case ChangeType.Position:
                switch (axisType)
                {
                    case AxisType.X:
                        objVal_target.transform.Find("Canvas").Find("ValueText").GetComponent<Text>().text = targetObject.transform.parent.parent.localPosition.x.ToString("f2");
                        break;
                    case AxisType.Y:
                        objVal_target.transform.Find("Canvas").Find("ValueText").GetComponent<Text>().text = targetObject.transform.parent.parent.localPosition.y.ToString("f2");
                        break;
                    case AxisType.Z:
                        objVal_target.transform.Find("Canvas").Find("ValueText").GetComponent<Text>().text = targetObject.transform.parent.parent.localPosition.z.ToString("f2");
                        break;
                }
                break;
            case ChangeType.Scale:
                switch (axisType)
                {
                    case AxisType.X:
                        objVal_target.transform.Find("Canvas").Find("ValueText").GetComponent<Text>().text = targetObject.transform.parent.localScale.x.ToString("f2");
                        break;
                    case AxisType.Y:
                        objVal_target.transform.Find("Canvas").Find("ValueText").GetComponent<Text>().text = targetObject.transform.parent.localScale.y.ToString("f2");
                        break;
                    case AxisType.Z:
                        objVal_target.transform.Find("Canvas").Find("ValueText").GetComponent<Text>().text = targetObject.transform.parent.localScale.z.ToString("f2");
                        break;
                }
                break;
        }
    }
    //銃のタイプ切り替え
    private void SwitchShotType()
    {
        switch (shotType)
        {
            case ShotType.Getting:
                shotType = ShotType.Setting;
                shotText.text = "shot:" + shotType.ToString();
                break;
            case ShotType.Setting:
                shotType = ShotType.Getting;
                shotText.text = "shot:" + shotType.ToString();
                break;
        }
    }
    //軸切り替え
    private void SwitchAxisType()
    {
        switch (axisType)
        {
            case AxisType.X:
                axisType = AxisType.Y;
                axisText.text = "axis:" + axisType.ToString();
                break;
            case AxisType.Y:
                axisType = AxisType.Z;
                axisText.text = "axis:" + axisType.ToString();
                break;
            case AxisType.Z:
                axisType = AxisType.X;
                axisText.text = "axis:" + axisType.ToString();
                break;
        }
    }
    //値切り替え
    private void SwitchChangeType()
    {
        switch (changeType)
        {
            case ChangeType.Position:
                changeType = ChangeType.Scale;
                changeText.text = "change:" + changeType.ToString();
                break;
            case ChangeType.Scale:
                changeType = ChangeType.Position;
                changeText.text = "change:" + changeType.ToString();
                break;
        }
    }

    /// <summary>
    /// 弾が当たった時の処理
    /// </summary>
    /// <param name="hitObject">当たったオブジェクト</param>
    public void HitBullet(GameObject hitObject)
    {
        //取得するオブジェクトと転置するオブジェクトが動いていなければ
        if (isTargetMove == false)
        {
            //当たった時の銃のタイプによりそれぞれ処理
            switch (shotType)
            {
                //取得タイプなら
                case ShotType.Getting:
                    //取得するオブジェクトに当たったオブジェクトを格納
                    prevOriginObj = originObject;
                    if (prevOriginObj != null)
                    {
                        prevOriginObj.GetComponent<ObjectController>().ResetMaterial();
                    }
                    originObject = hitObject.transform.gameObject;
                    originObject.GetComponent<ObjectController>().ChangeMaterial(outlineMat);
                    Destroy(objVal_origin);
                    objVal_origin = Instantiate(objValPref,
                                                    new Vector3(
                                                        originObject.transform.parent.parent.localPosition.x,
                                                        originObject.transform.parent.parent.localPosition.y + 1.0f,
                                                        originObject.transform.parent.parent.localPosition.z),
                                                    Quaternion.identity
                                                   );
                    objVal_origin.GetComponent<ValueDrawerController>().GetDrawBaseObj(originObject.transform.parent.parent.gameObject);
                    isOriginGet = true;
                    //取得するオブジェクトの値を取得
                    GetOriginAxisLength();
                    break;
                //転置タイプなら
                case ShotType.Setting:
                    //取得するオブジェクトが空でなければ
                    if (originObject != null)
                    {
                        //転置するオブジェクトに当たったオブジェクトを格納                   
                        targetObject = hitObject.transform.gameObject;
                        targetObject.GetComponent<ObjectController>().ChangeMaterial(outlineMat);
                        Destroy(objVal_target);
                        objVal_target = Instantiate(objValPref,
                                                     new Vector3(
                                                         targetObject.transform.parent.parent.localPosition.x,
                                                         targetObject.transform.parent.parent.localPosition.y + 1.0f,
                                                         targetObject.transform.parent.parent.localPosition.z),
                                                     Quaternion.identity
                                                    );
                        objVal_target.GetComponent<ValueDrawerController>().GetDrawBaseObj(targetObject.transform.parent.parent.gameObject);
                        isTargetGet = true;
                        Debug.Log(isTargetGet);
                        //指定した変更する値の軸を切り替え
                        ChangeObjectAxis();
                    }
                    break;
            }
        }
    }

    //取得するオブジェクトの値を取得
    private void GetOriginAxisLength()
    {
        originPositionValue = originObject.transform.parent.parent.localPosition;
        originScaleValue = originObject.transform.parent.localScale;
    }

    //指定した変更する値の軸を切り替え
    private void ChangeObjectAxis()
    {
        //isOriginMove = true;
        isTargetMove = true;
        //変更する値のタイプによってそれぞれ処理
        switch (changeType)
        {
            //位置
            case ChangeType.Position:
                switch (axisType)
                {
                    case AxisType.X:
                        targetObject.transform.GetComponent<ObjectController>().PositionShiftStart(
                            new Vector3(originPositionValue.x,
                                        targetObject.transform.parent.parent.position.y,
                                        targetObject.transform.parent.parent.position.z));
                        break;
                    case AxisType.Y:
                        targetObject.transform.GetComponent<ObjectController>().PositionShiftStart(
                            new Vector3(targetObject.transform.parent.parent.position.x,
                                        originPositionValue.y,
                                        targetObject.transform.parent.parent.position.z));
                        break;
                    case AxisType.Z:
                        targetObject.transform.GetComponent<ObjectController>().PositionShiftStart(
                            new Vector3(targetObject.transform.parent.parent.position.x,
                                        targetObject.transform.parent.parent.position.y,
                                        originPositionValue.z));
                        break;
                }
                break;
            //大きさ
            case ChangeType.Scale:
                switch (axisType)
                {
                    case AxisType.X:
                        targetObject.transform.GetComponent<ObjectController>().ScaleShiftStart(
                            new Vector3(originScaleValue.x,
                                        targetObject.transform.parent.localScale.y,
                                        targetObject.transform.parent.localScale.z));
                        break;
                    case AxisType.Y:
                        targetObject.transform.GetComponent<ObjectController>().ScaleShiftStart(
                            new Vector3(targetObject.transform.parent.localScale.x,
                                        originScaleValue.y,
                                        targetObject.transform.parent.localScale.z));
                        break;
                    case AxisType.Z:
                        targetObject.transform.GetComponent<ObjectController>().ScaleShiftStart(
                            new Vector3(targetObject.transform.parent.localScale.x,
                                        targetObject.transform.parent.localScale.y,
                                        originScaleValue.z));
                        break;
                }
                break;
        }
    }

    public void MovingEnd()
    {
        isTargetGet = false;
        isTargetMove = false;
        Destroy(objVal_target);
    }
}
