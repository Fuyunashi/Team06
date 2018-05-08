using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Shooter : MonoBehaviour
{
    //軸のタイプ
    enum AxisType
    {
        Xaxis, //x軸
        Yaxis, //y軸
        Zaxis  //z軸
    }
    //変更する値の指定
    enum ChangeType
    {
        Position, //位置
        Scale,    //大きさ
        Rotate    //回転
    }
    //銃のタイプ指定
    enum ShotType
    {
        Getting,    //取得
        Shifting    //転置
    }

    private AxisType axisType = AxisType.Xaxis;
    private ChangeType changeType = ChangeType.Position;
    private ShotType shotType = ShotType.Getting;

    [SerializeField]
    private GameObject bulletPrefabs;   //弾のプレハブ
    private GameObject bullet;          //弾生成ようオブジェクト
    [SerializeField]
    private Transform bulletShotPos;    //発射位置

    [SerializeField]
    private Text axisText;
    [SerializeField]
    private Text changeText;
    [SerializeField]
    private Text shotText;

    private GameObject originObject;     //数値を取得するオブジェクト
    private GameObject targetObject;     //数値を転置するオブジェクト
    private Vector3 originPositionValue; //取得するオブジェクトの位置の数値
    private Vector3 targetPositionValue; //転置するオブジェクトの位置の数値
    private Vector3 originScaleValue;    //取得するオブジェクトの大きさの数値
    private Vector3 targetScaleValue;    //転置するオブジェクトの大きさの数値
    private Vector3 originRotateValue;   //取得するオブジェクトの回転の数値
    private Vector3 targetRotateValue;   //転置するオブジェクトの回転の数値

    private bool isOriginMove;      //取得するオブジェクトが動いているか
    private bool isTargetMove;      //転置するオブジェクトが動いているか
    private bool getRTriggerDown;   //Rトリガーが押されたか
    private bool getLTriggerDown;   //Lトリガーが押されたか

    // Use this for initialization
    void Start()
    {
        //各変数の初期化
        originObject = null;
        targetObject = null;
        originPositionValue = Vector3.zero;
        targetPositionValue = Vector3.zero;
        originScaleValue = Vector3.zero;
        targetScaleValue = Vector3.zero;
        originRotateValue = Vector3.zero;
        targetRotateValue = Vector3.zero;

        isOriginMove = false;
        isTargetMove = false;

        getRTriggerDown = false;
        getLTriggerDown = false;

        axisText.text = "axis:" + axisType.ToString();
        changeText.text = "change:" + changeType.ToString();
        shotText.text = "shotType:" + shotType.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        //射撃ボタンが押されたら
        if (Input.GetMouseButtonDown(0) || Input.GetButtonDown("RButton"))
        {
            //弾を発射
            ShotBullet();
        }
        //弾が発射されていないかつショットタイプ切り替えボタンが押されたら
        if (bullet == null && (Input.GetMouseButtonDown(1) || Input.GetButtonDown("YButton")))
        {
            //ショットタイプの切り替え
            SwitchShotType();
        }
        //軸切り替えボタンが押されたら(コントローラーだとトリガーがAxisなため押されたどうか判別するためにboolを用いる)
        if (Input.GetKeyDown(KeyCode.E) || (Input.GetAxisRaw("LRTrigger") <= -0.85f && getRTriggerDown == false))
        {
            //Rトリガーが押された
            getRTriggerDown = true;
            //軸の切り替え
            SwitchAxisType();
        }
        if (getRTriggerDown == true && Input.GetAxisRaw("LRTrigger") >= -0.1f) getRTriggerDown = false;
        //値切り替えボタンが押されたら(コントローラーだとトリガーがAxisなため押されたどうか判別するためにboolを用いる)
        if (Input.GetKeyDown(KeyCode.Q) || (Input.GetAxisRaw("LRTrigger") >= 0.85f && getLTriggerDown == false))
        {
            //Lトリガーが押された
            getLTriggerDown = true;
            //値の切り替え
            SwitchChangeType();
        }
        if (getLTriggerDown == true && Input.GetAxisRaw("LRTrigger") < 0.1f) getLTriggerDown = false;
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
    //銃のタイプ切り替え
    private void SwitchShotType()
    {
        switch (shotType)
        {
            case ShotType.Getting:
                shotType = ShotType.Shifting;
                shotText.text = "shot:" + shotType.ToString();
                break;
            case ShotType.Shifting:
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
            case AxisType.Xaxis:
                axisType = AxisType.Yaxis;
                axisText.text = "axis:" + axisType.ToString();
                break;
            case AxisType.Yaxis:
                axisType = AxisType.Zaxis;
                axisText.text = "axis:" + axisType.ToString();
                break;
            case AxisType.Zaxis:
                axisType = AxisType.Xaxis;
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
                changeType = ChangeType.Rotate;
                changeText.text = "change:" + changeType.ToString();
                break;
            case ChangeType.Rotate:
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
        if (isOriginMove == false && isTargetMove == false)
        {
            //当たった時の銃のタイプによりそれぞれ処理
            switch (shotType)
            {
                //取得タイプなら
                case ShotType.Getting:
                    //取得するオブジェクトに当たったオブジェクトを格納
                    originObject = hitObject.transform.parent.gameObject;
                    break;
                //転置タイプなら
                case ShotType.Shifting:
                    //転置するオブジェクトに当たったオブジェクトを格納
                    targetObject = hitObject.transform.parent.gameObject;
                    //取得するオブジェクトが空でなければ
                    if (originObject != null)
                    {
                        //取得するオブジェクトの値を取得
                        GetOriginAxisLength();
                        //転置するオブジェクトの値を取得
                        GetTargetAxisLength();
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
        originPositionValue = originObject.transform.parent.localPosition;
        originScaleValue = originObject.transform.localScale;
        originRotateValue = originObject.transform.parent.localEulerAngles;
    }

    //転置するオブジェクトの値を取得
    private void GetTargetAxisLength()
    {
        targetPositionValue = targetObject.transform.parent.localPosition;
        targetScaleValue = targetObject.transform.localScale;
        targetRotateValue = targetObject.transform.parent.localEulerAngles;
    }

    //指定した変更する値の軸を切り替え
    private void ChangeObjectAxis()
    {
        isOriginMove = true;
        isTargetMove = true;
        //変更する値のタイプによってそれぞれ処理
        switch (changeType)
        {
            //位置
            case ChangeType.Position:
                switch (axisType)
                {
                    case AxisType.Xaxis:
                        LeanTween.moveX(targetObject.transform.parent.gameObject, originPositionValue.x, 0.5f).setOnComplete(() => { isTargetMove = false; });
                        LeanTween.moveX(originObject.transform.parent.gameObject, targetPositionValue.x, 0.5f).setOnComplete(() => { isOriginMove = false; });
                        break;
                    case AxisType.Yaxis:
                        LeanTween.moveY(targetObject.transform.parent.gameObject, originPositionValue.y, 0.5f).setOnComplete(() => { isTargetMove = false; });
                        LeanTween.moveY(originObject.transform.parent.gameObject, targetPositionValue.y, 0.5f).setOnComplete(() => { isOriginMove = false; });
                        break;
                    case AxisType.Zaxis:
                        LeanTween.moveZ(targetObject.transform.parent.gameObject, originPositionValue.z, 0.5f).setOnComplete(() => { isTargetMove = false; });
                        LeanTween.moveZ(originObject.transform.parent.gameObject, targetPositionValue.z, 0.5f).setOnComplete(() => { isOriginMove = false; });
                        break;
                }
                break;
            //大きさ
            case ChangeType.Scale:
                switch (axisType)
                {
                    case AxisType.Xaxis:
                        LeanTween.scaleX(targetObject, originScaleValue.x, 0.5f).setOnComplete(() => { isTargetMove = false; });
                        LeanTween.scaleX(originObject, targetScaleValue.x, 0.5f).setOnComplete(() => { isOriginMove = false; });
                        break;
                    case AxisType.Yaxis:
                        LeanTween.scaleY(targetObject, originScaleValue.y, 0.5f).setOnComplete(() => { isTargetMove = false; });
                        LeanTween.scaleY(originObject, targetScaleValue.y, 0.5f).setOnComplete(() => { isOriginMove = false; });
                        break;
                    case AxisType.Zaxis:
                        LeanTween.scaleZ(targetObject, originScaleValue.z, 0.5f).setOnComplete(() => { isTargetMove = false; });
                        LeanTween.scaleZ(originObject, targetScaleValue.z, 0.5f).setOnComplete(() => { isOriginMove = false; });
                        break;
                }
                break;
            //回転
            case ChangeType.Rotate:
                switch (axisType)
                {
                    case AxisType.Xaxis:
                        LeanTween.rotateX(targetObject.transform.parent.gameObject, originRotateValue.x, 0.5f).setOnComplete(() => { isTargetMove = false; });
                        LeanTween.rotateX(originObject.transform.parent.gameObject, targetRotateValue.x, 0.5f).setOnComplete(() => { isOriginMove = false; });
                        break;
                    case AxisType.Yaxis:
                        LeanTween.rotateY(targetObject.transform.parent.gameObject, originRotateValue.y, 0.5f).setOnComplete(() => { isTargetMove = false; });
                        LeanTween.rotateY(originObject.transform.parent.gameObject, targetRotateValue.y, 0.5f).setOnComplete(() => { isOriginMove = false; });
                        break;
                    case AxisType.Zaxis:
                        LeanTween.rotateZ(targetObject.transform.parent.gameObject, originRotateValue.z, 0.5f).setOnComplete(() => { isTargetMove = false; });
                        LeanTween.rotateZ(originObject.transform.parent.gameObject, targetRotateValue.z, 0.5f).setOnComplete(() => { isOriginMove = false; });
                        break;
                }
                break;
        }
    }
}
