using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XInputDotNetPure;
//銃の処理
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

    private AxisType axisType = AxisType.X;
    private ChangeType changeType = ChangeType.Position;
    [SerializeField]
    private Image changeTex;
    [SerializeField]
    private Sprite positionMark;
    [SerializeField]
    private Sprite scaleMark;
    [SerializeField]
    private Text axisTex;

    private bool playerIndexSet = false;
    private PlayerIndex playerIndex;
    private GamePadState padState;
    private GamePadState prevState;

    [SerializeField]
    private Material gunMat;
    [SerializeField]
    private Material laserPointMat;
    [SerializeField]
    private Transform shotPos;    //発射位置

    private GameObject prevOriginObj;    //前回の取得するオブジェクト
    private GameObject originObject;     //数値を取得するオブジェクト
    private GameObject targetObject;     //数値を転置するオブジェクト
    private Vector3 originPositionValue; //取得するオブジェクトの位置の数値
    private Vector3 originScaleValue;    //取得するオブジェクトの大きさの数値

    [SerializeField]
    private GameObject objValPref_ray;
    [SerializeField]
    private GameObject objValPref;
    private GameObject objVal_ray;
    private GameObject objVal_origin;
    private GameObject m_objVal_origin;
    private GameObject objVal_target;
    [SerializeField]
    private GameObject drawerCameraPref;
    private GameObject m_drawerCamera;

    private bool isTargetMove;   //転置するオブジェクトが動いているか
    private bool pushRTrigger;   //Rトリガーが押されたか
    private bool pushLTrigger;   //Lトリガーが押されたか

    //レイ・レーザーポインタ用変数
    private Ray ray;
    private RaycastHit rayhit;
    [SerializeField]
    private float rayDistance = 20.0f;
    [SerializeField]
    private LineRenderer laserPointer;
    private int layerMask;
    private GameObject prevRayHitObj;
    private GameObject currentRayHitObj;
    private bool isRayHit;

    [SerializeField]
    private Material estimateMat;
    private GameObject m_estimateObj;
    [SerializeField]
    private GameObject estimatePositionArrowPref;
    [SerializeField]
    private GameObject estimateScaleArrowPref;
    private GameObject m_estimateArrow;

    private KeyRestriction key_;
    private bool isShot;

    // Use this for initialization
    void Start()
    {
        changeTex.sprite = positionMark;
        axisTex.text = "X";

        //各変数の初期化
        prevOriginObj = null;
        originObject = null;
        targetObject = null;
        originPositionValue = Vector3.zero;
        originScaleValue = Vector3.zero;

        objVal_ray = null;
        objVal_origin = null;
        objVal_target = null;
        isTargetMove = false;

        pushRTrigger = false;
        pushLTrigger = false;

        m_estimateObj = null;
        m_estimateArrow = null;

        layerMask = ~(1 << 9 | 1 << 8);
        key_ = GameObject.Find("FPSPlayer").GetComponent<KeyRestriction>();
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

        if (!key_.Restriction()) return;

        isShot = false;
        //軸切り替えボタンが押されたら
        if (Input.GetKeyDown(KeyCode.E) || (prevState.Buttons.RightShoulder == ButtonState.Released && padState.Buttons.RightShoulder == ButtonState.Pressed))
        {
            SoundManager.GetInstance.PlaySE("Change_SE");
            //軸の切り替え
            SwitchAxisType();
            if (currentRayHitObj != null && originObject != null)
            {
                ResetEstimateObj();
                InstantEstimateObject(currentRayHitObj.transform.parent.parent.gameObject);
            }
        }
        //値切り替えボタンが押されたら
        if (Input.GetKeyDown(KeyCode.Q) || (prevState.Buttons.LeftShoulder == ButtonState.Released && padState.Buttons.LeftShoulder == ButtonState.Pressed))
        {
            SoundManager.GetInstance.PlaySE("Change_SE");
            //値の切り替え
            SwitchChangeType();
            if (currentRayHitObj != null && originObject != null)
            {
                ResetEstimateObj();
                InstantEstimateObject(currentRayHitObj.transform.parent.parent.gameObject);
            }
        }

        if (pushLTrigger == true && (padState.Triggers.Left <= 0.3f || Input.GetMouseButtonUp(1))) pushLTrigger = false;

        if (pushRTrigger == true && (padState.Triggers.Right <= 0.3f || Input.GetMouseButtonUp(0))) pushRTrigger = false;

        GunMaterialSet();
        GetObject_Ray();
        if (isRayHit == true)
        {
            ObjectsValueDraw_Ray(currentRayHitObj.transform.parent.parent.gameObject);
        }

        if (originObject != null) ObjectsValueDraw_Origin();
        if (targetObject != null) ObjectsValueDraw_Target();
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
                prevRayHitObj = currentRayHitObj;
                currentRayHitObj = rayhit.collider.transform.gameObject;
                if (prevRayHitObj != currentRayHitObj)
                {
                    if (objVal_ray != null) Destroy(objVal_ray.gameObject);
                    if (m_estimateObj != null) Destroy(m_estimateObj.gameObject);
                    if (m_estimateArrow != null) Destroy(m_estimateArrow.gameObject);
                }
                if (objVal_ray == null)
                {
                    objVal_ray = Instantiate(objValPref_ray,
                                             new Vector3(
                                                 currentRayHitObj.transform.parent.parent.position.x,
                                                 currentRayHitObj.transform.parent.parent.position.y,
                                                 currentRayHitObj.transform.parent.parent.position.z) +
                                                 new Vector3(0, currentRayHitObj.transform.parent.parent.localScale.y / 2 + 0.5f, 0),
                                             Quaternion.identity
                                            );
                    objVal_ray.GetComponent<ValueDrawerController>().GetDrawBaseObj(currentRayHitObj.transform.parent.gameObject);
                }
                //射撃ボタンが押されたら
                if ((Input.GetMouseButtonDown(0) || padState.Triggers.Left >= 0.8f) && pushRTrigger == false)
                {
                    SoundManager.GetInstance.PlaySE("Gun_SE");
                    //発射
                    GetShot(currentRayHitObj);
                    //弾を撃った
                    pushRTrigger = true;
                }

                if ((originObject != null) && (targetObject == null) && (currentRayHitObj != originObject))
                {
                    InstantEstimateObject(currentRayHitObj.transform.parent.parent.gameObject);
                    //射撃ボタンが押されたら
                    if ((Input.GetMouseButtonDown(1) || padState.Triggers.Right >= 0.8f) && pushLTrigger == false)
                    {
                        if (m_estimateObj != null) Destroy(m_estimateObj.gameObject);
                        if (m_estimateArrow != null) Destroy(m_estimateArrow.gameObject);
                        SoundManager.GetInstance.PlaySE("Gun_SE");
                        //発射
                        SetShot(currentRayHitObj);
                        ResetEstimateObj();
                        //弾を撃った
                        pushLTrigger = true;
                    }
                }
            }
            else
            {
                if (objVal_ray != null) Destroy(objVal_ray.gameObject);
                isRayHit = false;
            }
            laserPointer.SetPosition(1, rayhit.point);
        }
        else
        {
            if (objVal_ray != null) Destroy(objVal_ray.gameObject);
            isRayHit = false;
            laserPointer.SetPosition(1, ray.origin + ray.direction * rayDistance);
        }
    }
    //取得、転置オブジェクトの数値の表示
    private void ObjectsValueDraw_Ray(GameObject originObj)
    {
        if (originObj != null && objVal_ray != null)
        {
            switch (changeType)
            {
                case ChangeType.Position:
                    SetObjectValue(objVal_ray, originObj.transform.position);
                    break;
                case ChangeType.Scale:
                    SetObjectValue(objVal_ray, originObj.transform.localScale);
                    break;
            }
        }
    }
    //取得オブジェクトの数値表示処理
    private void ObjectsValueDraw_Origin()
    {
        if (objVal_origin != null)
        {
            switch (changeType)
            {
                case ChangeType.Position:
                    SetObjectValue(objVal_origin, originPositionValue);
                    break;
                case ChangeType.Scale:
                    SetObjectValue(objVal_origin, originScaleValue);
                    break;
            }
        }
    }
    //転置オブジェクトの数値表示処理
    private void ObjectsValueDraw_Target()
    {
        switch (changeType)
        {
            case ChangeType.Position:
                SetObjectValue(objVal_target, targetObject.transform.parent.parent.position);
                break;
            case ChangeType.Scale:
                SetObjectValue(objVal_target, targetObject.transform.parent.parent.localScale);
                break;
        }
    }
    private void SetObjectValue(GameObject valObj, Vector3 values)
    {
        switch (axisType)
        {
            case AxisType.X:
                valObj.GetComponent<ValueDrawerController>().SetObjectAxisValue(values.x);
                break;
            case AxisType.Y:
                valObj.GetComponent<ValueDrawerController>().SetObjectAxisValue(values.y);
                break;
            case AxisType.Z:
                valObj.GetComponent<ValueDrawerController>().SetObjectAxisValue(values.z);
                break;
        }
    }
    //コピー後の予測の生成
    private void InstantEstimateObject(GameObject obj)
    {
        if (m_estimateObj == null)
        {
            switch (changeType)
            {
                case ChangeType.Position:
                    switch (axisType)
                    {
                        case AxisType.X:
                            m_estimateObj = Instantiate(obj, new Vector3(
                                                originPositionValue.x,
                                                obj.transform.position.y,
                                                obj.transform.position.z
                                                ), obj.transform.localRotation);
                            if (originObject.transform.position.x >= obj.transform.position.x)
                                m_estimateArrow = Instantiate(estimatePositionArrowPref, obj.transform.position, Quaternion.Euler(0, 0, 0));
                            else
                                m_estimateArrow = Instantiate(estimatePositionArrowPref, obj.transform.position, Quaternion.Euler(0, 180, 0));
                            break;
                        case AxisType.Y:
                            m_estimateObj = Instantiate(obj, new Vector3(
                                               obj.transform.position.x,
                                               originPositionValue.y,
                                               obj.transform.position.z
                                               ), obj.transform.localRotation);
                            if (originObject.transform.position.y >= obj.transform.position.y)
                                m_estimateArrow = Instantiate(estimatePositionArrowPref, obj.transform.position, Quaternion.Euler(0, 0, 90));
                            else
                                m_estimateArrow = Instantiate(estimatePositionArrowPref, obj.transform.position, Quaternion.Euler(0, 0, -90));
                            break;
                        case AxisType.Z:
                            m_estimateObj = Instantiate(obj, new Vector3(
                                               obj.transform.position.x,
                                               obj.transform.position.y,
                                               originPositionValue.z
                                               ), obj.transform.localRotation);
                            if (originObject.transform.position.z >= obj.transform.position.z)
                                m_estimateArrow = Instantiate(estimatePositionArrowPref, obj.transform.position, Quaternion.Euler(0, -90, 0));
                            else
                                m_estimateArrow = Instantiate(estimatePositionArrowPref, obj.transform.position, Quaternion.Euler(0, 90, 0));
                            break;
                    }
                    m_estimateObj.transform.localScale = new Vector3(
                              obj.transform.localScale.x,
                              obj.transform.localScale.y,
                              obj.transform.localScale.z);
                    break;
                case ChangeType.Scale:
                    m_estimateObj = Instantiate(obj,
                              obj.transform.position,
                              obj.transform.localRotation);
                    switch (axisType)
                    {
                        case AxisType.X:
                            m_estimateObj.transform.localScale = new Vector3(
                                originScaleValue.x,
                                obj.transform.localScale.y,
                                obj.transform.localScale.z);
                            m_estimateArrow = Instantiate(estimateScaleArrowPref, obj.transform.position, Quaternion.Euler(0, 0, 0));
                            break;
                        case AxisType.Y:
                            m_estimateObj.transform.localScale = new Vector3(
                               obj.transform.localScale.x,
                               originScaleValue.y,
                               obj.transform.localScale.z);
                            m_estimateArrow = Instantiate(estimateScaleArrowPref, obj.transform.position, Quaternion.Euler(0, 0, 90));
                            break;
                        case AxisType.Z:
                            m_estimateObj.transform.localScale = new Vector3(
                                obj.transform.localScale.x,
                                obj.transform.localScale.y,
                                originScaleValue.z);
                            m_estimateArrow = Instantiate(estimateScaleArrowPref, obj.transform.position, Quaternion.Euler(0, 90, 0));
                            break;
                    }
                    break;
            }
            m_estimateObj.transform.GetChild(0).localPosition = obj.transform.GetChild(0).localPosition;
            m_estimateObj.transform.GetChild(0).GetChild(0).GetComponent<Renderer>().material = estimateMat;
            if (m_estimateObj.transform.GetChild(0).GetChild(0).GetComponent<Outline>() == null)
            {
                m_estimateObj.transform.GetChild(0).GetChild(0).gameObject.AddComponent<Outline>();
                m_estimateObj.transform.GetChild(0).GetChild(0).GetComponent<Outline>().OutlineWidth = 10;
                m_estimateObj.transform.GetChild(0).GetChild(0).GetComponent<Outline>().OutlineColor = Color.green;
            }
            Destroy(m_estimateObj.transform.GetChild(0).GetChild(0).GetComponent<ObjectController>());
            Destroy(m_estimateObj.transform.GetChild(0).GetChild(0).GetComponent<Rigidbody>());
            if (m_estimateObj.transform.GetChild(0).GetChild(0).GetComponent<BoxCollider>())
                Destroy(m_estimateObj.transform.GetChild(0).GetChild(0).GetComponent<BoxCollider>());
            if (m_estimateObj.transform.GetChild(0).GetChild(0).GetComponent<MeshCollider>())
                Destroy(m_estimateObj.transform.GetChild(0).GetChild(0).GetComponent<MeshCollider>());
        }
    }
    //予測オブジェクトを初期化
    private void ResetEstimateObj()
    {
        Destroy(m_estimateObj.gameObject);
        Destroy(m_estimateArrow.gameObject);
        m_estimateObj = null;
        m_estimateArrow = null;
    }
    //軸切り替え
    private void SwitchAxisType()
    {
        switch (axisType)
        {
            case AxisType.X:
                axisType = AxisType.Y;
                axisTex.text = "Y";
                break;
            case AxisType.Y:
                axisType = AxisType.Z;
                axisTex.text = "Z";
                break;
            case AxisType.Z:
                axisType = AxisType.X;
                axisTex.text = "X";
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
                changeTex.sprite = scaleMark;
                break;
            case ChangeType.Scale:
                changeType = ChangeType.Position;
                changeTex.sprite = positionMark;
                break;
        }
    }
    //銃の各タイプごとの色の設定処理
    private void GunMaterialSet()
    {
        switch (axisType)
        {
            case AxisType.X:
                gunMat.SetColor("_EmissionColor", new Color(2.0f, 0.0f, 0.0f));
                break;
            case AxisType.Y:
                gunMat.SetColor("_EmissionColor", new Color(0.0f, 0.0f, 2.0f));
                break;
            case AxisType.Z:
                gunMat.SetColor("_EmissionColor", new Color(0.0f, 2.0f, 0.0f));
                break;
        }

        switch (changeType)
        {
            case ChangeType.Position:
                laserPointMat.SetColor("_EmissionColor", new Color(1.48f, 1.5f, 0.12f));
                break;
            case ChangeType.Scale:
                laserPointMat.SetColor("_EmissionColor", new Color(1.335f, 0.12f, 1.5f));
                break;
        }
    }

    /// <summary>
    /// 弾が当たった時の処理
    /// </summary>
    /// <param name="hitObject">当たったオブジェクト</param>
    public void GetShot(GameObject hitObject)
    {
        isShot = true;
        //取得するオブジェクトと転置するオブジェクトが動いていなければ
        if (isTargetMove == false)
        {
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
                                                    originObject.transform.parent.parent.position.x,
                                                    originObject.transform.parent.parent.position.y,
                                                    originObject.transform.parent.parent.position.z) +
                                                    new Vector3(0, originObject.transform.parent.parent.localScale.y + 0.5f, 0),
                                                Quaternion.identity
                                               );
                objVal_origin.GetComponent<ValueDrawerController>().GetDrawBaseObj(originObject.transform.parent.gameObject);
                //取得するオブジェクトの値を取得
                GetOriginAxisLength();
            }
        }
    }

    //取得するオブジェクトの値を取得
    private void GetOriginAxisLength()
    {
        originPositionValue = originObject.transform.parent.parent.position;
        originScaleValue = originObject.transform.parent.parent.localScale;
    }

    public void SetShot(GameObject hitObject)
    {
        isShot = true;
        //取得するオブジェクトと転置するオブジェクトが動いていなければ
        if (isTargetMove == false)
        {
            //取得するオブジェクトが空でなければ
            if (targetObject == null)
            {
                //転置するオブジェクトに当たったオブジェクトを格納                   
                targetObject = hitObject.transform.gameObject;
                targetObject.GetComponent<ObjectController>().SetOutline();
                targetObject.AddComponent<ObjectCollider>();
                Destroy(objVal_target);
                objVal_target = Instantiate(objValPref,
                                             new Vector3(
                                                 targetObject.transform.parent.parent.position.x,
                                                 targetObject.transform.parent.parent.position.y,
                                                 targetObject.transform.parent.parent.position.z) +
                                                 new Vector3(0, targetObject.transform.parent.parent.localScale.y + 0.5f, 0),
                                             Quaternion.identity
                                            );
                objVal_target.GetComponent<ValueDrawerController>().GetDrawBaseObj(targetObject.transform.parent.gameObject);
                SettingAnimation(targetObject.transform.parent.gameObject);
            }
        }
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
                                        targetObject.transform.parent.parent.localScale.y,
                                        targetObject.transform.parent.parent.localScale.z));
                        break;
                    case AxisType.Y:
                        targetObject.transform.GetComponent<ObjectController>().ScaleShiftStart(
                            new Vector3(targetObject.transform.parent.parent.localScale.x,
                                        originScaleValue.y,
                                        targetObject.transform.parent.parent.localScale.z));
                        break;
                    case AxisType.Z:
                        targetObject.transform.GetComponent<ObjectController>().ScaleShiftStart(
                            new Vector3(targetObject.transform.parent.parent.localScale.x,
                                        targetObject.transform.parent.parent.localScale.y,
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
            Destroy(targetObject.GetComponent<ObjectCollider>());
            targetObject = null;
            Destroy(objVal_target);
            isTargetMove = false;
        }
    }
    //コピー時の演出アニメーション処理
    private void SettingAnimation(GameObject target)
    {
        m_objVal_origin = Instantiate(objVal_origin, objVal_origin.transform.position, objVal_origin.transform.rotation);
        m_drawerCamera = Instantiate(drawerCameraPref, this.transform.position + new Vector3(0, 1f, 0), GameObject.FindGameObjectWithTag("PlayCamera").transform.rotation);
        m_drawerCamera.GetComponent<DrawerCamera>().SetDrawerObj(m_objVal_origin);

        LeanTween.delayedCall(1.0f, () =>
        {
            m_objVal_origin.GetComponent<ValueDrawerController>().IsTween(true);
            LeanTween.move(m_objVal_origin, target.transform.parent.position + new Vector3(0, target.transform.localScale.y / 2 + 1.0f, 0), 1.0f).setOnComplete(() =>
            {
                LeanTween.scale(m_objVal_origin, new Vector3(0.1f, 0.1f, 0.1f), 0.5f);
                LeanTween.moveY(m_objVal_origin, m_objVal_origin.transform.position.y - 1.0f, 0.5f).setOnComplete(() =>
                {
                    m_drawerCamera.GetComponent<DrawerCamera>().TrackingEnd();
                    SoundManager.GetInstance.PlaySE("Move_SE");
                    Destroy(m_objVal_origin);
                    //指定した変更する値の軸を切り替え
                    ChangeObjectAxis();

                });
            });
        });
    }

    public GameObject GetRayObj()
    {
        return currentRayHitObj;
    }

    public string GetChangeType()
    {
        return changeType.ToString();
    }

    public string GetAxisType()
    {
        return axisType.ToString();
    }

    public bool GetIsShot()
    {
        return isShot;
    }
}
