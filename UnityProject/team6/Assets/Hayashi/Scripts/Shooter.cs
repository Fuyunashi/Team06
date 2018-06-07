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
    private GamePadState padState;
    private GamePadState prevState;

    [SerializeField]
    private Transform shotPos;    //発射位置

    [SerializeField]
    private Text axisText; //テスト用軸テキスト
    [SerializeField]
    private Text changeText; //テスト用変更する値テキスト
    [SerializeField]
    private Text shotText; //テスト用銃テキスト

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
    [SerializeField]
    private GameObject drawerCameraPref;
    private GameObject m_drawerCamera;

    private bool isTargetMove;      //転置するオブジェクトが動いているか
    private bool isShot;   //Rトリガーが押されたか

    private Ray ray;
    private RaycastHit rayhit;
    [SerializeField]
    private float rayDistance = 20.0f;
    [SerializeField]
    private LineRenderer laserPointer;
    private int layerMask;
    private GameObject rayOriginObj;
    private GameObject rayTargetObj;
    private bool isRayHit;
    private bool isOriginGet;
    private bool isTargetGet;

    [SerializeField]
    private GameObject estimateObjPrefab;
    private GameObject m_estimateObj = null;

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
        isTargetMove = false;

        isShot = false;

        axisText.text = "axis:" + axisType.ToString();
        changeText.text = "change:" + changeType.ToString();
        shotText.text = "shot:" + shotType.ToString();

        layerMask = ~(1 << 9 | 1 << 8);
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerIndexSet || !prevState.IsConnected)
        {
            playerIndex = (PlayerIndex)0;
            playerIndexSet = true;
        }
        prevState = padState;
        padState = GamePad.GetState(playerIndex);
        //軸切り替えボタンが押されたら
        if (Input.GetKeyDown(KeyCode.E) || (prevState.Buttons.RightShoulder == ButtonState.Released && padState.Buttons.RightShoulder == ButtonState.Pressed))
        {
            SoundManager.GetInstance.PlaySE("Change_SE");
            //軸の切り替え
            SwitchAxisType();
            if (isRayHit == true && shotType == ShotType.Setting)
            {
                if (m_estimateObj != null) Destroy(m_estimateObj.gameObject);
                InstantEstimateObject(rayTargetObj.transform.parent.parent.gameObject);
            }
        }
        //値切り替えボタンが押されたら
        if (Input.GetKeyDown(KeyCode.Q) || (prevState.Buttons.LeftShoulder == ButtonState.Released && padState.Buttons.LeftShoulder == ButtonState.Pressed))
        {
            SoundManager.GetInstance.PlaySE("Change_SE");
            //値の切り替え
            SwitchChangeType();
            if (isRayHit == true && shotType == ShotType.Setting)
            {
                if (m_estimateObj != null) Destroy(m_estimateObj.gameObject);
                InstantEstimateObject(rayTargetObj.transform.parent.parent.gameObject);
            }
        }
        //弾が発射されていないかつショットタイプ切り替えボタンが押されたら
        if ((Input.GetKeyDown(KeyCode.R) || (prevState.Buttons.Y == ButtonState.Released && padState.Buttons.Y == ButtonState.Pressed)))
        {
            SoundManager.GetInstance.PlaySE("Change_SE");
            //ショットタイプの切り替え
            SwitchShotType();
            if (m_estimateObj != null) Destroy(m_estimateObj.gameObject);
        }
        if (isShot == true && (padState.Triggers.Right <= 0.3f || Input.GetMouseButtonUp(0))) isShot = false;

        laserPointer.SetPosition(0, laserPointer.transform.position);
        GetObject_Ray();
        if (isRayHit == true)
        {
            switch (shotType)
            {
                case ShotType.Getting:
                    if (isTargetGet == false)
                        ObjectsValueDraw_Ray(rayOriginObj.transform.parent.gameObject, null);
                    break;
                case ShotType.Setting:
                    if (isOriginGet == false)
                        ObjectsValueDraw_Ray(null, rayTargetObj.transform.parent.gameObject);
                    else
                    {
                        if (originObject != null)
                            ObjectsValueDraw_Ray(originObject.transform.parent.gameObject, rayTargetObj.transform.parent.gameObject);
                    }
                    break;
            }
        }

        if (isOriginGet == true) ObjectsValueDraw_Origin();
        if (isTargetGet == true) ObjectsValueDraw_Target();
    }
    //レイで取得・転置オブジェクトを取得
    private void GetObject_Ray()
    {
        ray = new Ray(shotPos.position, shotPos.forward);
        if (Physics.Raycast(ray, out rayhit, rayDistance, layerMask))
        {
            if (rayhit.collider.gameObject.tag == "ChangeObject")
            {
                isRayHit = true;
                switch (shotType)
                {
                    case ShotType.Getting:
                        rayOriginObj = rayhit.collider.transform.gameObject;
                        if (objVal_target_ray != null) Destroy(objVal_target_ray.gameObject);
                        if (objVal_origin_ray == null)
                        {
                            objVal_origin_ray = Instantiate(objValPref_ray,
                                                     new Vector3(
                                                         rayOriginObj.transform.parent.parent.localPosition.x,
                                                         rayOriginObj.transform.parent.parent.localPosition.y,
                                                         rayOriginObj.transform.parent.parent.localPosition.z) +
                                                         new Vector3(0, rayOriginObj.transform.parent.localScale.y / 2 + 0.5f, 0),
                                                     Quaternion.identity
                                                    );
                            objVal_origin_ray.GetComponent<ValueDrawerController>().GetDrawBaseObj(rayOriginObj.transform.parent.gameObject);
                        }
                        //射撃ボタンが押されたら
                        if ((Input.GetMouseButtonDown(0) || padState.Triggers.Right >= 0.8f) && isShot == false)
                        {
                            SoundManager.GetInstance.PlaySE("Gun_SE");
                            //発射
                            Shot(rayOriginObj);
                            //弾を撃った
                            isShot = true;
                        }
                        break;
                    case ShotType.Setting:
                        rayTargetObj = rayhit.collider.transform.gameObject;
                        if (objVal_origin_ray != null) Destroy(objVal_origin_ray.gameObject);
                        if (objVal_target_ray == null)
                        {
                            objVal_target_ray = Instantiate(objValPref_ray,
                                                     new Vector3(
                                                         rayTargetObj.transform.parent.parent.localPosition.x,
                                                         rayTargetObj.transform.parent.parent.localPosition.y,
                                                         rayTargetObj.transform.parent.parent.localPosition.z) +
                                                         new Vector3(0, rayTargetObj.transform.parent.localScale.y / 2 + 0.5f, 0),
                                                     Quaternion.identity
                                                    );
                            objVal_target_ray.GetComponent<ValueDrawerController>().GetDrawBaseObj(rayTargetObj.transform.parent.gameObject);
                        }
                        InstantEstimateObject(rayTargetObj.transform.parent.parent.gameObject);
                        //射撃ボタンが押されたら
                        if ((Input.GetMouseButtonDown(0) || padState.Triggers.Right >= 0.8f) && isShot == false)
                        {
                            SoundManager.GetInstance.PlaySE("Gun_SE");
                            //発射
                            Shot(rayTargetObj);
                            //弾を撃った
                            isShot = true;
                        }
                        break;
                }
            }
            else
            {
                if (objVal_origin_ray != null) Destroy(objVal_origin_ray.gameObject);
                if (objVal_target_ray != null) Destroy(objVal_target_ray.gameObject);
                if (m_estimateObj != null) Destroy(m_estimateObj.gameObject);
                isRayHit = false;
            }
            laserPointer.SetPosition(1, rayhit.point);
        }
        else
        {
            if (objVal_origin_ray != null) Destroy(objVal_origin_ray.gameObject);
            if (objVal_target_ray != null) Destroy(objVal_target_ray.gameObject);
            if (m_estimateObj != null) Destroy(m_estimateObj.gameObject);
            isRayHit = false;
            laserPointer.SetPosition(1, ray.origin + ray.direction * rayDistance);
        }
        //Debug.DrawRay(shotPos.position, shotPos.forward * rayDistance, Color.red);
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
                            objVal_origin_ray.GetComponent<ValueDrawerController>().ObjectAxisValue(originObj.transform.parent.localPosition.x);
                        if (targetObj != null && objVal_target_ray != null)
                            objVal_target_ray.GetComponent<ValueDrawerController>().ObjectAxisValue(targetObj.transform.parent.localPosition.x);
                        break;
                    case AxisType.Y:
                        if (originObj != null && objVal_origin_ray != null)
                            objVal_origin_ray.GetComponent<ValueDrawerController>().ObjectAxisValue(originObj.transform.parent.localPosition.y);
                        if (targetObj != null && objVal_target_ray != null)
                            objVal_target_ray.GetComponent<ValueDrawerController>().ObjectAxisValue(targetObj.transform.parent.localPosition.y);
                        break;
                    case AxisType.Z:
                        if (originObj != null && objVal_origin_ray != null)
                            objVal_origin_ray.GetComponent<ValueDrawerController>().ObjectAxisValue(originObj.transform.parent.localPosition.z);
                        if (targetObj != null && objVal_target_ray != null)
                            objVal_target_ray.GetComponent<ValueDrawerController>().ObjectAxisValue(targetObj.transform.parent.localPosition.z);
                        break;
                }
                break;
            case ChangeType.Scale:
                switch (axisType)
                {
                    case AxisType.X:
                        if (originObj != null && objVal_origin_ray != null)
                            objVal_origin_ray.GetComponent<ValueDrawerController>().ObjectAxisValue(originObj.transform.localScale.x);
                        if (targetObj != null && objVal_target_ray != null)
                            objVal_target_ray.GetComponent<ValueDrawerController>().ObjectAxisValue(targetObj.transform.localScale.x);
                        break;
                    case AxisType.Y:
                        if (originObj != null && objVal_origin_ray != null)
                            objVal_origin_ray.GetComponent<ValueDrawerController>().ObjectAxisValue(originObj.transform.localScale.y);
                        if (targetObj != null && objVal_target_ray != null)
                            objVal_target_ray.GetComponent<ValueDrawerController>().ObjectAxisValue(targetObj.transform.localScale.y);
                        break;
                    case AxisType.Z:
                        if (originObj != null && objVal_origin_ray != null)
                            objVal_origin_ray.GetComponent<ValueDrawerController>().ObjectAxisValue(originObj.transform.localScale.z);
                        if (targetObj != null && objVal_target_ray != null)
                            objVal_target_ray.GetComponent<ValueDrawerController>().ObjectAxisValue(targetObj.transform.localScale.z);
                        break;
                }
                break;
        }
    }
    //取得オブジェクトの数値表示処理
    private void ObjectsValueDraw_Origin()
    {
        switch (changeType)
        {
            case ChangeType.Position:
                switch (axisType)
                {
                    case AxisType.X:
                        if (objVal_origin != null)
                            objVal_origin.GetComponent<ValueDrawerController>().ObjectAxisValue(originPositionValue.x);
                        break;
                    case AxisType.Y:
                        if (objVal_origin != null)
                            objVal_origin.GetComponent<ValueDrawerController>().ObjectAxisValue(originPositionValue.y);
                        break;
                    case AxisType.Z:
                        if (objVal_origin != null)
                            objVal_origin.GetComponent<ValueDrawerController>().ObjectAxisValue(originPositionValue.z);
                        break;
                }
                break;
            case ChangeType.Scale:
                switch (axisType)
                {
                    case AxisType.X:
                        if (objVal_origin != null)
                            objVal_origin.GetComponent<ValueDrawerController>().ObjectAxisValue(originScaleValue.x);
                        break;
                    case AxisType.Y:
                        if (objVal_origin != null)
                            objVal_origin.GetComponent<ValueDrawerController>().ObjectAxisValue(originScaleValue.y);
                        break;
                    case AxisType.Z:
                        if (objVal_origin != null)
                            objVal_origin.GetComponent<ValueDrawerController>().ObjectAxisValue(originScaleValue.z);
                        break;
                }
                break;
        }
    }
    //転置オブジェクトの数値表示処理
    private void ObjectsValueDraw_Target()
    {
        switch (changeType)
        {
            case ChangeType.Position:
                switch (axisType)
                {
                    case AxisType.X:
                        objVal_target.GetComponent<ValueDrawerController>().ObjectAxisValue(targetObject.transform.parent.parent.localPosition.x);
                        break;
                    case AxisType.Y:
                        objVal_target.GetComponent<ValueDrawerController>().ObjectAxisValue(targetObject.transform.parent.parent.localPosition.y);
                        break;
                    case AxisType.Z:
                        objVal_target.GetComponent<ValueDrawerController>().ObjectAxisValue(targetObject.transform.parent.parent.localPosition.z);
                        break;
                }
                break;
            case ChangeType.Scale:
                switch (axisType)
                {
                    case AxisType.X:
                        objVal_target.GetComponent<ValueDrawerController>().ObjectAxisValue(targetObject.transform.parent.localScale.x);
                        break;
                    case AxisType.Y:
                        objVal_target.GetComponent<ValueDrawerController>().ObjectAxisValue(targetObject.transform.parent.localScale.y);
                        break;
                    case AxisType.Z:
                        objVal_target.GetComponent<ValueDrawerController>().ObjectAxisValue(targetObject.transform.parent.localScale.z);
                        break;
                }
                break;
        }
    }
    //コピー後の予測の生成
    private void InstantEstimateObject(GameObject obj)
    {
        if (m_estimateObj == null && isOriginGet == true)
        {
            switch (changeType)
            {
                case ChangeType.Position:
                    switch (axisType)
                    {
                        case AxisType.X:
                            m_estimateObj = Instantiate(estimateObjPrefab, new Vector3(
                                                originPositionValue.x,
                                                obj.transform.position.y,
                                                obj.transform.position.z
                                                ), obj.transform.localRotation);
                            break;
                        case AxisType.Y:
                            m_estimateObj = Instantiate(estimateObjPrefab, new Vector3(
                                               obj.transform.position.x,
                                               originPositionValue.y,
                                               obj.transform.position.z
                                               ), obj.transform.localRotation);
                            break;
                        case AxisType.Z:
                            m_estimateObj = Instantiate(estimateObjPrefab, new Vector3(
                                               obj.transform.position.x,
                                               obj.transform.position.y,
                                               originPositionValue.z
                                               ), obj.transform.localRotation);
                            break;
                    }
                    m_estimateObj.transform.GetChild(0).localScale = new Vector3(
                              obj.transform.GetChild(0).localScale.x,
                              obj.transform.GetChild(0).localScale.y,
                              obj.transform.GetChild(0).localScale.z);
                    m_estimateObj.transform.GetChild(0).localPosition = obj.transform.GetChild(0).localPosition;
                    break;
                case ChangeType.Scale:
                    m_estimateObj = Instantiate(estimateObjPrefab,
                              obj.transform.localPosition,
                              obj.transform.localRotation);
                       m_estimateObj.transform.GetChild(0).localPosition = obj.transform.GetChild(0).localPosition;
                    switch (axisType)
                    {
                        case AxisType.X:
                            m_estimateObj.transform.GetChild(0).localScale = new Vector3(
                                originScaleValue.x,
                                obj.transform.GetChild(0).localScale.y,
                                obj.transform.GetChild(0).localScale.z);
                            break;
                        case AxisType.Y:
                            m_estimateObj.transform.GetChild(0).localScale = new Vector3(
                               obj.transform.GetChild(0).localScale.x,
                               originScaleValue.y,
                               obj.transform.GetChild(0).localScale.z);
                            break;
                        case AxisType.Z:
                            m_estimateObj.transform.GetChild(0).localScale = new Vector3(
                                obj.transform.GetChild(0).localScale.x,
                                obj.transform.GetChild(0).localScale.y,
                                originScaleValue.z);
                            break;
                    }
                    break;
            }
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
    public void Shot(GameObject hitObject)
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
                    originObject = hitObject.transform.gameObject;
                    if (prevOriginObj != null && prevOriginObj != originObject)
                    {
                        prevOriginObj.GetComponent<ObjectController>().DeleteOutline();
                    }
                    if (prevOriginObj != originObject)
                    {
                        originObject.GetComponent<ObjectController>().SetOutline();
                        Destroy(objVal_origin);
                        objVal_origin = Instantiate(objValPref,
                                                        new Vector3(
                                                            originObject.transform.parent.parent.localPosition.x,
                                                            originObject.transform.parent.parent.localPosition.y,
                                                            originObject.transform.parent.parent.localPosition.z) +
                                                            new Vector3(0, originObject.transform.parent.localScale.y + 0.5f, 0),
                                                        Quaternion.identity
                                                       );
                        objVal_origin.GetComponent<ValueDrawerController>().GetDrawBaseObj(originObject.transform.parent.gameObject);
                        //取得するオブジェクトの値を取得
                        GetOriginAxisLength();
                        isOriginGet = true;
                    }
                    break;
                //転置タイプなら
                case ShotType.Setting:
                    //取得するオブジェクトが空でなければ
                    if (originObject != null && targetObject == null)
                    {
                        //転置するオブジェクトに当たったオブジェクトを格納                   
                        targetObject = hitObject.transform.gameObject;
                        targetObject.GetComponent<ObjectController>().SetOutline();
                        Destroy(objVal_target);
                        objVal_target = Instantiate(objValPref,
                                                     new Vector3(
                                                         targetObject.transform.parent.parent.localPosition.x,
                                                         targetObject.transform.parent.parent.localPosition.y,
                                                         targetObject.transform.parent.parent.localPosition.z) +
                                                         new Vector3(0, targetObject.transform.parent.localScale.y + 0.5f, 0),
                                                     Quaternion.identity
                                                    );
                        objVal_target.GetComponent<ValueDrawerController>().GetDrawBaseObj(targetObject.transform.parent.gameObject);
                        isTargetGet = true;
                        SettingAnimation(targetObject.transform.parent.gameObject);
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
    //オブジェクトの動作終了時処理
    public void MovingEnd()
    {
        if (targetObject != null)
        {
            LeanTween.delayedCall(1.0f, () =>
            {
                isTargetGet = false;
                originObject = null;
                targetObject = null;
                Destroy(objVal_target);
                isTargetMove = false;
            });
        }
    }
    //コピー時の演出アニメーション処理
    private void SettingAnimation(GameObject target)
    {
        m_drawerCamera = Instantiate(drawerCameraPref, this.transform.position + new Vector3(0, 1f, 0), this.transform.rotation);
        m_drawerCamera.GetComponent<DrawerCamera>().SetDrawerObj(objVal_origin);

        LeanTween.delayedCall(1.0f, () =>
        {
            objVal_origin.GetComponent<ValueDrawerController>().IsTween(true);
            LeanTween.move(objVal_origin, target.transform.parent.position + new Vector3(0, target.transform.localScale.y / 2 + 1.0f, 0), 1.0f).setOnComplete(() =>
            {
                LeanTween.moveY(objVal_origin, objVal_origin.transform.position.y - 1.0f, 0.5f).setOnComplete(() =>
                {
                    Destroy(objVal_origin);
                    m_drawerCamera.GetComponent<DrawerCamera>().TrackingEnd();
                    SoundManager.GetInstance.PlaySE("Move_SE");
                    //指定した変更する値の軸を切り替え
                    ChangeObjectAxis();

                    originObject.transform.GetComponent<ObjectController>().DeleteOutline();
                    SwitchShotType();
                });
            });
        });

    }
}
